using Microsoft.EntityFrameworkCore;
using RubbishCam.Domain.Models;

namespace RubbishCam.Data.Repositories;

public interface IPointsRepository : IRepository
{
	IQueryable<PointModel> GetPoints();
	Task AddPointAsync( PointModel point );
	Task<int> SaveAsync();

	IQueryable<PointModel> WithTypes( IQueryable<PointModel> source );
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

	public IQueryable<PointModel> WithTypes( IQueryable<PointModel> source )
	{
		return source.Include( x => x.GarbageType );
	}
}

public static class PointsRepositoryExtensions
{
	public static IQueryable<PointModel> FilterByUserUuid( this IQueryable<PointModel> source, string uuid )
	{
		return source.Where( t => t.UserUuid == uuid );
	}
	public static IQueryable<PointModel> WithTypes( this IQueryable<PointModel> source, IPointsRepository repository )
	{
		return repository.WithTypes( source );
	}
}
