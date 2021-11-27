using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
    public partial class AddedGarbageTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropColumn(
				name: "Type",
				table: "Points" );

			_ = migrationBuilder.RenameColumn(
				name: "Value",
				table: "Points",
				newName: "GarbageTypeId" );

			_ = migrationBuilder.CreateTable(
				name: "GarbageTypes",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					PointsPerItem = table.Column<int>( type: "integer", nullable: false ),
					Name = table.Column<string>( type: "character varying(32)", maxLength: 32, nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_GarbageTypes", x => x.Id );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Points_GarbageTypeId",
				table: "Points",
				column: "GarbageTypeId" );

			_ = migrationBuilder.AddForeignKey(
				name: "FK_Points_GarbageTypes_GarbageTypeId",
				table: "Points",
				column: "GarbageTypeId",
				principalTable: "GarbageTypes",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropForeignKey(
				name: "FK_Points_GarbageTypes_GarbageTypeId",
				table: "Points" );

			_ = migrationBuilder.DropTable(
				name: "GarbageTypes" );

			_ = migrationBuilder.DropIndex(
				name: "IX_Points_GarbageTypeId",
				table: "Points" );

			_ = migrationBuilder.RenameColumn(
				name: "GarbageTypeId",
				table: "Points",
				newName: "Value" );

			_ = migrationBuilder.AddColumn<string>(
				name: "Type",
				table: "Points",
				type: "character varying(50)",
				maxLength: 50,
				nullable: false,
				defaultValue: "" );
        }
    }
}
