using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Octavus.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddActivityProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Activities",
                newName: "IsPublic");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsPublic",
                table: "Activities",
                newName: "Status");
        }
    }
}
