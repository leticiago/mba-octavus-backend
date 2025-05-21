using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Octavus.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class OpenTextAnswerMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OpenTextAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponseText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenTextAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpenTextAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpenTextAnswers_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpenTextAnswers_QuestionId",
                table: "OpenTextAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpenTextAnswers_StudentId",
                table: "OpenTextAnswers",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpenTextAnswers");
        }
    }
}
