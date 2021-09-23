using Microsoft.EntityFrameworkCore.Migrations;

namespace Tally_Payment_API.Migrations
{
    public partial class AddedFromcolumntotable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "payer",
                table: "userPaymentModels",
                newName: "Payer");

            migrationBuilder.AddColumn<string>(
                name: "From",
                table: "userPaymentModels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "From",
                table: "userPaymentModels");

            migrationBuilder.RenameColumn(
                name: "Payer",
                table: "userPaymentModels",
                newName: "payer");
        }
    }
}
