using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RubbishCam.Data;
using RubbishCam.Domain.Models;

namespace RubbishCam.Api.Services;

public interface IUsersService
{
	Task<UserModel[]> GetUsersAsync();
	Task<UserModel?> GetUserAsync( string uuid );
	Task<UserModel> CreateUserAsync( UserModel user );
	Task DeleteUserAsync( string uuid );
}

public class UsersService : IUsersService
{
	private readonly AppDbContext _dbContext;
	private readonly ILogger<UsersService> _logger;
	public UsersService( AppDbContext dbContext, ILogger<UsersService> logger )
	{
		_dbContext = dbContext ?? throw new ArgumentNullException( nameof( dbContext ) );
		_logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
	}

	public Task<UserModel[]> GetUsersAsync()
	{
		return _dbContext.Users.ToArrayAsync();
	}

	public Task<UserModel?> GetUserAsync( string uuid )
	{
		if ( uuid is null )
		{
			throw new ArgumentNullException( nameof( uuid ) );
		}

		return _dbContext.Users
			.Where( u => u.Uuid == uuid )
			.FirstOrDefaultAsync();
	}

	public async Task<UserModel> CreateUserAsync( UserModel user )
	{
		if ( user is null )
		{
			throw new ArgumentNullException( nameof( user ) );
		}

		user.Uuid = await GenerateUuid();

		_ = await _dbContext.Users.AddAsync( user );

		try
		{
			_ = await _dbContext.SaveChangesAsync();
		}
		catch ( DbUpdateException e )
		{
			_logger.LogError( e, "Unexpected error." );
			throw;
		}

		return user;
	}

	public async Task DeleteUserAsync( string uuid )
	{
		if ( uuid is null )
		{
			throw new ArgumentNullException( nameof( uuid ) );
		}

		var user = await GetUserAsync( uuid );
		if ( user is null )
		{
			throw new NotFoundException();
		}

		_ = _dbContext.Users.Remove( user );

		try
		{
			_ = await _dbContext.SaveChangesAsync();
		}
		catch ( DbUpdateException e )
		{
			_logger.LogError( e, "Unexpected error." );
			throw;
		}

	}

	private async Task<string> GenerateUuid()
	{
		string encoded;
		do
		{
			var guid = Guid.NewGuid();
			encoded = Base64UrlTextEncoder.Encode( guid.ToByteArray() );

		} while ( ( await GetUserAsync( encoded ) ) is not null );

		return encoded;
	}


}
