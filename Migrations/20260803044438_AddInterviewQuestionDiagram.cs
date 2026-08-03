using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentorOS.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewQuestionDiagram : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiagramBody",
                table: "InterviewQuestions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiagramFormat",
                table: "InterviewQuestions",
                type: "TEXT",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiagramBody",
                table: "InterviewQuestions");

            migrationBuilder.DropColumn(
                name: "DiagramFormat",
                table: "InterviewQuestions");
        }
    }
}
