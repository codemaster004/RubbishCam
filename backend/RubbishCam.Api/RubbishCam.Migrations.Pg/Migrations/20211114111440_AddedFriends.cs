using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
    public partial class AddedFriends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.CreateTable(
				name: "FriendshipModel",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					InitiatorUuid = table.Column<string>( type: "character varying(24)", nullable: false ),
					TargetUuid = table.Column<string>( type: "character varying(24)", nullable: false ),
					Accepted = table.Column<bool>( type: "boolean", nullable: false ),
					Rejected = table.Column<bool>( type: "boolean", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_FriendshipModel", x => x.Id );
					_ = table.ForeignKey(
						name: "FK_FriendshipModel_Users_InitiatorUuid",
						column: x => x.InitiatorUuid,
						principalTable: "Users",
						principalColumn: "Uuid",
						onDelete: ReferentialAction.Cascade );
					_ = table.ForeignKey(
						name: "FK_FriendshipModel_Users_TargetUuid",
						column: x => x.TargetUuid,
						principalTable: "Users",
						principalColumn: "Uuid",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_FriendshipModel_InitiatorUuid",
				table: "FriendshipModel",
				column: "InitiatorUuid" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_FriendshipModel_TargetUuid",
				table: "FriendshipModel",
				column: "TargetUuid" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "FriendshipModel" );
        }
    }
}
