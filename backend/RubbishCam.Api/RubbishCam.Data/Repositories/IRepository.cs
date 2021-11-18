using Microsoft.EntityFrameworkCore;

namespace RubbishCam.Data.Repositories;

public interface IRepository
{
	async Task<T?> FirstOrDefaultAsync<T>( IQueryable<T> source )
	{
		if ( source is IAsyncEnumerable<T> )
		{
			return await source.FirstOrDefaultAsync();
		}
		return source.FirstOrDefault();
	}
	async Task<T[]> ToArrayAsync<T>( IQueryable<T> source )
	{
		if ( source is IAsyncEnumerable<T> )
		{
			return await source.ToArrayAsync();
		}
		return source.ToArray();
	}
	async Task<bool> AnyAsync<T>( IQueryable<T> source )
	{
		if ( source is IAsyncEnumerable<T> )
		{
			return await source.AnyAsync();
		}
		return source.Any();
	}
}

public static class RepositoryExtensions
{
	public static Task<T?> FirstOrDefaultAsync<T>( this IQueryable<T> source, IRepository repository )
	{
		return repository.FirstOrDefaultAsync( source );
	}
	public static Task<T[]> ToArrayAsync<T>( this IQueryable<T> source, IRepository repository )
	{
		return repository.ToArrayAsync( source );
	}
	public static Task<bool> AnyAsync<T>( this IQueryable<T> source, IRepository repository )
	{
		return repository.AnyAsync( source );
	}
}
