using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RubbishCam.Data;
using RubbishCam.Domain.Dtos.User;
using RubbishCam.Domain.Models;
using System.Security.Cryptography;
using System.Text;

namespace RubbishCam.Api.Services;

public interface IUsersService
{
	Task<GetUserDto[]> GetUsersAsync();
	Task<GetUserDetailsDto?> GetUserAsync( string uuid );
	Task<GetUserDto> CreateUserAsync( CreateUserDto user );
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

	public Task<GetUserDto[]> GetUsersAsync()
	{
		return _dbContext.Users
			.Select( GetUserDto.FromUserExp )
			.ToArrayAsync();
	}

	public Task<GetUserDetailsDto?> GetUserAsync( string uuid )
	{
		if ( uuid is null )
		{
			throw new ArgumentNullException( nameof( uuid ) );
		}

		return _dbContext.Users
			.Where( u => u.Uuid == uuid )
			.Select( GetUserDetailsDto.FromUserExp )
			.FirstOrDefaultAsync();
	}

	public async Task<GetUserDto> CreateUserAsync( CreateUserDto dto )
	{
		if ( dto is null )
		{
			throw new ArgumentNullException( nameof( dto ) );
		}

		var user = await dto.ToUserAsync( AuthService.HashPasswordAsync, GenerateUuid );

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

		return GetUserDto.FromUser( user );
	}

	public async Task DeleteUserAsync( string uuid )
	{
		if ( uuid is null )
		{
			throw new ArgumentNullException( nameof( uuid ) );
		}

		var user = await _dbContext.Users
			.Where( u => u.Uuid == uuid )
			.FirstOrDefaultAsync();

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

		} while ( await _dbContext.Users.AnyAsync( u => u.Uuid == encoded ) );

		return encoded;
	}

}
