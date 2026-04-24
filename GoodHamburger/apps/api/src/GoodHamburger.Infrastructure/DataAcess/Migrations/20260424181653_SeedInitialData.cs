using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoodHamburger.Infrastructure.DataAcess.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "Address", "CreatedAt", "Email", "FirstName", "LastName", "Phone", "UpdatedAt" },
                values: new object[] { new Guid("c3000000-0000-0000-0000-000000000001"), "Rio de Janeiro", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Lucas", "Silva", "+55 21 97534-2254", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "CreatedAt", "Currency", "Description", "Name", "Price", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("a1000000-0000-0000-0000-000000000001"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BRL", "Pão, hambúrguer artesanal e queijo", "X Burger", 5m, "Available", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a1000000-0000-0000-0000-000000000002"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BRL", "Pão, hambúrguer artesanal, queijo e ovo", "X Egg", 4.50m, "Available", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("a1000000-0000-0000-0000-000000000003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BRL", "Pão, hambúrguer artesanal, queijo e bacon", "X Bacon", 7m, "Available", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "SideDishes",
                columns: new[] { "Id", "Category", "CreatedAt", "Currency", "Description", "Name", "Price", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("b2000000-0000-0000-0000-000000000001"), "FRIES", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BRL", "Porção de batata frita crocante", "Batata Frita", 2m, "Available", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b2000000-0000-0000-0000-000000000002"), "DRINK", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BRL", "Lata 350ml", "Refrigerante", 2.50m, "Available", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: new Guid("c3000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: new Guid("a1000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "SideDishes",
                keyColumn: "Id",
                keyValue: new Guid("b2000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "SideDishes",
                keyColumn: "Id",
                keyValue: new Guid("b2000000-0000-0000-0000-000000000002"));
        }
    }
}
