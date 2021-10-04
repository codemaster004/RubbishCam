using Microsoft.EntityFrameworkCore.Migrations;

namespace HackathonE1.Api.Migrations
{
    public partial class ChangeAreaForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropForeignKey(
				name: "FK_ObservedAreas_Users_UserId",
				table: "ObservedAreas" );

			_ = migrationBuilder.DropIndex(
				name: "IX_ObservedAreas_UserId",
				table: "ObservedAreas" );

			_ = migrationBuilder.DropColumn(
				name: "UserId",
				table: "ObservedAreas" );

			_ = migrationBuilder.AddColumn<string>(
				name: "UserIdentifier",
				table: "ObservedAreas",
				type: "character varying(24)",
				nullable: false,
				defaultValue: "" );

			_ = migrationBuilder.AddUniqueConstraint(
				name: "AK_Users_Identifier",
				table: "Users",
				column: "Identifier" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_ObservedAreas_UserIdentifier",
				table: "ObservedAreas",
				column: "UserIdentifier" );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_ObservedAreas_Users_UserIdentifier",
				table: "ObservedAreas",
				column: "UserIdentifier",
				principalTable: "Users",
				principalColumn: "Identifier",
				onDelete: ReferentialAction.Cascade );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropForeignKey(
				name: "FK_ObservedAreas_Users_UserIdentifier",
				table: "ObservedAreas" );

			_ = migrationBuilder.DropUniqueConstraint(
				name: "AK_Users_Identifier",
				table: "Users" );

			_ = migrationBuilder.DropIndex(
				name: "IX_ObservedAreas_UserIdentifier",
				table: "ObservedAreas" );

			_ = migrationBuilder.DropColumn(
				name: "UserIdentifier",
				table: "ObservedAreas" );

			_ = migrationBuilder.AddColumn<int>(
				name: "UserId",
				table: "ObservedAreas",
				type: "integer",
				nullable: false,
				defaultValue: 0 );

			_ = migrationBuilder.CreateIndex(
				name: "IX_ObservedAreas_UserId",
				table: "ObservedAreas",
				column: "UserId" );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_ObservedAreas_Users_UserId",
				table: "ObservedAreas",
				column: "UserId",
				principalTable: "Users",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade );
        }
    }
}
