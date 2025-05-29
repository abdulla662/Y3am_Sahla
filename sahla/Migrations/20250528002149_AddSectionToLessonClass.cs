using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sahla.Migrations
{
    /// <inheritdoc />
    public partial class AddSectionToLessonClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Section",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Section",
                table: "Lessons");
        }
    }
}
