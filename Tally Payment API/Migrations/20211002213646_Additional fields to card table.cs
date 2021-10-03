using Microsoft.EntityFrameworkCore.Migrations;

namespace Tally_Payment_API.Migrations
{
    public partial class Additionalfieldstocardtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "cardBIN",
                table: "CardTokenTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "expirymonth",
                table: "CardTokenTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "expiryyear",
                table: "CardTokenTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last4digits",
                table: "CardTokenTable",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "CardTokenTable",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cardBIN",
                table: "CardTokenTable");

            migrationBuilder.DropColumn(
                name: "expirymonth",
                table: "CardTokenTable");

            migrationBuilder.DropColumn(
                name: "expiryyear",
                table: "CardTokenTable");

            migrationBuilder.DropColumn(
                name: "last4digits",
                table: "CardTokenTable");

            migrationBuilder.DropColumn(
                name: "type",
                table: "CardTokenTable");
        }
    }
}
