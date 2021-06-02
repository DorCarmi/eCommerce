using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eCommerce.Migrations
{
    public partial class CreateTestDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Classroom",
                columns: table => new
                {
                    ClassroomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classroom", x => x.ClassroomId);
                });

            migrationBuilder.CreateTable(
                name: "MemberInfos",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ListPairs",
                columns: table => new
                {
                    KeyId = table.Column<int>(type: "int", nullable: false),
                    HolderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListPairs", x => new { x.HolderId, x.KeyId });
                    table.ForeignKey(
                        name: "FK_ListPairs_Classroom_KeyId",
                        column: x => x.KeyId,
                        principalTable: "Classroom",
                        principalColumn: "ClassroomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberInfoId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                    table.ForeignKey(
                        name: "FK_Users_MemberInfos_MemberInfoId",
                        column: x => x.MemberInfoId,
                        principalTable: "MemberInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ListPairClassroomCourseHolderId = table.Column<int>(name: "ListPair<Classroom, Course>HolderId", type: "int", nullable: true),
                    ListPairClassroomCourseKeyId = table.Column<int>(name: "ListPair<Classroom, Course>KeyId", type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Course_ListPairs_ListPair<Classroom, Course>HolderId_ListPair<Classroom, Course>KeyId",
                        columns: x => new { x.ListPairClassroomCourseHolderId, x.ListPairClassroomCourseKeyId },
                        principalTable: "ListPairs",
                        principalColumns: new[] { "HolderId", "KeyId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_ListPair<Classroom, Course>HolderId_ListPair<Classroom, Course>KeyId",
                table: "Course",
                columns: new[] { "ListPair<Classroom, Course>HolderId", "ListPair<Classroom, Course>KeyId" });

            migrationBuilder.CreateIndex(
                name: "IX_ListPairs_KeyId",
                table: "ListPairs",
                column: "KeyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_MemberInfoId",
                table: "Users",
                column: "MemberInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ListPairs");

            migrationBuilder.DropTable(
                name: "MemberInfos");

            migrationBuilder.DropTable(
                name: "Classroom");
        }
    }
}
