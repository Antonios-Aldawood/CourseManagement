using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations.CourseManagement
{
    /// <inheritdoc />
    public partial class SessionPhyiscalAndOnlineManaged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "optional_completion",
                table: "Enrollments");

            migrationBuilder.AddColumn<string>(
                name: "app",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_offline",
                table: "Sessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "link",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "seats",
                table: "Sessions",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "app",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "is_offline",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "link",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "seats",
                table: "Sessions");

            migrationBuilder.AddColumn<int>(
                name: "optional_completion",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
