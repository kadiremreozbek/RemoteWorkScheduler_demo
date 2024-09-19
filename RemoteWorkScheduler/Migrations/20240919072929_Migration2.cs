using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RemoteWorkScheduler.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_teams",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_remote_logs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_employees",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "t_teams",
                type: "TEXT",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "t_teams",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "t_remote_logs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "t_employees",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "t_teams",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 500,
                oldNullable: true);
        }
    }
}
