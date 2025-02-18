using Microsoft.EntityFrameworkCore.Migrations;
using Tazkartk.Models.Enums;

#nullable disable

namespace Tazkartk.Migrations
{
    /// <inheritdoc />
    public partial class companyrole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "AspNetRoles",
               columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
               values: new object[] { 3, Roles.Company.ToString(), Roles.Company.ToString().ToUpper(), Guid.NewGuid().ToString() }

               );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
         migrationBuilder.DeleteData(
         table: "AspNetRoles",
         keyColumn: "Id",
         keyValue: 3
     );
        }
    }
}
