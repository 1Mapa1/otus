using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BillingService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHeldAndSagaLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "held_amount",
                table: "accounts",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "balance_before",
                table: "account_transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "held_after",
                table: "account_transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "held_before",
                table: "account_transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "payment_id",
                table: "account_transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    authorized_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    captured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    canceled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                    table.ForeignKey(
                        name: "FK_payments_accounts_user_id",
                        column: x => x.user_id,
                        principalTable: "accounts",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_transactions_payment_id",
                table: "account_transactions",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_payments_order_id",
                table: "payments",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_user_id",
                table: "payments",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_account_transactions_payments_payment_id",
                table: "account_transactions",
                column: "payment_id",
                principalTable: "payments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_account_transactions_payments_payment_id",
                table: "account_transactions");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropIndex(
                name: "IX_account_transactions_payment_id",
                table: "account_transactions");

            migrationBuilder.DropColumn(
                name: "held_amount",
                table: "accounts");

            migrationBuilder.DropColumn(
                name: "held_after",
                table: "account_transactions");

            migrationBuilder.DropColumn(
                name: "held_before",
                table: "account_transactions");

            migrationBuilder.DropColumn(
                name: "payment_id",
                table: "account_transactions");

            migrationBuilder.AlterColumn<decimal>(
                name: "balance_before",
                table: "account_transactions",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);
        }
    }
}
