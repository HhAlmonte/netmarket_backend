using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinessLogic.Data.Migrations
{
    public partial class MigracionOrdenCompra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Categoria_categoriaId",
                table: "Producto");

            migrationBuilder.RenameColumn(
                name: "categoriaId",
                table: "Producto",
                newName: "CategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Producto_categoriaId",
                table: "Producto",
                newName: "IX_Producto_CategoriaId");

            migrationBuilder.AlterColumn<string>(
                name: "Imagen",
                table: "Producto",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Marca",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Categoria",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "TipoEnvios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoEnvios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrdenCompras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompradorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrdenCompraFecha = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DireccionEnvio_Calle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DireccionEnvio_Ciudad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DireccionEnvio_Departamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DireccionEnvio_CodigoPostal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DireccionEnvio_Pais = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoEnvioId = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PagoIntentoId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenCompras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenCompras_TipoEnvios_TipoEnvioId",
                        column: x => x.TipoEnvioId,
                        principalTable: "TipoEnvios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemOrdenado_ProductoItemId = table.Column<int>(type: "int", nullable: false),
                    ItemOrdenado_ProductoName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemOrdenado_ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    OrdenComprasId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenItems_OrdenCompras_OrdenComprasId",
                        column: x => x.OrdenComprasId,
                        principalTable: "OrdenCompras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenCompras_TipoEnvioId",
                table: "OrdenCompras",
                column: "TipoEnvioId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenItems_OrdenComprasId",
                table: "OrdenItems",
                column: "OrdenComprasId");

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Categoria_CategoriaId",
                table: "Producto",
                column: "CategoriaId",
                principalTable: "Categoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Producto_Categoria_CategoriaId",
                table: "Producto");

            migrationBuilder.DropTable(
                name: "OrdenItems");

            migrationBuilder.DropTable(
                name: "OrdenCompras");

            migrationBuilder.DropTable(
                name: "TipoEnvios");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "Producto",
                newName: "categoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Producto_CategoriaId",
                table: "Producto",
                newName: "IX_Producto_categoriaId");

            migrationBuilder.AlterColumn<string>(
                name: "Imagen",
                table: "Producto",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Marca",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Categoria",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Producto_Categoria_categoriaId",
                table: "Producto",
                column: "categoriaId",
                principalTable: "Categoria",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
