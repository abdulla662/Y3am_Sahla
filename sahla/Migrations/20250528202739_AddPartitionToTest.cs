using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sahla.Migrations
{
    /// <inheritdoc />
    public partial class AddPartitionToTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Partition",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Partition",
                table: "Tests");
        }
    }
}
