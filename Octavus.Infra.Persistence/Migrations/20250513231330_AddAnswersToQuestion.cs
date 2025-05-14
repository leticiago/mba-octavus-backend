using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Octavus.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAnswersToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
