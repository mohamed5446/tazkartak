using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tazkartk.Migrations
{
    /// <inheritdoc />
    public partial class messages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_bookings_bookingId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_bookingId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TripCode",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "OTP",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OTPExpiry",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "bookingId",
                table: "Payments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_bookingId",
                table: "Payments",
                column: "bookingId",
                unique: true,
                filter: "[bookingId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_bookings_bookingId",
                table: "Payments",
                column: "bookingId",
                principalTable: "bookings",
                principalColumn: "BookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_bookings_bookingId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Payments_bookingId",
                table: "Payments");

            migrationBuilder.AddColumn<int>(
                name: "TripCode",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "bookingId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OTP",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OTPExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_bookingId",
                table: "Payments",
                column: "bookingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_bookings_bookingId",
                table: "Payments",
                column: "bookingId",
                principalTable: "bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
