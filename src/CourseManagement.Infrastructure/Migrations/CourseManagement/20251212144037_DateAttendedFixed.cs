using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations.CourseManagement
{
    /// <inheritdoc />
    public partial class DateAttendedFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateAttended",
                table: "Attendances",
                newName: "date_attended");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date_attended",
                table: "Attendances",
                newName: "DateAttended");
        }
    }
}
