using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace payfish.Migrations
{
    /// <inheritdoc />
    public partial class SplitFullName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // مقداردهی اولیه بر اساس FullName
            migrationBuilder.Sql(@"
        UPDATE Employees
        SET 
            FirstName = LEFT(FullName, CHARINDEX(' ', FullName + ' ') - 1),
            LastName = LTRIM(SUBSTRING(FullName, CHARINDEX(' ', FullName + ' '), LEN(FullName)))
    ");

            // حذف ستون FullName
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Employees");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
        UPDATE Employees
        SET FullName = FirstName + ' ' + LastName
    ");

            migrationBuilder.DropColumn(name: "FirstName", table: "Employees");
            migrationBuilder.DropColumn(name: "LastName", table: "Employees");
        }
    }
}
