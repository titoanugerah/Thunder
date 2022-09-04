using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Thunder.Migrations
{
    public partial class ConnectUnivandAccre : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accreditation",
                table: "University");

            migrationBuilder.DropColumn(
                name: "CuriculumFile",
                table: "University");

            migrationBuilder.AddColumn<int>(
                name: "AccreditationId",
                table: "University",
                type: "int",
                maxLength: 1,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_University_AccreditationId",
                table: "University",
                column: "AccreditationId");

            migrationBuilder.AddForeignKey(
                name: "FK_University_Accreditation_AccreditationId",
                table: "University",
                column: "AccreditationId",
                principalTable: "Accreditation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_University_Accreditation_AccreditationId",
                table: "University");

            migrationBuilder.DropIndex(
                name: "IX_University_AccreditationId",
                table: "University");

            migrationBuilder.DropColumn(
                name: "AccreditationId",
                table: "University");

            migrationBuilder.AddColumn<string>(
                name: "Accreditation",
                table: "University",
                type: "varchar(1)",
                maxLength: 1,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CuriculumFile",
                table: "University",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
