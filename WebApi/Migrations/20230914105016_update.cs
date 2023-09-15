using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Milestones_Students_StudentId",
                table: "Milestones");

            migrationBuilder.DropIndex(
                name: "IX_Milestones_StudentId",
                table: "Milestones");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Milestones");

            migrationBuilder.CreateTable(
                name: "MilestoneStudent",
                columns: table => new
                {
                    MilestonesId = table.Column<Guid>(type: "char(36)", nullable: false),
                    StudentsId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MilestoneStudent", x => new { x.MilestonesId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_MilestoneStudent_Milestones_MilestonesId",
                        column: x => x.MilestonesId,
                        principalTable: "Milestones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MilestoneStudent_Students_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_MilestoneStudent_StudentsId",
                table: "MilestoneStudent",
                column: "StudentsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MilestoneStudent");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "Milestones",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_StudentId",
                table: "Milestones",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Milestones_Students_StudentId",
                table: "Milestones",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
