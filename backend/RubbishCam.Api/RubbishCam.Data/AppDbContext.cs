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

	}

	public DbSet<UserModel> Users { get; set; }
}
