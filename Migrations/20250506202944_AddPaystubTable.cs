using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace payfish.Migrations
{
    /// <inheritdoc />
    public partial class AddPaystubTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Paystubs",
                newName: "FileName");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "Paystubs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Paystubs");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Paystubs",
                newName: "FilePath");
        }
    }
}
