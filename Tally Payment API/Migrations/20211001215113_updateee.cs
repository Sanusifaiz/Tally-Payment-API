using Microsoft.EntityFrameworkCore.Migrations;

namespace Tally_Payment_API.Migrations
{
    public partial class updateee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentFrontEndUrl",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentFrontEndUrl",
                table: "Transactions");
        }
    }
}
