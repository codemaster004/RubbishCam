using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
    public partial class NamingUnification1_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropForeignKey(
				name: "FK_GroupPointsRelation_Groups_GroupId",
				table: "GroupPointsRelation" );

			_ = migrationBuilder.DropForeignKey(
				name: "FK_GroupPointsRelation_Points_PointId",
				table: "GroupPointsRelation" );

			_ = migrationBuilder.DropForeignKey(
				name: "FK_GroupsMembers_Groups_GroupId",
				table: "GroupsMembers" );

			_ = migrationBuilder.DropForeignKey(
				name: "FK_GroupsMembers_Users_UserUuid",
				table: "GroupsMembers" );

			_ = migrationBuilder.DropPrimaryKey(
				name: "PK_GroupsMembers",
				table: "GroupsMembers" );

			_ = migrationBuilder.DropPrimaryKey(
				name: "PK_GroupPointsRelation",
				table: "GroupPointsRelation" );

			_ = migrationBuilder.RenameTable(
				name: "GroupsMembers",
				newName: "GroupsMemberships" );

			_ = migrationBuilder.RenameTable(
				name: "GroupPointsRelation",
				newName: "GroupsPoints" );

			_ = migrationBuilder.RenameIndex(
				name: "IX_GroupsMembers_UserUuid",
				table: "GroupsMemberships",
				newName: "IX_GroupsMemberships_UserUuid" );

			_ = migrationBuilder.RenameIndex(
				name: "IX_GroupPointsRelation_PointId",
				table: "GroupsPoints",
				newName: "IX_GroupsPoints_PointId" );

			_ = migrationBuilder.AddPrimaryKey(
				name: "PK_GroupsMemberships",
				table: "GroupsMemberships",
				columns: new[] { "GroupId", "UserUuid" } );

			_ = migrationBuilder.AddPrimaryKey(
				name: "PK_GroupsPoints",
				table: "GroupsPoints",
				columns: new[] { "GroupId", "PointId" } );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_GroupsMemberships_Groups_GroupId",
				table: "GroupsMemberships",
				column: "GroupId",
				principalTable: "Groups",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_GroupsMemberships_Users_UserUuid",
				table: "GroupsMemberships",
				column: "UserUuid",
				principalTable: "Users",
				principalColumn: "Uuid",
				onDelete: ReferentialAction.Cascade );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_GroupsPoints_Groups_GroupId",
				table: "GroupsPoints",
				column: "GroupId",
				principalTable: "Groups",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_GroupsPoints_Points_PointId",
				table: "GroupsPoints",
				column: "PointId",
				principalTable: "Points",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropForeignKey(
				name: "FK_GroupsMemberships_Groups_GroupId",
				table: "GroupsMemberships" );

			_ = migrationBuilder.DropForeignKey(
				name: "FK_GroupsMemberships_Users_UserUuid",
				table: "GroupsMemberships" );

			_ = migrationBuilder.DropForeignKey(
				name: "FK_GroupsPoints_Groups_GroupId",
				table: "GroupsPoints" );

			_ = migrationBuilder.DropForeignKey(
				name: "FK_GroupsPoints_Points_PointId",
				table: "GroupsPoints" );

			_ = migrationBuilder.DropPrimaryKey(
				name: "PK_GroupsPoints",
				table: "GroupsPoints" );

			_ = migrationBuilder.DropPrimaryKey(
				name: "PK_GroupsMemberships",
				table: "GroupsMemberships" );

			_ = migrationBuilder.RenameTable(
				name: "GroupsPoints",
				newName: "GroupPointsRelation" );

			_ = migrationBuilder.RenameTable(
				name: "GroupsMemberships",
				newName: "GroupsMembers" );

			_ = migrationBuilder.RenameIndex(
				name: "IX_GroupsPoints_PointId",
				table: "GroupPointsRelation",
				newName: "IX_GroupPointsRelation_PointId" );

			_ = migrationBuilder.RenameIndex(
				name: "IX_GroupsMemberships_UserUuid",
				table: "GroupsMembers",
				newName: "IX_GroupsMembers_UserUuid" );

			_ = migrationBuilder.AddPrimaryKey(
				name: "PK_GroupPointsRelation",
				table: "GroupPointsRelation",
				columns: new[] { "GroupId", "PointId" } );

			_ = migrationBuilder.AddPrimaryKey(
				name: "PK_GroupsMembers",
				table: "GroupsMembers",
				columns: new[] { "GroupId", "UserUuid" } );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_GroupPointsRelation_Groups_GroupId",
				table: "GroupPointsRelation",
				column: "GroupId",
				principalTable: "Groups",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_GroupPointsRelation_Points_PointId",
				table: "GroupPointsRelation",
				column: "PointId",
				principalTable: "Points",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_GroupsMembers_Groups_GroupId",
				table: "GroupsMembers",
				column: "GroupId",
				principalTable: "Groups",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_GroupsMembers_Users_UserUuid",
				table: "GroupsMembers",
				column: "UserUuid",
				principalTable: "Users",
				principalColumn: "Uuid",
				onDelete: ReferentialAction.Cascade );
        }
    }
}
