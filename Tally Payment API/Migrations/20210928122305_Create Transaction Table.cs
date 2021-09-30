using Microsoft.EntityFrameworkCore.Migrations;

namespace Tally_Payment_API.Migrations
{
    public partial class CreateTransactionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "From",
                table: "userPaymentModels",
                newName: "inData");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "userPaymentModels",
                newName: "outData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "outData",
                table: "userPaymentModels",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "inData",
                table: "userPaymentModels",
                newName: "From");
        }
    }
}
