using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrainingCenterFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class AddPetPhotoColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePhotoUrl",
                table: "Pets",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            //migrationBuilder.InsertData(
            //    table: "SysMenus",
            //    columns: new[] { "MenuId", "ActionName", "ControllerName", "IconClass", "IsVisible", "MenuName", "ParentId", "RequiredAuthLevel", "SortOrder" },
            //    values: new object[,]
            //    {
            //        { 1, "Index", "Home", null, true, "首頁", null, (byte)0, 1 },
            //        { 2, "Index", "User", null, true, "使用者列表", null, (byte)1, 2 },
            //        { 3, "Index", "MedicalRecord", null, true, "寵物病歷管理", null, (byte)2, 3 }
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "SysMenus",
            //    keyColumn: "MenuId",
            //    keyValue: 1);

            //migrationBuilder.DeleteData(
            //    table: "SysMenus",
            //    keyColumn: "MenuId",
            //    keyValue: 2);

            //migrationBuilder.DeleteData(
            //    table: "SysMenus",
            //    keyColumn: "MenuId",
            //    keyValue: 3);

            migrationBuilder.DropColumn(
                name: "ProfilePhotoUrl",
                table: "Pets");
        }
    }
}
