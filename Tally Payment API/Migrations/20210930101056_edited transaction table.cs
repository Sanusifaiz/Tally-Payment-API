using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tally_Payment_API.Migrations
{
    public partial class editedtransactiontable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "appfee",
                table: "Transactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "authurl",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chargeResponseCode",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "chargeResponseMessage",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "charged_amount",
                table: "Transactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "merchantfee",
                table: "Transactions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "narration",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "orderRef",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "appfee",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "authurl",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "chargeResponseCode",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "chargeResponseMessage",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "charged_amount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "merchantfee",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "narration",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "orderRef",
                table: "Transactions");
        }
    }
}
