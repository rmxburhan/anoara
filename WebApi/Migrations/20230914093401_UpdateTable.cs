using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Teachers",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Teachers",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Students",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Students",
                type: "longtext",
                nullable: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Milestones",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "Attendances",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Announcements",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Announcements",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_ClassId",
                table: "Attendances",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_TeacherId",
                table: "Announcements",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Teachers_TeacherId",
                table: "Announcements",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Classes_ClassId",
                table: "Attendances",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Teachers_TeacherId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Classes_ClassId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_ClassId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_TeacherId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Announcements");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "UpdatedAt",
                table: "Milestones",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);
        }
    }
}
