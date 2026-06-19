using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSagaColumnOrderAndTableOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "price",
                table: "orders",
                newName: "total_amount");

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "orders",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<DateTime>(
                name: "completed_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "delivery_reservation_id",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "delivery_slot_id",
                table: "orders",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "failure_details",
                table: "orders",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "locked_by",
                table: "orders",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "locked_until",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "next_retry_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "payment_id",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "rejected_at",
                table: "orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "retry_count",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "saga_step",
                table: "orders",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "stock_reservation_id",
                table: "orders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "order_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_items_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_delivery_slot_id",
                table: "orders",
                column: "delivery_slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_order_id",
                table: "order_items",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_order_items_product_id",
                table: "order_items",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_items");

            migrationBuilder.DropIndex(
                name: "IX_orders_delivery_slot_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "completed_at",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivery_reservation_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "delivery_slot_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "failure_details",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "locked_by",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "locked_until",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "next_retry_at",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "payment_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "rejected_at",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "retry_count",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "saga_step",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "stock_reservation_id",
                table: "orders");

            migrationBuilder.RenameColumn(
                name: "total_amount",
                table: "orders",
                newName: "price");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "orders",
                type: "integer",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);
        }
    }
}
