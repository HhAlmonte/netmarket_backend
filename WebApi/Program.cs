using BussinessLogic.Data;
using BussinessLogic.Logic;
using Core.Entities;
using Core.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using WebApi.DTOs;
using WebApi.Middleware;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);
var _builder = builder.Services.AddIdentityCore<Usuario>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IOrdenCompraService, OrdenComprasService>();

_builder = new IdentityBuilder(_builder.UserType, _builder.Services);
_builder.AddRoles<IdentityRole>();
_builder.AddEntityFrameworkStores<SeguridadDbContext>();
_builder.AddSignInManager<SignInManager<Usuario>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:Key"])),
        ValidIssuer = builder.Configuration["Token:Issuer"],
        ValidateIssuer = true,
        ValidateAudience = false
    };
});

builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericSeguridadRepository<>), typeof(GenericSeguridadRepository<>));

builder.Services.AddDbContext<MarketDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<SeguridadDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("IdentitySeguridad"));
});

builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var configuration = ConfigurationOptions.Parse(builder.Configuration.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.TryAddSingleton<ISystemClock, SystemClock>();

builder.Services.AddTransient<IProductoRepository, ProductoRepository>();

builder.Services.AddControllers();

builder.Services.AddScoped<ICarritoCompraRepository, CarritoCompraRepository>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsRule", rule =>
    {
        rule.AllowAnyHeader()
        .AllowAnyHeader()
        .WithOrigins("*");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try
    {
        var context = services.GetRequiredService<MarketDbContext>();

        //Migracion y carga de datos para la base de datos de productos
        await context.Database.MigrateAsync();
        await MarketDbContextData.CargarDataAsync(context, loggerFactory);


        //Migracion y carga de datos para la base de datos de Usuario
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        var identityContext = services.GetRequiredService<SeguridadDbContext>();
        await identityContext.Database.MigrateAsync();
        await SeguridadDbContextData.SeedUserAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "Errores en el proceso de migracion");
    }
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors", "?code={0}");

app.UseRouting();

app.UseCors("CorsRule");

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();