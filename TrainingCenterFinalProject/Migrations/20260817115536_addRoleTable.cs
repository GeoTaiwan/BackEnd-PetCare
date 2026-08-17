using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenterFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class addRoleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "tinyint", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_Authorization",
                table: "UserProfiles",
                column: "Authorization");
            //string roleName = user.Authorization switch
            //{
            //    0 => "Member",
            //    1 => "Employee",
            //    2 => "Manager",
            //    9 => "Admin",
            //    _ => "Member" // 預設防呆
            //};
            migrationBuilder.Sql("INSERT INTO [Role] (Id, RoleName) VALUES (0, 'Member')");
            migrationBuilder.Sql("INSERT INTO [Role] (Id, RoleName) VALUES (1, 'Employee')");
            migrationBuilder.Sql("INSERT INTO [Role] (Id, RoleName) VALUES (2, 'Manager')");
            migrationBuilder.Sql("INSERT INTO [Role] (Id, RoleName) VALUES (9, 'Admin')");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Role_Authorization",
                table: "UserProfiles",
                column: "Authorization",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Role_Authorization",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_Authorization",
                table: "UserProfiles");
        }
    }
}
