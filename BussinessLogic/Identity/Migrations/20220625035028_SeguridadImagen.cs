﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BussinessLogic.Identity.Migrations
{
    public partial class SeguridadImagen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imagen",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imagen",
                table: "AspNetUsers");
        }
    }
}
