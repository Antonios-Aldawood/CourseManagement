using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations.CourseManagement
{
    /// <inheritdoc />
    public partial class UserPositionFixedDbName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Position",
                table: "Users",
                newName: "position");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobSkillsIds",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CoursesSkillsIds",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "Users",
                newName: "Position");
        }
    }
}
