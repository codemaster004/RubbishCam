using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
    public partial class AddedGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.CreateTable(
				name: "Groups",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Name = table.Column<string>( type: "text", nullable: false ),
					TimeCreated = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Groups", x => x.Id );
				} );

			_ = migrationBuilder.CreateTable(
				name: "GroupPointsRelation",
				columns: table => new
				{
					GroupId = table.Column<int>( type: "integer", nullable: false ),
					PointId = table.Column<int>( type: "integer", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_GroupPointsRelation", x => new { x.GroupId, x.PointId } );
					_ = table.ForeignKey(
						name: "FK_GroupPointsRelation_Groups_GroupId",
						column: x => x.GroupId,
						principalTable: "Groups",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
					_ = table.ForeignKey(
						name: "FK_GroupPointsRelation_Points_PointId",
						column: x => x.PointId,
						principalTable: "Points",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateTable(
				name: "GroupsMembers",
				columns: table => new
				{
					UserUuid = table.Column<string>( type: "character varying(24)", nullable: false ),
					GroupId = table.Column<int>( type: "integer", nullable: false ),
					IsOwner = table.Column<bool>( type: "boolean", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_GroupsMembers", x => new { x.GroupId, x.UserUuid } );
					_ = table.ForeignKey(
						name: "FK_GroupsMembers_Groups_GroupId",
						column: x => x.GroupId,
						principalTable: "Groups",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
					_ = table.ForeignKey(
						name: "FK_GroupsMembers_Users_UserUuid",
						column: x => x.UserUuid,
						principalTable: "Users",
						principalColumn: "Uuid",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_GroupPointsRelation_PointId",
				table: "GroupPointsRelation",
				column: "PointId" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_GroupsMembers_UserUuid",
				table: "GroupsMembers",
				column: "UserUuid" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "GroupPointsRelation" );

			_ = migrationBuilder.DropTable(
				name: "GroupsMembers" );

			_ = migrationBuilder.DropTable(
				name: "Groups" );
        }
    }
}
