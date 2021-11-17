using Microsoft.EntityFrameworkCore;
using RubbishCam.Data;
using RubbishCam.Domain.Models;

namespace RubbishCam.Api.Repositories;

public interface IUserRepository
{
	IQueryable<UserModel> GetUsers();
	Task AddUserAsync( UserModel user );
	Task RemoveUserAsync( UserModel user );
	Task<int> SaveAsync();

	IQueryable<UserModel> WithRoles( IQueryable<UserModel> source );
	IQueryable<UserModel> WithTokens( IQueryable<UserModel> source );
	IQueryable<UserModel> WithFriendships( IQueryable<UserModel> source );
	IQueryable<UserModel> WithFriends( IQueryable<UserModel> source );
	Task<T?> FirstOrDefaultAsync<T>( IQueryable<T> source );
	Task<T[]> ToArrayAsync<T>( IQueryable<T> source );
	Task<bool> AnyAsync<T>( IQueryable<T> source );

}

public class UserRepository : IUserRepository
{
	private readonly AppDbContext _dbContext;

	public UserRepository( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}


	public IQueryable<UserModel> GetUsers()
	{
		return _dbContext.Users;
	}

	public async Task AddUserAsync( UserModel user )
	{
		_ = await _dbContext.Users.AddAsync( user );
	}

	public async Task RemoveUserAsync( UserModel user )
	{
		await Task.CompletedTask;
		_ = _dbContext.Users.Remove( user );
	}

	public async Task<int> SaveAsync()
	{
		return await _dbContext.SaveChangesAsync();
	}

	public IQueryable<UserModel> WithRoles( IQueryable<UserModel> source )
	{
		return source.Include( u => u.Roles );
	}
	public IQueryable<UserModel> WithTokens( IQueryable<UserModel> source )
	{
		return source.Include( u => u.Tokens );
	}
	public IQueryable<UserModel> WithFriendships( IQueryable<UserModel> source )
	{
		return source.Include( u => u.InitiatedFriendships )
			.Include( u => u.TargetingFriendships );
	}
	public IQueryable<UserModel> WithFriends( IQueryable<UserModel> source )
	{
		return source.Include( u => u.InitiatedFriends )
			.Include( u => u.TargetingFriends );
	}
	public Task<T?> FirstOrDefaultAsync<T>( IQueryable<T> source )
	{
		return source.FirstOrDefaultAsync();
	}
	public Task<T[]> ToArrayAsync<T>( IQueryable<T> source )
	{
		return source.ToArrayAsync();
	}
	public Task<bool> AnyAsync<T>( IQueryable<T> source )
	{
		return source.AnyAsync();
	}

}

public static class UserRepositoryExtensions
{
	public static IQueryable<UserModel> FilterById( this IQueryable<UserModel> source, string uuid )
	{
		return source.Where( u => u.Uuid == uuid );
	}

	public static IQueryable<UserModel> WithRoles( this IQueryable<UserModel> source, IUserRepository repository )
	{
		return repository.WithRoles( source );
	}
	public static IQueryable<UserModel> WithTokens( this IQueryable<UserModel> source, IUserRepository repository )
	{
		return repository.WithTokens( source );
	}
	public static IQueryable<UserModel> WithFriendships( this IQueryable<UserModel> source, IUserRepository repository )
	{
		return repository.WithFriendships( source );
	}
	public static IQueryable<UserModel> WithFriends( this IQueryable<UserModel> source, IUserRepository repository )
	{
		return repository.WithFriends( source );
	}
	public static Task<T?> FirstOrDefaultAsync<T>( this IQueryable<T> source, IUserRepository repository )
	{
		return repository.FirstOrDefaultAsync( source );
	}
	public static Task<T[]> ToArrayAsync<T>( this IQueryable<T> source, IUserRepository repository )
	{
		return repository.ToArrayAsync( source );
	}
	public static Task<bool> AnyAsync<T>( this IQueryable<T> source, IUserRepository repository )
	{
		return repository.AnyAsync( source );
	}

}