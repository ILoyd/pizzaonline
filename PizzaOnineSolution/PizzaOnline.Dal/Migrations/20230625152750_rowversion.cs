using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaOnline.Dal.Migrations
{
    public partial class rowversion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Pizzas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Pizzas",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Pizzas");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Pizzas");
        }
    }
}
