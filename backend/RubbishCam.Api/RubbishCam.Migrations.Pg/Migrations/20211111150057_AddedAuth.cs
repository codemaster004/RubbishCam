using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
    public partial class AddedAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.AddColumn<string>(
				name: "UserName",
				table: "Users",
				type: "character varying(32)",
				maxLength: 32,
				nullable: false,
				defaultValue: "" );

			_ = migrationBuilder.AddUniqueConstraint(
				name: "AK_Users_Uuid",
				table: "Users",
				column: "Uuid" );

			_ = migrationBuilder.CreateTable(
				name: "Roles",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Name = table.Column<string>( type: "character varying(24)", maxLength: 24, nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Roles", x => x.Id );
				} );

			_ = migrationBuilder.CreateTable(
				name: "Tokens",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Token = table.Column<string>( type: "text", nullable: false ),
					UserUuid = table.Column<string>( type: "character varying(24)", nullable: false ),
					ValidUntil = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Tokens", x => x.Id );
					_ = table.ForeignKey(
						name: "FK_Tokens_Users_UserUuid",
						column: x => x.UserUuid,
						principalTable: "Users",
						principalColumn: "Uuid",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateTable(
				name: "RoleModelUserModel",
				columns: table => new
				{
					RolesId = table.Column<int>( type: "integer", nullable: false ),
					UsersId = table.Column<int>( type: "integer", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_RoleModelUserModel", x => new { x.RolesId, x.UsersId } );
					_ = table.ForeignKey(
						name: "FK_RoleModelUserModel_Roles_RolesId",
						column: x => x.RolesId,
						principalTable: "Roles",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
					_ = table.ForeignKey(
						name: "FK_RoleModelUserModel_Users_UsersId",
						column: x => x.UsersId,
						principalTable: "Users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Users_UserName",
				table: "Users",
				column: "UserName",
				unique: true );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Users_Uuid",
				table: "Users",
				column: "Uuid",
				unique: true );

			_ = migrationBuilder.CreateIndex(
				name: "IX_RoleModelUserModel_UsersId",
				table: "RoleModelUserModel",
				column: "UsersId" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Tokens_UserUuid",
				table: "Tokens",
				column: "UserUuid" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "RoleModelUserModel" );

			_ = migrationBuilder.DropTable(
				name: "Tokens" );

			_ = migrationBuilder.DropTable(
				name: "Roles" );

			_ = migrationBuilder.DropUniqueConstraint(
				name: "AK_Users_Uuid",
				table: "Users" );

			_ = migrationBuilder.DropIndex(
				name: "IX_Users_UserName",
				table: "Users" );

			_ = migrationBuilder.DropIndex(
				name: "IX_Users_Uuid",
				table: "Users" );

			_ = migrationBuilder.DropColumn(
				name: "UserName",
				table: "Users" );
        }
    }
}
