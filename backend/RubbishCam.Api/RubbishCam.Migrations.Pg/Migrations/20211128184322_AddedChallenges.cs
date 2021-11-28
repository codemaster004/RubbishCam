using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
    public partial class AddedChallenges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.CreateTable(
				name: "Challenges",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Name = table.Column<string>( type: "text", nullable: false ),
					Description = table.Column<string>( type: "text", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Challenges", x => x.Id );
				} );

			_ = migrationBuilder.CreateTable(
				name: "ChallengeRequirements",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Name = table.Column<string>( type: "text", nullable: false ),
					ChallengeModelId = table.Column<int>( type: "integer", nullable: true ),
					Discriminator = table.Column<string>( type: "text", nullable: false ),
					GarbageTypeId = table.Column<int>( type: "integer", nullable: true ),
					RequiredAmount = table.Column<int>( type: "integer", nullable: true )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_ChallengeRequirements", x => x.Id );
					_ = table.ForeignKey(
						name: "FK_ChallengeRequirements_Challenges_ChallengeModelId",
						column: x => x.ChallengeModelId,
						principalTable: "Challenges",
						principalColumn: "Id" );
					_ = table.ForeignKey(
						name: "FK_ChallengeRequirements_GarbageTypes_GarbageTypeId",
						column: x => x.GarbageTypeId,
						principalTable: "GarbageTypes",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateTable(
				name: "UsersChallenges",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					UserUuid = table.Column<string>( type: "character varying(24)", nullable: false ),
					ChallengeId = table.Column<int>( type: "integer", nullable: false ),
					DateStarted = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: false ),
					DateCompleted = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: true )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_UsersChallenges", x => x.Id );
					_ = table.ForeignKey(
						name: "FK_UsersChallenges_Challenges_ChallengeId",
						column: x => x.ChallengeId,
						principalTable: "Challenges",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
					_ = table.ForeignKey(
						name: "FK_UsersChallenges_Users_UserUuid",
						column: x => x.UserUuid,
						principalTable: "Users",
						principalColumn: "Uuid",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateTable(
				name: "UsersChallengesPoints",
				columns: table => new
				{
					PointId = table.Column<int>( type: "integer", nullable: false ),
					UserChallengeId = table.Column<int>( type: "integer", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_UsersChallengesPoints", x => new { x.PointId, x.UserChallengeId } );
					_ = table.ForeignKey(
						name: "FK_UsersChallengesPoints_Points_PointId",
						column: x => x.PointId,
						principalTable: "Points",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
					_ = table.ForeignKey(
						name: "FK_UsersChallengesPoints_UsersChallenges_UserChallengeId",
						column: x => x.UserChallengeId,
						principalTable: "UsersChallenges",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_ChallengeRequirements_ChallengeModelId",
				table: "ChallengeRequirements",
				column: "ChallengeModelId" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_ChallengeRequirements_GarbageTypeId",
				table: "ChallengeRequirements",
				column: "GarbageTypeId" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_UsersChallenges_ChallengeId",
				table: "UsersChallenges",
				column: "ChallengeId" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_UsersChallenges_UserUuid",
				table: "UsersChallenges",
				column: "UserUuid" );

			_ = migrationBuilder.CreateIndex(
				name: "IX_UsersChallengesPoints_UserChallengeId",
				table: "UsersChallengesPoints",
				column: "UserChallengeId" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "ChallengeRequirements" );

			_ = migrationBuilder.DropTable(
				name: "UsersChallengesPoints" );

			_ = migrationBuilder.DropTable(
				name: "UsersChallenges" );

			_ = migrationBuilder.DropTable(
				name: "Challenges" );
        }
    }
}
