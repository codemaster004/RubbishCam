using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RubbishCam.Api.Exceptions.Auth;
using RubbishCam.Data;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Dtos.Token;
using RubbishCam.Domain.Models;
using System.Security.Cryptography;
using System.Text;

namespace RubbishCam.Api.Services;

public interface IAuthService
{
	Task<GetTokenDto> Login( string username, string password );
	Task<TokenModel?> GetTokenAsync( string token );
	Task<GetTokenDto> RefreshTokenAsync( string token );
	Task RevokeTokenAsync( string token );
	Task RevokeTokenAsync( TokenModel token );
}

public class AuthService : IAuthService
{
	private readonly ITokenRepository _tokenRepo;
	private readonly IUserRepository _userRepo;
	private readonly ILogger<AuthService> _logger;
	//private readonly AppDbContext _dbContext;

	public AuthService( /*AppDbContext dbContext,*/ ITokenRepository tokenRepo, IUserRepository userRepo, ILogger<AuthService> logger )
	{
		//_dbContext = dbContext;
		_tokenRepo = tokenRepo;
		_userRepo = userRepo;
		_logger = logger;
	}

	public async Task<GetTokenDto> Login( string username, string password )
	{
		//var user = _dbContext.Users
		//	.Where( u => u.UserName == username )
		//	.FirstOrDefault();
		var user = await _userRepo.GetUsers()
			.FilterByUsername( username )
			.FirstOrDefaultAsync( _userRepo );

		if ( user is null )
		{
			throw new NotFoundException();
		}

		var hash = await HashPasswordAsync( password );

		if ( user.PasswordHash != hash )
		{
			throw new NotAuthorizedException();
		}

		var token = await GenerateTokenAsync( user );

		return GetTokenDto.FromToken( token );
	}

	private const int tokenValidityMinutes = 10;
	private async Task<TokenModel> GenerateTokenAsync( UserModel user )
	{
		(string access, string refresh) = await GenerateTokens();

		TokenModel token = new( access, refresh, user.Uuid, DateTimeOffset.UtcNow.AddMinutes( tokenValidityMinutes ) );

		//_ = await _dbContext.Tokens.AddAsync( token );
		await _tokenRepo.AddTokenAsync( token );

		//_ = await _dbContext.SaveChangesAsync();
		_ = await _tokenRepo.SaveAsync();

		return token;
	}
	private async Task<(string access, string refresh)> GenerateTokens()
	{
		string refresh;
		string access;
		bool isUsed;
		do
		{
			var accGuid = Guid.NewGuid();
			access = Base64UrlTextEncoder.Encode( accGuid.ToByteArray() );
			var refGuid = Guid.NewGuid();
			refresh = Base64UrlTextEncoder.Encode( refGuid.ToByteArray() );

			isUsed = await _tokenRepo.GetTokens()
				.Where( t => t.Token == access || t.RefreshToken == refresh )
				.AnyAsync( _tokenRepo );
			//} while ( await _dbContext.Tokens.AnyAsync( t => t.Token == access || t.RefreshToken == refresh ) );
		} while ( isUsed );
		return (refresh, access);
	}

	public async Task RevokeTokenAsync( string token )
	{
		//var found = await _dbContext.Tokens
		//	.Where( t => t.Token == token )
		//	.FirstOrDefaultAsync();
		var found = await _tokenRepo.GetTokens()
			.FilterByAccessToken( token )
			.FirstOrDefaultAsync( _tokenRepo );

		if ( found is null )
		{
			throw new TokenInvalidException();
		}

		await RevokeTokenAsync( found );
	}
	public async Task RevokeTokenAsync( TokenModel token )
	{
		if ( token is null )
		{
			throw new ArgumentNullException( nameof( token ) );
		}

		if ( token.Revoked )
		{
			return;
		}

		token.Revoked = true;

		var modified = await _tokenRepo.SaveAsync();
		if ( modified < 1 )
		{
			throw new TokenInvalidException();
		}
	}

	public async Task<GetTokenDto> RefreshTokenAsync( string token )
	{
		//todo: add user uuid as parameter, remove following db call
		//var found = await _dbContext.Tokens
		//	.Where( t => t.Token == token )
		//	.FirstOrDefaultAsync();
		var found = await _tokenRepo.GetTokens()
			.FilterByRefreshToken( token )
			.FirstOrDefaultAsync( _tokenRepo );

		if ( found is null )
		{
			throw new TokenInvalidException();
		}

		(string access, string refresh) = await GenerateTokens();

		TokenModel @new = new( access, refresh, found.UserUuid, DateTimeOffset.UtcNow.AddMinutes( tokenValidityMinutes ) );

		//_ = await _dbContext.Tokens.AddAsync( @new );
		await _tokenRepo.AddTokenAsync( @new );
		found.Revoked = true;

		//_ = await _dbContext.SaveChangesAsync();
		_ = await _tokenRepo.SaveAsync();

		return GetTokenDto.FromToken( @new );
	}


	private static readonly SHA512 sha = SHA512.Create();
	public static async Task<string> HashPasswordAsync( string password )
	{
		await Task.CompletedTask;
		// todo: change to proper algorythm

		var passwordHash = sha.ComputeHash( Encoding.UTF8.GetBytes( password ) );
		return Convert.ToBase64String( passwordHash );
	}


	public Task<TokenModel?> GetTokenAsync( string token )
	{
		//return _dbContext.Tokens
		//	.Include( t => t.User! )
		//	.ThenInclude( u => u.Roles! )
		//	.Where( t => t.Token == token )
		//	.FirstOrDefaultAsync();
		return _tokenRepo.GetTokens()
			.WithUsersWithRoles( _tokenRepo )
			.FilterByAccessToken( token )
			.FirstOrDefaultAsync( _tokenRepo );
	}


}
