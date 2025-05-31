using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Octavus.Infra.Persistence.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddNewColumnsAndTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessorStudents",
                table: "ProfessorStudents");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProfessorId",
                table: "ProfessorStudents",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessorStudents",
                table: "ProfessorStudents",
                columns: new[] { "ProfessorId", "StudentId" });

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ActivityStudents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "CorrectionDate",
                table: "ActivityStudents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrected",
                table: "ActivityStudents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ActivityStudents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfessorStudents",
                table: "ProfessorStudents");

            migrationBuilder.AlterColumn<string>(
                name: "ProfessorId",
                table: "ProfessorStudents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfessorStudents",
                table: "ProfessorStudents",
                columns: new[] { "ProfessorId", "StudentId" });

            migrationBuilder.DropColumn(
                name: "CorrectionDate",
                table: "ActivityStudents");

            migrationBuilder.DropColumn(
                name: "IsCorrected",
                table: "ActivityStudents");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ActivityStudents");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ActivityStudents",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
