using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace HackathonE1.Api.Migrations
{
    public partial class AddObservedAreasTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.CreateTable(
				name: "ObservedAreas",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Latitude = table.Column<double>( type: "double precision", nullable: false ),
					Longitude = table.Column<double>( type: "double precision", nullable: false ),
					Radius = table.Column<double>( type: "double precision", nullable: false ),
					UserId = table.Column<int>( type: "integer", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_ObservedAreas", x => x.Id );
					_ = table.ForeignKey(
						name: "FK_ObservedAreas_Users_UserId",
						column: x => x.UserId,
						principalTable: "Users",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_ObservedAreas_UserId",
				table: "ObservedAreas",
				column: "UserId" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "ObservedAreas" );
        }
    }
}
