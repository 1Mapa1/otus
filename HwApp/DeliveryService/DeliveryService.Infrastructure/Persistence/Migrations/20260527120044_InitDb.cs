using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_reservation");

            migrationBuilder.DropTable(
                name: "delivery_slot");
        }
    }
}
