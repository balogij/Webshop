using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComputerShopApi.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoriesAndComponents2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Computers_Computers_ComputersId",
                table: "Computers");

            migrationBuilder.DropIndex(
                name: "IX_Computers_ComputersId",
                table: "Computers");

            migrationBuilder.DropColumn(
                name: "ComputersId",
                table: "Computers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ComputersId",
                table: "Computers",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Computers_ComputersId",
                table: "Computers",
                column: "ComputersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Computers_Computers_ComputersId",
                table: "Computers",
                column: "ComputersId",
                principalTable: "Computers",
                principalColumn: "Id");
        }
    }
}
