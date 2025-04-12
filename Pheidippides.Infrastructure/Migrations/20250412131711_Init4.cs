using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pheidippides.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IncidentId1",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IncidentId1",
                table: "Users",
                column: "IncidentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Incidents_IncidentId1",
                table: "Users",
                column: "IncidentId1",
                principalTable: "Incidents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Incidents_IncidentId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IncidentId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IncidentId1",
                table: "Users");
        }
    }
}
