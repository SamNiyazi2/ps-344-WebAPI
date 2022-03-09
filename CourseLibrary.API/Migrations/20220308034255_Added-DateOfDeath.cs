using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseLibrary.API.Migrations
{
    public partial class AddedDateOfDeath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DateOfDeath",
                schema: "ps-344",
                table: "Authors",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfDeath",
                schema: "ps-344",
                table: "Authors");
        }
    }
}
