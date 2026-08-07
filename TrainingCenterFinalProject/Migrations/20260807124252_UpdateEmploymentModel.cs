using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenterFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmploymentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthLevel",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "Occupation",
                table: "Employments");

            migrationBuilder.AddColumn<string>(
                name: "OccupationID",
                table: "Employments",
                type: "nchar(3)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Occupations",
                columns: table => new
                {
                    OccupationID = table.Column<string>(type: "nchar(3)", nullable: false),
                    OccupationName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Occupations", x => x.OccupationID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employments_OccupationID",
                table: "Employments",
                column: "OccupationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Employments_Occupations_OccupationID",
                table: "Employments",
                column: "OccupationID",
                principalTable: "Occupations",
                principalColumn: "OccupationID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employments_Occupations_OccupationID",
                table: "Employments");

            migrationBuilder.DropTable(
                name: "Occupations");

            migrationBuilder.DropIndex(
                name: "IX_Employments_OccupationID",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "OccupationID",
                table: "Employments");

            migrationBuilder.AddColumn<string>(
                name: "AuthLevel",
                table: "Employments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Occupation",
                table: "Employments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
