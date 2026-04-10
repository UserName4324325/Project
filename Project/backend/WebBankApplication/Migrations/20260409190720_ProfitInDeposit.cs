using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBankApplication.Migrations
{
    /// <inheritdoc />
    public partial class ProfitInDeposit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Profit",
                table: "Deposits",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profit",
                table: "Deposits");
        }
    }
}
