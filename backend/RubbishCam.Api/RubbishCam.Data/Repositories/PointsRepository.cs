using RubbishCam.Domain.Models;

namespace RubbishCam.Data.Repositories;

public interface IPointsRepository : IRepository
{
	IQueryable<PointModel> GetPoints();
	Task AddPointAsync( PointModel point );
	Task<int> SaveAsync();
}

public class PointsRepository : IPointsRepository
{
	private readonly AppDbContext _dbContext;

	public PointsRepository( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	public IQueryable<PointModel> GetPoints()
	{
		return _dbContext.Points;
	}

	public async Task AddPointAsync( PointModel point )
	{
		_ = await _dbContext.AddAsync( point );
	}

	public Task<int> SaveAsync()
	{
		return _dbContext.SaveChangesAsync();
	}
}

public static class PointsRepositoryExtensions
{
	public static IQueryable<PointModel> FilterByUserUuid( this IQueryable<PointModel> source, string uuid )
	{
		return source.Where( t => t.UserUuid == uuid );
	}
}
