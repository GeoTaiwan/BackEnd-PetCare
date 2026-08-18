using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenterFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class updateInst1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerID",
                table: "Institutions",
                type: "nchar(13)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_OwnerID",
                table: "Institutions",
                column: "OwnerID");

            migrationBuilder.AddForeignKey(
                name: "FK_Institutions_UserProfiles_OwnerID",
                table: "Institutions",
                column: "OwnerID",
                principalTable: "UserProfiles",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Institutions_UserProfiles_OwnerID",
                table: "Institutions");

            migrationBuilder.DropIndex(
                name: "IX_Institutions_OwnerID",
                table: "Institutions");

            migrationBuilder.DropColumn(
                name: "OwnerID",
                table: "Institutions");
        }
    }
}
