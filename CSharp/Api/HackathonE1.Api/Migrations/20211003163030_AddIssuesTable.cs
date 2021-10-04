using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace HackathonE1.Api.Migrations
{
    public partial class AddIssuesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.CreateTable(
				name: "IssueTypes",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Name = table.Column<string>( type: "character varying(24)", maxLength: 24, nullable: false ),
					Description = table.Column<string>( type: "character varying(256)", maxLength: 256, nullable: true )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_IssueTypes", x => x.Id );
				} );

			_ = migrationBuilder.CreateTable(
				name: "Issues",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Latitude = table.Column<double>( type: "double precision", nullable: false ),
					Longitude = table.Column<double>( type: "double precision", nullable: false ),
					TypeId = table.Column<int>( type: "integer", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Issues", x => x.Id );
					_ = table.ForeignKey(
						name: "FK_Issues_IssueTypes_TypeId",
						column: x => x.TypeId,
						principalTable: "IssueTypes",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Issues_TypeId",
				table: "Issues",
				column: "TypeId" );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "Issues" );

			_ = migrationBuilder.DropTable(
				name: "IssueTypes" );
        }
    }
}
