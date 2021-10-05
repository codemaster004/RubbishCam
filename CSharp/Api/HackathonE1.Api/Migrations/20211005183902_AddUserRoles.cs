using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace HackathonE1.Api.Migrations
{
    public partial class AddUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.AddColumn<bool>(
				name: "ReciveEmails",
				table: "Users",
				type: "boolean",
				nullable: false,
				defaultValue: true );

			_ = migrationBuilder.CreateTable(
				name: "RoleModel",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Name = table.Column<string>( type: "character varying(16)", maxLength: 16, nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_RoleModel", x => x.Id );
				} );

			_ = migrationBuilder.CreateTable(
				name: "RoleUserRelation",
				columns: table => new
				{
					UserId = table.Column<int>( type: "integer", nullable: false ),
					RoleId = table.Column<int>( type: "integer", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_RoleUserRelation", x => new { x.UserId, x.RoleId } );
					_ = table.ForeignKey(
						name: "FK_RoleUserRelation_RoleModel_RoleId",
						column: x => x.RoleId,
						principalTable: "RoleModel",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
					_ = table.ForeignKey(
						name: "FK_RoleUserRelation_Users_UserId",
						column: x => x.UserId,
						principalTable: "Users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_RoleUserRelation_RoleId",
				table: "RoleUserRelation",
				column: "RoleId" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "RoleUserRelation" );

			_ = migrationBuilder.DropTable(
				name: "RoleModel" );

			_ = migrationBuilder.DropColumn(
				name: "ReciveEmails",
				table: "Users" );
        }
    }
}
