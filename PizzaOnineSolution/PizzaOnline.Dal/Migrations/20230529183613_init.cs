using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PizzaOnline.Dal.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pizzas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StuffedCrust = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pizzas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    PizzaId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderItem_Pizzas_PizzaId",
                        column: x => x.PizzaId,
                        principalTable: "Pizzas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Pizzas",
                columns: new[] { "Id", "Description", "ImageUrl", "Name", "Size", "StuffedCrust", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "seasoned-tomato sauce, mozzarella", "/Images/pizza_1.jpg", "Margherita", "Small", "Normal", 1000 },
                    { 2, "tomato sauce, ham, sweetcorn, mozzarella", "/Images/pizza_2.jpg", "Ham & Sweetcorn", "Medium", "Cheese", 1500 },
                    { 3, "tomato sauce, ham, mushrooms, mozzarella, sweetcorn", "/Images/pizza_3.jpg", "SonGoKu", "Large", "Normal", 2000 },
                    { 4, "tomato sauce, salami, bacon, onions, pepperoni pepper, mozzarella", "/Images/pizza_4.jpg", "Hungarian", "Medium", "Jalapeno", 3000 },
                    { 5, "cream base, mozzarella, garlic, tomato slices, sausage, ham", "/Images/pizza_5.jpg", "Garlic-Cream", "Small", "Normal", 3000 },
                    { 6, "bolognese sauce, mozzarella", "/Images/pizza_6.jpg", "Bolognese", "Medium", "Normal", 2500 },
                    { 7, "tomato sauce, marble cheese, parmesan, cheddar, mozzarella", "/Images/pizza_7.jpg", "Four Cheese", "Large", "Beacon", 2300 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_PizzaId",
                table: "OrderItem",
                column: "PizzaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Pizzas");
        }
    }
}
