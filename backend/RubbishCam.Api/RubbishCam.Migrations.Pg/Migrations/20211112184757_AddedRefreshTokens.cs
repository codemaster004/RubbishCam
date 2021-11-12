using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
    public partial class AddedRefreshTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.AddColumn<string>(
				name: "RefreshToken",
				table: "Tokens",
				type: "text",
				nullable: false,
				defaultValue: "" );

			_ = migrationBuilder.AddColumn<bool>(
				name: "Revoked",
				table: "Tokens",
				type: "boolean",
				nullable: false,
				defaultValue: false );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropColumn(
				name: "RefreshToken",
				table: "Tokens" );

			_ = migrationBuilder.DropColumn(
				name: "Revoked",
				table: "Tokens" );
        }
    }
}
