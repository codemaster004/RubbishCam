using Microsoft.EntityFrameworkCore;
using RubbishCam.Domain.Models;
using RubbishCam.Domain.Relations;

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
		ArgumentNullException.ThrowIfNull( Groups );
		ArgumentNullException.ThrowIfNull( GroupsMembers );
	}

	protected override void OnModelCreating( ModelBuilder modelBuilder )
	{
		// user <= token
		_ = modelBuilder.Entity<UserModel>()
			.HasMany( u => u.Tokens )
			.WithOne( t => t.User )
			.HasForeignKey( t => t.UserUuid )
			.HasPrincipalKey( u => u.Uuid );

		// user <= friendships => friends
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

		// user <= _ => groups
		_ = modelBuilder.Entity<UserModel>()
			.HasMany( u => u.Groups )
			.WithMany( g => g.Members )
			.UsingEntity<GroupMembersRelation>(
			j =>
			{
				return j.HasOne( gm => gm.Group )
						.WithMany( g => g.MembersR )
						.HasForeignKey( gm => gm.GroupId )
						.HasPrincipalKey( g => g.Id );
			},
			j =>
			{
				return j.HasOne( gm => gm.User )
						.WithMany( u => u.GroupsR )
						.HasForeignKey( gm => gm.UserUuid )
						.HasPrincipalKey( u => u.Uuid );
			} );

		//// user <= _ => owned groups
		//_ = modelBuilder.Entity<UserModel>()
		//	.HasMany( u => u.OwnedGroups )
		//	.WithMany( g => g.Owners )
		//	.UsingEntity<GroupOwnersRelation>(
		//	j =>
		//	{
		//		return j.HasOne( go => go.Group )
		//				.WithMany( g => g.OwnersR )
		//				.HasForeignKey( go => go.GroupId )
		//				.HasPrincipalKey( g => g.Id );
		//	},
		//	j =>
		//	{
		//		return j.HasOne( go => go.User )
		//				.WithMany( u => u.OwnedGroupsR )
		//				.HasForeignKey( go => go.UserUuid )
		//				.HasPrincipalKey( u => u.Uuid );
		//	} );

		// user <= point
		_ = modelBuilder.Entity<UserModel>()
			.HasMany( u => u.Points )
			.WithOne( p => p.User )
			.HasForeignKey( p => p.UserUuid )
			.HasPrincipalKey( u => u.Uuid );

		// group <= _ => points
		_ = modelBuilder.Entity<GroupModel>()
			.HasMany( g => g.Points )
			.WithMany( p => p.Groups )
			.UsingEntity<GroupPointsRelation>(
			j =>
			{
				return j.HasOne( gp => gp.Point)
						.WithMany( p => p.GroupsR )
						.HasForeignKey( gp => gp.PointId )
						.HasPrincipalKey( p => p.Id );
			},
			j =>
			{
				return j.HasOne( gp => gp.Group )
						.WithMany( g => g.PointsR )
						.HasForeignKey( gp => gp.GroupId )
						.HasPrincipalKey( g => g.Id );
			} );

	}

	public DbSet<UserModel> Users { get; set; }
	public DbSet<TokenModel> Tokens { get; set; }
	public DbSet<RoleModel> Roles { get; set; }
	public DbSet<FriendshipModel> Friendships { get; set; }
	public DbSet<PointModel> Points { get; set; }
	public DbSet<GroupModel> Groups { get; set; }
	public DbSet<GroupMembersRelation> GroupsMembers { get; set; }
}
