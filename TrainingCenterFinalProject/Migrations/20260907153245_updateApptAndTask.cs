using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrainingCenterFinalProject.Migrations
{
    /// <inheritdoc />
    public partial class updateApptAndTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppointmentID",
                table: "TaskSchedules",
                type: "nchar(20)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RevokedDate",
                table: "AuthorizationRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskSchedules_AppointmentID",
                table: "TaskSchedules",
                column: "AppointmentID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskSchedules_Appointments_AppointmentID",
                table: "TaskSchedules",
                column: "AppointmentID",
                principalTable: "Appointments",
                principalColumn: "AppointmentID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskSchedules_Appointments_AppointmentID",
                table: "TaskSchedules");

            migrationBuilder.DropIndex(
                name: "IX_TaskSchedules_AppointmentID",
                table: "TaskSchedules");

            migrationBuilder.DropColumn(
                name: "AppointmentID",
                table: "TaskSchedules");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RevokedDate",
                table: "AuthorizationRecords",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
