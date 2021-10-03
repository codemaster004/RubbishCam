using HackathonE1.Domain.Models;
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

	}
}
