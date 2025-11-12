using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations.CourseManagement
{
    /// <inheritdoc />
    public partial class FixedEnrollmentsTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersCourses_Courses_course_id",
                table: "UsersCourses");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersCourses_Users_user_id",
                table: "UsersCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersCourses",
                table: "UsersCourses");

            migrationBuilder.RenameTable(
                name: "UsersCourses",
                newName: "Enrollments");

            migrationBuilder.RenameIndex(
                name: "IX_UsersCourses_user_id",
                table: "Enrollments",
                newName: "IX_Enrollments_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_UsersCourses_course_id",
                table: "Enrollments",
                newName: "IX_Enrollments_course_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments",
                column: "Id")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_course_id",
                table: "Enrollments",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Users_user_id",
                table: "Enrollments",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_course_id",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Users_user_id",
                table: "Enrollments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments");

            migrationBuilder.RenameTable(
                name: "Enrollments",
                newName: "UsersCourses");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_user_id",
                table: "UsersCourses",
                newName: "IX_UsersCourses_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_course_id",
                table: "UsersCourses",
                newName: "IX_UsersCourses_course_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersCourses",
                table: "UsersCourses",
                column: "Id")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersCourses_Courses_course_id",
                table: "UsersCourses",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersCourses_Users_user_id",
                table: "UsersCourses",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
