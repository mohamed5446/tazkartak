using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tazkartk.Migrations
{
    /// <inheritdoc />
    public partial class GuestNameandPhoneinbooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_Users_UserId",
                table: "bookings");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "bookings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "GuestFirstName",
                table: "bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuestLastName",
                table: "bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuestPhoneNumber",
                table: "bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_Users_UserId",
                table: "bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bookings_Users_UserId",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "GuestFirstName",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "GuestLastName",
                table: "bookings");

            migrationBuilder.DropColumn(
                name: "GuestPhoneNumber",
                table: "bookings");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "bookings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_bookings_Users_UserId",
                table: "bookings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
