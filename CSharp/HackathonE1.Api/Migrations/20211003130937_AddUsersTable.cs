using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace HackathonE1.Api.Migrations
{
    public partial class AddUsersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.CreateTable(
				name: "Users",
				columns: table => new
				{
					Id = table.Column<int>( type: "integer", nullable: false )
						.Annotation( "Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn ),
					Identifier = table.Column<string>( type: "character varying(24)", maxLength: 24, nullable: false ),
					FirstName = table.Column<string>( type: "character varying(32)", maxLength: 32, nullable: false ),
					LastName = table.Column<string>( type: "character varying(32)", maxLength: 32, nullable: false ),
					Email = table.Column<string>( type: "character varying(32)", maxLength: 32, nullable: false ),
					PasswordHash = table.Column<string>( type: "character varying(88)", maxLength: 88, nullable: false )
				},
				constraints: table =>
				{
					_ = table.PrimaryKey( "PK_Users", x => x.Id );
				} );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Users_Email",
				table: "Users",
				column: "Email",
				unique: true );

			_ = migrationBuilder.CreateIndex(
				name: "IX_Users_Identifier",
				table: "Users",
				column: "Identifier",
				unique: true );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			_ = migrationBuilder.DropTable(
				name: "Users" );
        }
    }
}
