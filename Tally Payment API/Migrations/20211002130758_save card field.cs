using Microsoft.EntityFrameworkCore.Migrations;

namespace Tally_Payment_API.Migrations
{
    public partial class savecardfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SaveCard",
                table: "Transactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaveCard",
                table: "Transactions");
        }
    }
}
