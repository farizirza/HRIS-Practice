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
                    { new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), 2, "Staff" },
                    { new Guid("01a166be-ee55-747c-b090-6c3b9bd9cd5c"), 5, "Supervisor" },
                    { new Guid("01a166be-ee55-747c-b090-72cbd94cb9f2"), 8, "Department Head" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: new Guid("01a166be-ee55-747c-b090-69f7572f26e8"));

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: new Guid("01a166be-ee55-747c-b090-6c3b9bd9cd5c"));

            migrationBuilder.DeleteData(
                table: "Positions",
                keyColumn: "Id",
                keyValue: new Guid("01a166be-ee55-747c-b090-72cbd94cb9f2"));
        }
    }
}
