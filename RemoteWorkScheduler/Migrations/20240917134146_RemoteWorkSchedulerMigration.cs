using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteWorkScheduler.Migrations
{
    /// <inheritdoc />
    public partial class RemoteWorkSchedulerMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Teams_TeamId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_RemoteLogs_Employees_EmployeeId",
                table: "RemoteLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teams",
                table: "Teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RemoteLogs",
                table: "RemoteLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.RenameTable(
                name: "Teams",
                newName: "t_teams");

            migrationBuilder.RenameTable(
                name: "RemoteLogs",
                newName: "t_remote_logs");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "t_employees");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "t_teams",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "t_teams",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "t_remote_logs",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "t_remote_logs",
                newName: "employee_id");

            migrationBuilder.RenameIndex(
                name: "IX_RemoteLogs_EmployeeId",
                table: "t_remote_logs",
                newName: "IX_t_remote_logs_employee_id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "t_employees",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "t_employees",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "t_employees",
                newName: "team_id");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_TeamId",
                table: "t_employees",
                newName: "IX_t_employees_team_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_teams",
                table: "t_teams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_remote_logs",
                table: "t_remote_logs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_employees",
                table: "t_employees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_employees_t_teams_team_id",
                table: "t_employees",
                column: "team_id",
                principalTable: "t_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_t_remote_logs_t_employees_employee_id",
                table: "t_remote_logs",
                column: "employee_id",
                principalTable: "t_employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_employees_t_teams_team_id",
                table: "t_employees");

            migrationBuilder.DropForeignKey(
                name: "FK_t_remote_logs_t_employees_employee_id",
                table: "t_remote_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_t_teams",
                table: "t_teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_t_remote_logs",
                table: "t_remote_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_t_employees",
                table: "t_employees");

            migrationBuilder.RenameTable(
                name: "t_teams",
                newName: "Teams");

            migrationBuilder.RenameTable(
                name: "t_remote_logs",
                newName: "RemoteLogs");

            migrationBuilder.RenameTable(
                name: "t_employees",
                newName: "Employees");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Teams",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Teams",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "RemoteLogs",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "employee_id",
                table: "RemoteLogs",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_t_remote_logs_employee_id",
                table: "RemoteLogs",
                newName: "IX_RemoteLogs_EmployeeId");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Employees",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Employees",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "team_id",
                table: "Employees",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_t_employees_team_id",
                table: "Employees",
                newName: "IX_Employees_TeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teams",
                table: "Teams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RemoteLogs",
                table: "RemoteLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Teams_TeamId",
                table: "Employees",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RemoteLogs_Employees_EmployeeId",
                table: "RemoteLogs",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
