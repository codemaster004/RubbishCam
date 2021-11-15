using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RubbishCam.Api.Repositories;
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
	//private readonly AppDbContext _dbContext;
	private readonly IUserRepository _userRepo;
	private readonly ILogger<UsersService> _logger;
	public UsersService( /*AppDbContext dbContext,*/ ILogger<UsersService> logger, IUserRepository userRepo )
	{
		//_dbContext = dbContext ?? throw new ArgumentNullException( nameof( dbContext ) );
		_logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
		_userRepo = userRepo;
	}

	public Task<GetUserDto[]> GetUsersAsync()
	{
		return _userRepo.GetUsers()
			.Select( GetUserDto.FromUserExp )
			.ToArrayAsync();

		//return _dbContext.Users
		//	.Select( GetUserDto.FromUserExp )
		//	.ToArrayAsync();
	}

	public Task<GetUserDetailsDto?> GetUserAsync( string uuid )
	{
		if ( uuid is null )
		{
			throw new ArgumentNullException( nameof( uuid ) );
		}

		return _userRepo.GetUsers()
			.FilterById( uuid )
			.Select( GetUserDetailsDto.FromUserExp )
			.FirstOrDefaultAsync();

		//return _dbContext.Users
		//	.Where( u => u.Uuid == uuid )
		//	.Select( GetUserDetailsDto.FromUserExp )
		//	.FirstOrDefaultAsync();
	}

	public async Task<GetUserDto> CreateUserAsync( CreateUserDto dto )
	{
		if ( dto is null )
		{
			throw new ArgumentNullException( nameof( dto ) );
		}

		var user = await dto.ToUserAsync( AuthService.HashPasswordAsync, GenerateUuid );

		user.Uuid = await GenerateUuid();

		await _userRepo.AddUserAsync( user );

		//_ = await _dbContext.Users.AddAsync( user );

		try
		{
			_ = await _userRepo.SaveAsync();
			//_ = await _dbContext.SaveChangesAsync();
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

		var user = await _userRepo.GetUsers()
			.FilterById( uuid )
			.FirstOrDefaultAsync();

		//var user = await _dbContext.Users
		//	.Where( u => u.Uuid == uuid )
		//	.FirstOrDefaultAsync();

		if ( user is null )
		{
			throw new NotFoundException();
		}

		await _userRepo.RemoveUserAsync( user );

		//_ = _dbContext.Users.Remove( user );

		try
		{
			_ = await _userRepo.SaveAsync();
			//_ = await _dbContext.SaveChangesAsync();
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

		} while ( await _userRepo.GetUsers().AnyAsync( u => u.Uuid == encoded ) );
		//} while ( await _dbContext.Users.AnyAsync( u => u.Uuid == encoded ) );

		return encoded;
	}

}
