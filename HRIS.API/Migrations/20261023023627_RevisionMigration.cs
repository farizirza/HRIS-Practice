using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRIS.API.Migrations
{
    /// <inheritdoc />
    public partial class RevisionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentDeparmentId",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "ParentDeparmentId",
                table: "Departments",
                newName: "ParentDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_ParentDeparmentId",
                table: "Departments",
                newName: "IX_Departments_ParentDepartmentId");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeStatus",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Active");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "Employees",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments",
                column: "ParentDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Departments_ParentDepartmentId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "ParentDepartmentId",
                table: "Departments",
                newName: "ParentDeparmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_ParentDepartmentId",
                table: "Departments",
                newName: "IX_Departments_ParentDeparmentId");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeStatus",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Active",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Departments_ParentDeparmentId",
                table: "Departments",
                column: "ParentDeparmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
