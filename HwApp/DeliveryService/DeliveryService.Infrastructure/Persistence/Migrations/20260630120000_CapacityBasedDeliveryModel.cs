using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CapacityBasedDeliveryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_reservation");

            migrationBuilder.DropTable(
                name: "delivery_slot");

            migrationBuilder.CreateTable(
                name: "delivery_zone",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery_zone", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "delivery_slot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    zone_id = table.Column<Guid>(type: "uuid", nullable: false),
                    time_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    time_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    reserved_count = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery_slot", x => x.id);
                    table.CheckConstraint("CK_delivery_slot_capacity", "capacity > 0");
                    table.CheckConstraint("CK_delivery_slot_reserved_count", "reserved_count >= 0");
                    table.CheckConstraint("CK_delivery_slot_reserved_capacity", "reserved_count <= capacity");
                    table.ForeignKey(
                        name: "FK_delivery_slot_delivery_zone_zone_id",
                        column: x => x.zone_id,
                        principalTable: "delivery_zone",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "delivery_reservation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_slot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    zone_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    address_city = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    address_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    address_house = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    address_apartment = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    canceled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery_reservation", x => x.id);
                    table.ForeignKey(
                        name: "FK_delivery_reservation_delivery_slot_delivery_slot_id",
                        column: x => x.delivery_slot_id,
                        principalTable: "delivery_slot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX "IX_delivery_zone_city_lower" ON delivery_zone (LOWER(TRIM(city)));
                """);

            migrationBuilder.CreateIndex(
                name: "IX_delivery_slot_zone_id",
                table: "delivery_slot",
                column: "zone_id");

            migrationBuilder.CreateIndex(
                name: "IX_delivery_reservation_customer_id",
                table: "delivery_reservation",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_delivery_reservation_delivery_slot_id",
                table: "delivery_reservation",
                column: "delivery_slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_delivery_reservation_order_id",
                table: "delivery_reservation",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_delivery_reservation_zone_id",
                table: "delivery_reservation",
                column: "zone_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_reservation");

            migrationBuilder.DropTable(
                name: "delivery_slot");

            migrationBuilder.DropTable(
                name: "delivery_zone");

            migrationBuilder.CreateTable(
                name: "delivery_slot",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    time_from = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    time_to = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery_slot", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "delivery_reservation",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_slot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    canceled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_delivery_reservation", x => x.id);
                    table.ForeignKey(
                        name: "FK_delivery_reservation_delivery_slot_delivery_slot_id",
                        column: x => x.delivery_slot_id,
                        principalTable: "delivery_slot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_delivery_reservation_delivery_slot_id",
                table: "delivery_reservation",
                column: "delivery_slot_id");

            migrationBuilder.CreateIndex(
                name: "IX_delivery_reservation_order_id",
                table: "delivery_reservation",
                column: "order_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_delivery_reservation_user_id",
                table: "delivery_reservation",
                column: "user_id");
        }
    }
}
