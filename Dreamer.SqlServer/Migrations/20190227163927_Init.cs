using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dreamer.SqlServer.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Main");

            migrationBuilder.EnsureSchema(
                name: "Security");

            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateSequence<int>(
                name: "OrdersSerialNumber",
                schema: "shared",
                startValue: 10000L);

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "Main",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsValid = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderShipments",
                schema: "Main",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsValid = table.Column<bool>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    ShippedTime = table.Column<DateTimeOffset>(nullable: true),
                    DeliveryTime = table.Column<DateTimeOffset>(nullable: true),
                    ShipmentCost = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderShipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "Main",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsValid = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    DiscountPercentage = table.Column<int>(nullable: false),
                    FreeDelivary = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DreamerUsers",
                schema: "Security",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsValid = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Role = table.Column<int>(nullable: false),
                    LastActivityDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DreamerUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                schema: "Main",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsValid = table.Column<bool>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    CategoryId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Main",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductCategories_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Main",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                schema: "Main",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsValid = table.Column<bool>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    DreamerUserId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_DreamerUsers_DreamerUserId",
                        column: x => x.DreamerUserId,
                        principalSchema: "Security",
                        principalTable: "DreamerUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Main",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "Main",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsValid = table.Column<bool>(nullable: false),
                    PaymentDateTime = table.Column<DateTimeOffset>(nullable: false),
                    NetItemsPrice = table.Column<double>(nullable: false),
                    NetItemsDiscountValue = table.Column<double>(nullable: false),
                    Total = table.Column<double>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    SerialNumber = table.Column<int>(nullable: false, defaultValueSql: "NEXT VALUE FOR shared.OrdersSerialNumber"),
                    EmailNotified = table.Column<bool>(nullable: false),
                    FreeDelivery = table.Column<bool>(nullable: false),
                    DreamerUserId = table.Column<Guid>(nullable: false),
                    OrderShipmentSetId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_DreamerUsers_DreamerUserId",
                        column: x => x.DreamerUserId,
                        principalSchema: "Security",
                        principalTable: "DreamerUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_OrderShipments_OrderShipmentSetId",
                        column: x => x.OrderShipmentSetId,
                        principalSchema: "Main",
                        principalTable: "OrderShipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderProducts",
                schema: "Main",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsValid = table.Column<bool>(nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    OneItemPrice = table.Column<double>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    DiscountPercentage = table.Column<int>(nullable: false),
                    DiscountValue = table.Column<double>(nullable: false),
                    NetPrice = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProducts_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "Main",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Main",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_DreamerUserId",
                schema: "Main",
                table: "CartItems",
                column: "DreamerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                schema: "Main",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_OrderId",
                schema: "Main",
                table: "OrderProducts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProducts_ProductId",
                schema: "Main",
                table: "OrderProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DreamerUserId",
                schema: "Main",
                table: "Orders",
                column: "DreamerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderShipmentSetId",
                schema: "Main",
                table: "Orders",
                column: "OrderShipmentSetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_CategoryId",
                schema: "Main",
                table: "ProductCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategories_ProductId",
                schema: "Main",
                table: "ProductCategories",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DreamerUsers_Email",
                schema: "Security",
                table: "DreamerUsers",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems",
                schema: "Main");

            migrationBuilder.DropTable(
                name: "OrderProducts",
                schema: "Main");

            migrationBuilder.DropTable(
                name: "ProductCategories",
                schema: "Main");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "Main");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "Main");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "Main");

            migrationBuilder.DropTable(
                name: "DreamerUsers",
                schema: "Security");

            migrationBuilder.DropTable(
                name: "OrderShipments",
                schema: "Main");

            migrationBuilder.DropSequence(
                name: "OrdersSerialNumber",
                schema: "shared");
        }
    }
}
