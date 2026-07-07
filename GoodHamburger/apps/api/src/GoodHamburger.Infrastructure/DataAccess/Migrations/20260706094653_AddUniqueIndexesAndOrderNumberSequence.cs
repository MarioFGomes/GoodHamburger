using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodHamburger.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesAndOrderNumberSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "OrderNumbers");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_Name",
                table: "Menus",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Phone",
                table: "Customer",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderSideDishes_SideDishes_SideDishesId",
                table: "OrderSideDishes",
                column: "SideDishesId",
                principalTable: "SideDishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderSideDishes_SideDishes_SideDishesId",
                table: "OrderSideDishes");

            migrationBuilder.DropIndex(
                name: "IX_Menus_Name",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Phone",
                table: "Customer");

            migrationBuilder.DropSequence(
                name: "OrderNumbers");
        }
    }
}
