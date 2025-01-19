using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ActivityTracker.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalTrainingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CaloriesBurned",
                table: "Trainings",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DistanceCovered",
                table: "Trainings",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TopSpeed",
                table: "Trainings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesBurned",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "DistanceCovered",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "TopSpeed",
                table: "Trainings");
        }
    }
}
