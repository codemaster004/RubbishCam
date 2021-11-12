using Microsoft.EntityFrameworkCore;
using RubbishCam.Domain.Models;

namespace RubbishCam.Data;
public class AppDbContext : DbContext
{
	public AppDbContext( DbContextOptions options ) 
		: base( options )
	{
		if ( Users is null )
		{
			throw new NullReferenceException();
		}
		if ( Tokens is null )
		{
			throw new NullReferenceException();
		}
		if ( Roles is null )
		{
			throw new NullReferenceException();
		}

	}

	protected override void OnModelCreating( ModelBuilder modelBuilder )
	{
		_ = modelBuilder.Entity<TokenModel>()
				.HasOne( t => t.User )
				.WithMany( u => u.Tokens )
				.HasForeignKey( t => t.UserUuid )
				.HasPrincipalKey( u => u.Uuid );
	}

	public DbSet<UserModel> Users { get; set; }

	public DbSet<TokenModel> Tokens { get; set; }

	public DbSet<RoleModel> Roles { get; set; }
}
