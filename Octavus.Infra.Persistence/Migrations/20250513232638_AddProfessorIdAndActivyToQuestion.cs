using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Octavus.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class AddProfessorIdAndActivyToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Questions_QuestionId1",
                table: "Answers");

            migrationBuilder.DropIndex(
                name: "IX_Answers_QuestionId1",
                table: "Answers");

            migrationBuilder.DropColumn(
                name: "QuestionId1",
                table: "Answers");

            migrationBuilder.AddColumn<Guid>(
                name: "ActivityId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessorId",
                table: "Activities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ActivityId",
                table: "Questions",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_InstrumentId",
                table: "Activities",
                column: "InstrumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Instruments_InstrumentId",
                table: "Activities",
                column: "InstrumentId",
                principalTable: "Instruments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Activities_ActivityId",
                table: "Questions",
                column: "ActivityId",
                principalTable: "Activities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Instruments_InstrumentId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Activities_ActivityId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_ActivityId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Activities_InstrumentId",
                table: "Activities");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ProfessorId",
                table: "Activities");

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionId1",
                table: "Answers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Answers_QuestionId1",
                table: "Answers",
                column: "QuestionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Questions_QuestionId1",
                table: "Answers",
                column: "QuestionId1",
                principalTable: "Questions",
                principalColumn: "Id");
        }
    }
}
