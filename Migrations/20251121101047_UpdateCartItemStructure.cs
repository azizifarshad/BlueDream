using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlueDream.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartItemStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Carts_CartsId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Items_ItemsId",
                table: "CartItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_ItemsId",
                table: "CartItem");

            migrationBuilder.RenameColumn(
                name: "ItemsId",
                table: "CartItem",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "CartsId",
                table: "CartItem",
                newName: "ItemId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CartItem",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CartId",
                table: "CartItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "CartItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "CartItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CartId",
                table: "CartItem",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ItemId",
                table: "CartItem",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Carts_CartId",
                table: "CartItem",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Items_ItemId",
                table: "CartItem",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Carts_CartId",
                table: "CartItem");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Items_ItemId",
                table: "CartItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_CartId",
                table: "CartItem");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_ItemId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "CartItem");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "CartItem");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "CartItem",
                newName: "ItemsId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "CartItem",
                newName: "CartsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItem",
                table: "CartItem",
                columns: new[] { "CartsId", "ItemsId" });

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ItemsId",
                table: "CartItem",
                column: "ItemsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Carts_CartsId",
                table: "CartItem",
                column: "CartsId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Items_ItemsId",
                table: "CartItem",
                column: "ItemsId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
