using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRIS.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedPositions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "Level", "Title" },
                values: new object[,]
                {
                    { new Guid("01a166bf-2518-743a-bc79-972df6de3ecc"), 2, "Staff" },
                    { new Guid("01a166bf-2518-743a-bc79-98d145939083"), 5, "Supervisor" },
                    { new Guid("01a166bf-2518-743a-bc79-9e1819223d9f"), 8, "Department Head" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: new Guid("01a166bf-2518-743a-bc79-972df6de3ecc"));

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: new Guid("01a166bf-2518-743a-bc79-98d145939083"));

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: new Guid("01a166bf-2518-743a-bc79-9e1819223d9f"));
        }
    }
}
