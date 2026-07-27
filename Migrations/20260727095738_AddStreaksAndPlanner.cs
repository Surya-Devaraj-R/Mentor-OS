using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentorOS.Migrations
{
    /// <inheritdoc />
    public partial class AddStreaksAndPlanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyPlanItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlanDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    EntityKind = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    EntityId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsDone = table.Column<bool>(type: "INTEGER", nullable: false),
                    DoneUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPlanItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StreakDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActivityDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ActivityCount = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreakDays", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPlanItems_PlanDate",
                table: "DailyPlanItems",
                column: "PlanDate");

            migrationBuilder.CreateIndex(
                name: "IX_StreakDays_ActivityDate",
                table: "StreakDays",
                column: "ActivityDate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPlanItems");

            migrationBuilder.DropTable(
                name: "StreakDays");
        }
    }
}
