using HackathonE1.Domain.Models;
using HackathonE1.Domain.RelationModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext( DbContextOptions<AppDbContext> options )
			: base( options )
		{
		}

		public DbSet<UserModel> Users { get; set; }
		public DbSet<IssueModel> Issues { get; set; }
		public DbSet<IssueTypeModel> IssueTypes { get; set; }
		public DbSet<ObservedAreaModel> ObservedAreas { get; set; }

		protected override void OnModelCreating( ModelBuilder modelBuilder )
		{
			_ = modelBuilder.Entity<ObservedAreaModel>()
				.HasOne( oa => oa.User )
				.WithMany( u => u.ObservedAreas )
				.HasForeignKey( oa => oa.UserIdentifier )
				.HasPrincipalKey( u => u.Identifier );

			_ = modelBuilder.Entity<UserModel>()
				.HasMany( u => u.Roles )
				.WithMany( r => r.Owners )
				.UsingEntity<RoleUserRelation>(
					j => j
						 .HasOne( ru => ru.Role )
						 .WithMany( r => r.RoleUsers )
						 .HasForeignKey( ru => ru.RoleId ),
					j => j
						 .HasOne( ru => ru.User )
						 .WithMany( u => u.RoleUsers )
						 .HasForeignKey( ru => ru.UserId ),
					j => j
						 .HasKey( ru => new { ru.UserId, ru.RoleId } )
				);

			_ = modelBuilder.Entity<UserModel>()
				.Property( u=>u.ReciveEmails )
				.HasDefaultValue( true );

		}

	}
}
