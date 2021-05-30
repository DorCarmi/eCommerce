using Microsoft.EntityFrameworkCore.Migrations;

namespace eCommerce.Migrations
{
    public partial class createSchoolDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Classroom",
                columns: table => new
                {
                    ClassroomId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classroom", x => x.ClassroomId);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StudentName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                });

            migrationBuilder.CreateTable(
                name: "Pairs",
                columns: table => new
                {
                    ClassroomId = table.Column<int>(type: "INTEGER", nullable: false),
                    StudentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pairs", x => new { x.StudentId, x.ClassroomId });
                    table.ForeignKey(
                        name: "FK_Pairs_Classroom_ClassroomId",
                        column: x => x.ClassroomId,
                        principalTable: "Classroom",
                        principalColumn: "ClassroomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pairs_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CourseName = table.Column<string>(type: "TEXT", nullable: true),
                    MyPairClassroomCourseClassroomId = table.Column<int>(name: "MyPair<Classroom, Course>ClassroomId", type: "INTEGER", nullable: true),
                    MyPairClassroomCourseStudentId = table.Column<int>(name: "MyPair<Classroom, Course>StudentId", type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Courses_Pairs_MyPair<Classroom, Course>StudentId_MyPair<Classroom, Course>ClassroomId",
                        columns: x => new { x.MyPairClassroomCourseStudentId, x.MyPairClassroomCourseClassroomId },
                        principalTable: "Pairs",
                        principalColumns: new[] { "StudentId", "ClassroomId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_MyPair<Classroom, Course>StudentId_MyPair<Classroom, Course>ClassroomId",
                table: "Courses",
                columns: new[] { "MyPair<Classroom, Course>StudentId", "MyPair<Classroom, Course>ClassroomId" });

            migrationBuilder.CreateIndex(
                name: "IX_Pairs_ClassroomId",
                table: "Pairs",
                column: "ClassroomId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Pairs");

            migrationBuilder.DropTable(
                name: "Classroom");

            migrationBuilder.DropTable(
                name: "Students");
        }
    }
}
