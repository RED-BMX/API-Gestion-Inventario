using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_Gestion_Inventario.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryMovementConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "InventoryMovements",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_InventoryMovement_Quantity",
                table: "InventoryMovements",
                sql: "\"Quantity\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_InventoryMovement_Quantity",
                table: "InventoryMovements");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "InventoryMovements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
