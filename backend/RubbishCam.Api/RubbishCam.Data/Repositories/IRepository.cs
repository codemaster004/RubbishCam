using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
	async Task<int> CountAsync<T>( IQueryable<T> source )
	{
		if ( source is IAsyncEnumerable<T> )
		{
			return await source.CountAsync();
		}
		return source.Count();
	}
	async Task<int> SumAsync<T>( IQueryable<T> source, Expression<Func<T, int>> selector )
	{
		if ( source is IAsyncEnumerable<T> )
		{
			return await source.SumAsync( selector );
		}
		return source.Sum( selector );
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
	public static Task<int> CountAsync<T>( this IQueryable<T> source, IRepository repository )
	{
		return repository.CountAsync( source );
	}
	public static Task<int> SumAsync<T>( this IQueryable<T> source, IRepository repository, Expression<Func<T, int>> selector )
	{
		return repository.SumAsync( source, selector );
	}
}
