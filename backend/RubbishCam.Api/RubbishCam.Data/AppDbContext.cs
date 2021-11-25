using Microsoft.EntityFrameworkCore;
using RubbishCam.Domain.Models;

namespace RubbishCam.Data;
public class AppDbContext : DbContext
{
	public AppDbContext( DbContextOptions options )
		: base( options )
	{
		ArgumentNullException.ThrowIfNull( Users );
		ArgumentNullException.ThrowIfNull( Tokens );
		ArgumentNullException.ThrowIfNull( Roles );
		ArgumentNullException.ThrowIfNull( Friendships );
		ArgumentNullException.ThrowIfNull( Points );
	}

	protected override void OnModelCreating( ModelBuilder modelBuilder )
	{
		_ = modelBuilder.Entity<TokenModel>()
			.HasOne( t => t.User )
			.WithMany( u => u.Tokens )
			.HasForeignKey( t => t.UserUuid )
			.HasPrincipalKey( u => u.Uuid );

		_ = modelBuilder.Entity<UserModel>()
			.HasMany( u => u.InitiatedFriends )
			.WithMany( u => u.TargetingFriends )
			.UsingEntity<FriendshipModel>(
			j =>
			{
				return j.HasOne( f => f.Initiator )
						.WithMany( u => u.InitiatedFriendships )
						.HasForeignKey( f => f.InitiatorUuid )
						.HasPrincipalKey( u => u.Uuid );
			},
			j =>
			{
				return j.HasOne( f => f.Target )
						.WithMany( u => u.TargetingFriendships )
						.HasForeignKey( f => f.TargetUuid )
						.HasPrincipalKey( u => u.Uuid );
			} );

		_ = modelBuilder.Entity<PointModel>()
			.HasOne( p => p.User )
			.WithMany( u => u.Points )
			.HasForeignKey( p => p.UserUuid )
			.HasPrincipalKey( u => u.Uuid );

	}

	public DbSet<UserModel> Users { get; set; }
	public DbSet<TokenModel> Tokens { get; set; }
	public DbSet<RoleModel> Roles { get; set; }
	public DbSet<FriendshipModel> Friendships { get; set; }
	public DbSet<PointModel> Points { get; set; }
}
