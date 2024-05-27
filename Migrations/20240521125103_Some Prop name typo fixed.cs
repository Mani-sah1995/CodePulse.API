using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodePulse.API.Migrations
{
    /// <inheritdoc />
    public partial class SomePropnametypofixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tile",
                table: "BlogImages",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "DataCreated",
                table: "BlogImages",
                newName: "DateCreated");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "BlogImages",
                newName: "Tile");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "BlogImages",
                newName: "DataCreated");
        }
    }
}
