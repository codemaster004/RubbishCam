using Microsoft.EntityFrameworkCore;

namespace RubbishCam.Data;
public class AppDbContext : DbContext
{
	public AppDbContext( DbContextOptions options ) 
		: base( options )
	{
	}
}
