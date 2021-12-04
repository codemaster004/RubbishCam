using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Dtos.User;

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
	private readonly IUsersRepository _userRepo;
	private readonly ILogger<UsersService> _logger;
	public UsersService( IUsersRepository userRepo, ILogger<UsersService> logger )
	{
		_userRepo = userRepo ?? throw new ArgumentNullException( nameof( userRepo ) );
		_logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
	}

	public Task<GetUserDto[]> GetUsersAsync()
	{
		return _userRepo.GetUsers()
			.Select( GetUserDto.FromUserExp )
			.ToArrayAsync( _userRepo );
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
			.FirstOrDefaultAsync( _userRepo );
	}

	public async Task<GetUserDto> CreateUserAsync( CreateUserDto dto )
	{
		if ( dto is null )
		{
			throw new ArgumentNullException( nameof( dto ) );
		}

		var exists = await _userRepo.GetUsers()
			.FilterByUsername( dto.UserName )
			.AnyAsync( _userRepo );
		if ( exists )
		{
			throw new ConflictException();
		}

		var user = await dto.ToUserAsync( AuthService.HashPasswordAsync, GenerateUuid );

		await _userRepo.AddUserAsync( user );

		try
		{
			_ = await _userRepo.SaveAsync();
		}
		catch ( DbUpdateException e )
		{
			// todo: do something here
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
			.FirstOrDefaultAsync( _userRepo );

		if ( user is null )
		{
			throw new NotFoundException();
		}

		await _userRepo.RemoveUserAsync( user );


		try
		{
			_ = await _userRepo.SaveAsync();
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

		} while ( await _userRepo.GetUsers().FilterById( encoded ).AnyAsync( _userRepo ) );

		return encoded;
	}

}
