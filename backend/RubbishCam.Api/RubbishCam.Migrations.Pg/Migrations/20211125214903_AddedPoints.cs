using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
    public partial class AddedPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.CreateTable(
				name: "Points",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Longitude = table.Column<double>( type: "double precision", nullable: false ),
					Latitude = table.Column<double>( type: "double precision", nullable: false ),
					Type = table.Column<string>( type: "character varying(50)", maxLength: 50, nullable: false ),
					Value = table.Column<int>( type: "integer", nullable: false ),
					DateScored = table.Column<DateTimeOffset>( type: "timestamp with time zone", nullable: false ),
					UserUuid = table.Column<string>( type: "character varying(24)", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Points", x => x.Id );
					_ = table.ForeignKey(
						name: "FK_Points_Users_UserUuid",
						column: x => x.UserUuid,
						principalTable: "Users",
						principalColumn: "Uuid",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Points_UserUuid",
				table: "Points",
				column: "UserUuid" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "Points" );
        }
    }
}
