using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CATEGORIES",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    IMAGE_URL = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CATEGORIES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PRODUCTS",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NAME = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    DESCRIPTION = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    PRICE = table.Column<decimal>(type: "SMALLMONEY", nullable: false),
                    IMAGE_URL = table.Column<string>(type: "NVARCHAR(MAX)", nullable: false),
                    STOCK = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    REGISTER_DATE = table.Column<DateTime>(type: "SMALLDATETIME", nullable: false),
                    CATEGORY_ID = table.Column<int>(type: "INT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUCTS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CATEGORY_ID",
                        column: x => x.CATEGORY_ID,
                        principalTable: "CATEGORIES",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CATEGORY_ID",
                table: "PRODUCTS",
                column: "CATEGORY_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRODUCTS");

            migrationBuilder.DropTable(
                name: "CATEGORIES");
        }
    }
}
