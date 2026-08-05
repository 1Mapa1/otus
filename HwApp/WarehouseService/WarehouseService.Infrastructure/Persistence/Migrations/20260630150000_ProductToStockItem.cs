using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProductToStockItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_stock_reservation_items_products_product_id",
                table: "stock_reservation_items");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "stock_items");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "stock_items",
                newName: "product_id");

            migrationBuilder.RenameIndex(
                name: "PK_products",
                table: "stock_items",
                newName: "PK_stock_items");

            migrationBuilder.Sql(
                """
                UPDATE stock_items
                SET available_quantity = available_quantity - reserved_quantity;
                """);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "stock_items",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.DropColumn(
                name: "name",
                table: "stock_items");

            migrationBuilder.DropColumn(
                name: "unit_price",
                table: "stock_items");

            migrationBuilder.AddCheckConstraint(
                name: "CK_stock_items_available_quantity",
                table: "stock_items",
                sql: "available_quantity >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_stock_items_reserved_quantity",
                table: "stock_items",
                sql: "reserved_quantity >= 0");

            migrationBuilder.AddForeignKey(
                name: "FK_stock_reservation_items_stock_items_product_id",
                table: "stock_reservation_items",
                column: "product_id",
                principalTable: "stock_items",
                principalColumn: "product_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.CreateTable(
                name: "stock_movements",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_movements", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_movements_stock_items_product_id",
                        column: x => x.product_id,
                        principalTable: "stock_items",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    topic = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    key = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_product_id",
                table: "stock_movements",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_movements_product_id_created_at",
                table: "stock_movements",
                columns: new[] { "product_id", "created_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "stock_movements");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_reservation_items_stock_items_product_id",
                table: "stock_reservation_items");

            migrationBuilder.DropCheckConstraint(
                name: "CK_stock_items_available_quantity",
                table: "stock_items");

            migrationBuilder.DropCheckConstraint(
                name: "CK_stock_items_reserved_quantity",
                table: "stock_items");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "stock_items");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "stock_items",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "unit_price",
                table: "stock_items",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(
                """
                UPDATE stock_items
                SET available_quantity = available_quantity + reserved_quantity;
                """);

            migrationBuilder.RenameTable(
                name: "stock_items",
                newName: "products");

            migrationBuilder.RenameColumn(
                name: "product_id",
                table: "products",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "PK_stock_items",
                table: "products",
                newName: "PK_products");

            migrationBuilder.AddForeignKey(
                name: "FK_stock_reservation_items_products_product_id",
                table: "stock_reservation_items",
                column: "product_id",
                principalTable: "products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
