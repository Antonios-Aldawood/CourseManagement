using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations.CourseManagement
{
    /// <inheritdoc />
    public partial class FixedLooseDbListOfIdsInJobsAndCoursesForSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobSkillsIds",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CoursesSkillsIds",
                table: "Courses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobSkillsIds",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoursesSkillsIds",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
