using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Debts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Remainder",
                table: "Debts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "OptimizationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptimizationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptimizationRequests_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OptimizationRequests_Users_InitiatorId",
                        column: x => x.InitiatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OptimizationRequestApprovals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OptimizationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Approved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OptimizationRequestApprovals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OptimizationRequestApprovals_OptimizationRequests_OptimizationRequestId",
                        column: x => x.OptimizationRequestId,
                        principalTable: "OptimizationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OptimizationRequestApprovals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_OptimizationRequestApprovals_OptimizationRequestId",
                table: "OptimizationRequestApprovals",
                column: "OptimizationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_OptimizationRequestApprovals_UserId",
                table: "OptimizationRequestApprovals",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OptimizationRequests_GroupId",
                table: "OptimizationRequests",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_OptimizationRequests_InitiatorId",
                table: "OptimizationRequests",
                column: "InitiatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OptimizationRequestApprovals");

            migrationBuilder.DropTable(
                name: "OptimizationRequests");

            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "Remainder",
                table: "Debts");
        }
    }
}
