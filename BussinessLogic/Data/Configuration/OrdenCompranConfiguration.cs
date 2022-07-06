using Core.Entities.OrdenCompra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BussinessLogic.Data.Configuration
{
    public class OrdenCompranConfiguration : IEntityTypeConfiguration<OrdenCompras>
    {
        public void Configure(EntityTypeBuilder<OrdenCompras> builder)
        {
            builder.OwnsOne(o => o.DireccionEnvio, x =>
            {
                x.WithOwner();
            });

            builder.Property(s => s.Status)
                .HasConversion(
                    o => o.ToString(),
                    o => (OrdenStatus)Enum.Parse(typeof(OrdenStatus), o)
                 );

            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.SubTotal)
                .HasColumnType("decimal(18,2)");
        }
    }
}
