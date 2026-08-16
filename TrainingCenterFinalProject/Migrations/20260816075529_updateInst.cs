using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenterFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class updateInst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Institutions");

            migrationBuilder.AddColumn<string>(
                name: "StatusId",
                table: "Institutions",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeId",
                table: "Institutions",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InstitutionStatuses",
                columns: table => new
                {
                    StatusId = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitutionStatuses", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "InstitutionTypes",
                columns: table => new
                {
                    TypeId = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstitutionTypes", x => x.TypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_StatusId",
                table: "Institutions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_TypeId",
                table: "Institutions",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Institutions_InstitutionStatuses_StatusId",
                table: "Institutions",
                column: "StatusId",
                principalTable: "InstitutionStatuses",
                principalColumn: "StatusId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Institutions_InstitutionTypes_TypeId",
                table: "Institutions",
                column: "TypeId",
                principalTable: "InstitutionTypes",
                principalColumn: "TypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Institutions_InstitutionStatuses_StatusId",
                table: "Institutions");

            migrationBuilder.DropForeignKey(
                name: "FK_Institutions_InstitutionTypes_TypeId",
                table: "Institutions");

            migrationBuilder.DropTable(
                name: "InstitutionStatuses");

            migrationBuilder.DropTable(
                name: "InstitutionTypes");

            migrationBuilder.DropIndex(
                name: "IX_Institutions_StatusId",
                table: "Institutions");

            migrationBuilder.DropIndex(
                name: "IX_Institutions_TypeId",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Institutions");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Institutions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Institutions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
