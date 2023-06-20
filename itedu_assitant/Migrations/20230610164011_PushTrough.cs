using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace itedu_assitant.Migrations
{
    /// <inheritdoc />
    public partial class PushTrough : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CurrentActive_UserNumbers_IsActiveid",
                table: "CurrentActive");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserGroups_userGroupid",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserNumbers",
                table: "UserNumbers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserInstance",
                table: "UserInstance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CurrentActive",
                table: "CurrentActive");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "ContUsers");

            migrationBuilder.RenameTable(
                name: "UserNumbers",
                newName: "ContUserNumbers");

            migrationBuilder.RenameTable(
                name: "UserInstance",
                newName: "ContUserInstance");

            migrationBuilder.RenameTable(
                name: "UserGroups",
                newName: "ContUserGroups");

            migrationBuilder.RenameTable(
                name: "CurrentActive",
                newName: "ContCurrentActive");

            migrationBuilder.RenameIndex(
                name: "IX_Users_userGroupid",
                table: "ContUsers",
                newName: "IX_ContUsers_userGroupid");

            migrationBuilder.RenameIndex(
                name: "IX_CurrentActive_IsActiveid",
                table: "ContCurrentActive",
                newName: "IX_ContCurrentActive_IsActiveid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContUsers",
                table: "ContUsers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContUserNumbers",
                table: "ContUserNumbers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContUserInstance",
                table: "ContUserInstance",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContUserGroups",
                table: "ContUserGroups",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContCurrentActive",
                table: "ContCurrentActive",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContCurrentActive_ContUserNumbers_IsActiveid",
                table: "ContCurrentActive",
                column: "IsActiveid",
                principalTable: "ContUserNumbers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ContUsers_ContUserGroups_userGroupid",
                table: "ContUsers",
                column: "userGroupid",
                principalTable: "ContUserGroups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContCurrentActive_ContUserNumbers_IsActiveid",
                table: "ContCurrentActive");

            migrationBuilder.DropForeignKey(
                name: "FK_ContUsers_ContUserGroups_userGroupid",
                table: "ContUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContUsers",
                table: "ContUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContUserNumbers",
                table: "ContUserNumbers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContUserInstance",
                table: "ContUserInstance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContUserGroups",
                table: "ContUserGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContCurrentActive",
                table: "ContCurrentActive");

            migrationBuilder.RenameTable(
                name: "ContUsers",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "ContUserNumbers",
                newName: "UserNumbers");

            migrationBuilder.RenameTable(
                name: "ContUserInstance",
                newName: "UserInstance");

            migrationBuilder.RenameTable(
                name: "ContUserGroups",
                newName: "UserGroups");

            migrationBuilder.RenameTable(
                name: "ContCurrentActive",
                newName: "CurrentActive");

            migrationBuilder.RenameIndex(
                name: "IX_ContUsers_userGroupid",
                table: "Users",
                newName: "IX_Users_userGroupid");

            migrationBuilder.RenameIndex(
                name: "IX_ContCurrentActive_IsActiveid",
                table: "CurrentActive",
                newName: "IX_CurrentActive_IsActiveid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserNumbers",
                table: "UserNumbers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserInstance",
                table: "UserInstance",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserGroups",
                table: "UserGroups",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CurrentActive",
                table: "CurrentActive",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_CurrentActive_UserNumbers_IsActiveid",
                table: "CurrentActive",
                column: "IsActiveid",
                principalTable: "UserNumbers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserGroups_userGroupid",
                table: "Users",
                column: "userGroupid",
                principalTable: "UserGroups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
