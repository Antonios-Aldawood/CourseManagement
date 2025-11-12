using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseManagement.Infrastructure.Migrations.CourseManagement
{
    /// <inheritdoc />
    public partial class RemovedTrainersAndMadeFixingAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Sessions_SessionId",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Materials_trainer_id",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Trainers_trainer_id",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "Trainers");

            migrationBuilder.RenameColumn(
                name: "user_is_course_teacher",
                table: "UsersCourses",
                newName: "is_completed");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Materials",
                newName: "session_id");

            migrationBuilder.RenameIndex(
                name: "IX_Materials_SessionId",
                table: "Materials",
                newName: "IX_Materials_session_id");

            migrationBuilder.RenameColumn(
                name: "all",
                table: "Courses",
                newName: "is_for_all");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Sessions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "session_id",
                table: "Materials",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_name",
                table: "Sessions",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Sessions_session_id",
                table: "Materials",
                column: "session_id",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Users_trainer_id",
                table: "Sessions",
                column: "trainer_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Materials_Sessions_session_id",
                table: "Materials");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Users_trainer_id",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_name",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "name",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "is_completed",
                table: "UsersCourses",
                newName: "user_is_course_teacher");

            migrationBuilder.RenameColumn(
                name: "session_id",
                table: "Materials",
                newName: "SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Materials_session_id",
                table: "Materials",
                newName: "IX_Materials_SessionId");

            migrationBuilder.RenameColumn(
                name: "is_for_all",
                table: "Courses",
                newName: "all");

            migrationBuilder.AlterColumn<int>(
                name: "SessionId",
                table: "Materials",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "Trainers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone_number = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainers", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trainers_email",
                table: "Trainers",
                column: "email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Materials_Sessions_SessionId",
                table: "Materials",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Materials_trainer_id",
                table: "Sessions",
                column: "trainer_id",
                principalTable: "Materials",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Trainers_trainer_id",
                table: "Sessions",
                column: "trainer_id",
                principalTable: "Trainers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
