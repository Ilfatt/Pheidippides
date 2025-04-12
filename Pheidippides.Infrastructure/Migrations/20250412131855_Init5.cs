using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pheidippides.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Incidents_IncidentId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Incidents_IncidentId1",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IncidentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IncidentId1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IncidentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IncidentId1",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "IncidentUser",
                columns: table => new
                {
                    AcknowledgedUsersId = table.Column<long>(type: "bigint", nullable: false),
                    AcknowledgedUsersIncidentsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentUser", x => new { x.AcknowledgedUsersId, x.AcknowledgedUsersIncidentsId });
                    table.ForeignKey(
                        name: "FK_IncidentUser_Incidents_AcknowledgedUsersIncidentsId",
                        column: x => x.AcknowledgedUsersIncidentsId,
                        principalTable: "Incidents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncidentUser_Users_AcknowledgedUsersId",
                        column: x => x.AcknowledgedUsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncidentUser_AcknowledgedUsersIncidentsId",
                table: "IncidentUser",
                column: "AcknowledgedUsersIncidentsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncidentUser");

            migrationBuilder.AddColumn<long>(
                name: "IncidentId",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "IncidentId1",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IncidentId",
                table: "Users",
                column: "IncidentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IncidentId1",
                table: "Users",
                column: "IncidentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Incidents_IncidentId",
                table: "Users",
                column: "IncidentId",
                principalTable: "Incidents",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Incidents_IncidentId1",
                table: "Users",
                column: "IncidentId1",
                principalTable: "Incidents",
                principalColumn: "Id");
        }
    }
}
