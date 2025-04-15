using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tazkartk.Migrations
{
    /// <inheritdoc />
    public partial class addedcompanybalanceprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Balance",
                table: "Companies",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Companies");
        }
    }
}
