using Microsoft.EntityFrameworkCore;
using RubbishCam.Domain.Models;

namespace RubbishCam.Data.Repositories;

public interface IUsersRepository : IRepository
{
	IQueryable<UserModel> GetUsers();
	Task AddUserAsync( UserModel user );
	Task RemoveUserAsync( UserModel user );
	Task<int> SaveAsync();

	IQueryable<UserModel> WithRoles( IQueryable<UserModel> source );
	IQueryable<UserModel> WithTokens( IQueryable<UserModel> source );
	IQueryable<UserModel> WithFriendships( IQueryable<UserModel> source );
	IQueryable<UserModel> WithFriends( IQueryable<UserModel> source );

}

public class UsersRepository : IUsersRepository
{
	private readonly AppDbContext _dbContext;

	public UsersRepository( AppDbContext dbContext )
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

}

public static class UsersRepositoryExtensions
{
	public static IQueryable<UserModel> FilterById( this IQueryable<UserModel> source, string uuid )
	{
		return source.Where( u => u.Uuid == uuid );
	}
	public static IQueryable<UserModel> FilterByUsername( this IQueryable<UserModel> source, string username )
	{
		return source.Where( u => u.UserName == username );
	}

	public static IQueryable<UserModel> WithRoles( this IQueryable<UserModel> source, IUsersRepository repository )
	{
		return repository.WithRoles( source );
	}
	public static IQueryable<UserModel> WithTokens( this IQueryable<UserModel> source, IUsersRepository repository )
	{
		return repository.WithTokens( source );
	}
	public static IQueryable<UserModel> WithFriendships( this IQueryable<UserModel> source, IUsersRepository repository )
	{
		return repository.WithFriendships( source );
	}
	public static IQueryable<UserModel> WithFriends( this IQueryable<UserModel> source, IUsersRepository repository )
	{
		return repository.WithFriends( source );
	}

}