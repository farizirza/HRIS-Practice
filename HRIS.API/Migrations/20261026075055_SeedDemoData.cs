using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRIS.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedDemoData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "Name", "ParentDepartmentId" },
                values: new object[,]
                {
                    { new Guid("0199f200-0000-7000-8000-0000000000d1"), "TECH", "Technology", null },
                    { new Guid("0199f200-0000-7000-8000-0000000000d3"), "PEOPLE", "People Operations", null }
                });


            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "Code", "Name", "ParentDepartmentId" },
                values: new object[] { new Guid("0199f200-0000-7000-8000-0000000000d2"), "TECH-BE", "Backend", new Guid("0199f200-0000-7000-8000-0000000000d1") });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "DepartmentId", "Email", "EmployeeNumber", "FullName", "HireDate", "ManagerId", "PositionId", "Status", "TerminatedAt" },
                values: new object[,]
                {
                    { new Guid("0199f200-0000-7000-8000-0000000000e1"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d1"), "dewi@demo.hris.test", "EMP-1001", "Dewi Lestari", new DateOnly(2020, 1, 6), null, new Guid("01a166be-ee55-747c-b090-72cbd94cb9f2"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000105"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff5@demo.hris.test", "EMP-1005", "Staff 5", new DateOnly(2024, 1, 5), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000106"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff6@demo.hris.test", "EMP-1006", "Staff 6", new DateOnly(2024, 1, 6), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000107"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff7@demo.hris.test", "EMP-1007", "Staff 7", new DateOnly(2024, 1, 7), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000108"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff8@demo.hris.test", "EMP-1008", "Staff 8", new DateOnly(2024, 1, 8), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000109"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff9@demo.hris.test", "EMP-1009", "Staff 9", new DateOnly(2024, 1, 9), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000110"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff10@demo.hris.test", "EMP-1010", "Staff 10", new DateOnly(2024, 1, 10), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000111"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff11@demo.hris.test", "EMP-1011", "Staff 11", new DateOnly(2024, 1, 11), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000112"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff12@demo.hris.test", "EMP-1012", "Staff 12", new DateOnly(2024, 1, 12), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000113"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff13@demo.hris.test", "EMP-1013", "Staff 13", new DateOnly(2024, 1, 13), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-000000000114"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d3"), "staff14@demo.hris.test", "EMP-1014", "Staff 14", new DateOnly(2024, 1, 14), null, new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-0000000000e2"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d1"), "budi@demo.hris.test", "EMP-1002", "Budi Hartono", new DateOnly(2021, 3, 1), new Guid("0199f200-0000-7000-8000-0000000000e1"), new Guid("01a166be-ee55-747c-b090-6c3b9bd9cd5c"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-0000000000e3"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d2"), "siti@demo.hris.test", "EMP-1003", "Siti Aminah", new DateOnly(2022, 6, 15), new Guid("0199f200-0000-7000-8000-0000000000e2"), new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null },
                    { new Guid("0199f200-0000-7000-8000-0000000000e4"), new DateTimeOffset(new DateTime(2026, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), new Guid("0199f200-0000-7000-8000-0000000000d2"), "andi@demo.hris.test", "EMP-1004", "Andi Pratama", new DateOnly(2022, 9, 1), new Guid("0199f200-0000-7000-8000-0000000000e2"), new Guid("01a166be-ee55-747c-b090-69f7572f26e8"), "Active", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-0000000000e3"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-0000000000e4"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000105"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000106"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000107"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000108"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000109"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000110"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000111"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000112"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000113"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-000000000114"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-0000000000d2"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-0000000000d3"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-0000000000e2"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-0000000000e1"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("0199f200-0000-7000-8000-0000000000d1"));
        }
    }
}
