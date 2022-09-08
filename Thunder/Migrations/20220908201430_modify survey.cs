using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thunder.Migrations
{
    public partial class modifysurvey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccreditationPriority",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "CityPriority",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "FacilityPriority",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "PricePriority",
                table: "Survey");

            migrationBuilder.AddColumn<string>(
                name: "AccreditationToCity",
                table: "Survey",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FacilityToAccreditation",
                table: "Survey",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FacilityToCity",
                table: "Survey",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FacilityToPrice",
                table: "Survey",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PriceToAccreditation",
                table: "Survey",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "PriceToCity",
                table: "Survey",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccreditationToCity",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "FacilityToAccreditation",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "FacilityToCity",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "FacilityToPrice",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "PriceToAccreditation",
                table: "Survey");

            migrationBuilder.DropColumn(
                name: "PriceToCity",
                table: "Survey");

            migrationBuilder.AddColumn<int>(
                name: "AccreditationPriority",
                table: "Survey",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CityPriority",
                table: "Survey",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FacilityPriority",
                table: "Survey",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PricePriority",
                table: "Survey",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
