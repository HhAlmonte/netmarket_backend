using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace BussinessLogic.Data
{
    public class SeguridadDbContextData
    {
        public static async Task SeedUserAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                var usuario = new Usuario
                {
                    Nombre = "Hector",
                    Apellido = "Almonte",
                    UserName = "Sstewiie",
                    Email = "Hbalmontess@gmail.com",
                    Direccion = new Direccion
                    {
                        Calle = "Calle Enriquillo 8",
                        Ciudad = "Santo Domingo",
                        CodigoPostal = "19002",
                        Departamento = "Santo Domingo Este"
                    }
                };

                await userManager.CreateAsync(usuario, "Bryan12s#");
            }

            if (!roleManager.Roles.Any()) 
            {
                var role = new IdentityRole
                {
                    Name = "ADMIN"
                };

                await roleManager.CreateAsync(role);
            }
        }
    }
}
