using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RubbishCam.Migrations.Pg.Migrations
{
	public partial class AddedUsers : Migration
	{
		protected override void Up( MigrationBuilder migrationBuilder )
		{
			_ = migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Uuid = table.Column<string>( type: "character varying(24)", maxLength: 24, nullable: false ),
					FirstName = table.Column<string>( type: "character varying(50)", maxLength: 50, nullable: false ),
					LastName = table.Column<string>( type: "character varying(50)", maxLength: 50, nullable: false ),
					PasswordHash = table.Column<string>( type: "text", nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Users", x => x.Id );
				} );
		}

		protected override void Down( MigrationBuilder migrationBuilder )
		{
			_ = migrationBuilder.DropTable(
				name: "Users" );
		}
	}
}
