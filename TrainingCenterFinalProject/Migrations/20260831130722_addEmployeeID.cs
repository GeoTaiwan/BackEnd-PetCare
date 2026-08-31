using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenterFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class addEmployeeID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeID",
                table: "Appointments",
                type: "nchar(13)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_EmployeeID",
                table: "Appointments",
                column: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_UserProfiles_EmployeeID",
                table: "Appointments",
                column: "EmployeeID",
                principalTable: "UserProfiles",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_UserProfiles_EmployeeID",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_EmployeeID",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "EmployeeID",
                table: "Appointments");
        }
    }
}
