using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using RubbishCam.Api.Exceptions.Auth;
using RubbishCam.Data;
using RubbishCam.Domain.Dtos.Token;
using RubbishCam.Domain.Models;
using System.Security.Cryptography;
using System.Text;

namespace RubbishCam.Api.Services;

public interface IAuthService
{
	Task<GetTokenDto> Login( string username, string password );
	Task<TokenModel?> GetTokenAsync( string token );
	Task RevokeTokenAsync( string token );
}

public class AuthService : IAuthService
{
	private readonly AppDbContext _dbContext;

	public AuthService( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	public async Task<GetTokenDto> Login( string username, string password )
	{
		var user = _dbContext.Users
			.Where( u => u.UserName == username )
			.FirstOrDefault();

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

	const int tokenValidityMinutes = 10;
	private async Task<TokenModel> GenerateTokenAsync( UserModel user )
	{
		string access;
		do
		{
			var guid = Guid.NewGuid();
			access = Base64UrlTextEncoder.Encode( guid.ToByteArray() );

		} while ( await _dbContext.Tokens.AnyAsync( t => t.Token == access ) );

		string refresh;
		do
		{
			var guid = Guid.NewGuid();
			refresh = Base64UrlTextEncoder.Encode( guid.ToByteArray() );

		} while ( await _dbContext.Tokens.AnyAsync( t => t.RefreshToken == refresh ) );

		TokenModel token = new( access, refresh, user.Uuid, DateTimeOffset.UtcNow.AddMinutes( tokenValidityMinutes ) );

		_ = await _dbContext.Tokens.AddAsync( token );

		_ = await _dbContext.SaveChangesAsync();

		return token;
	}


	public async Task RevokeTokenAsync( string token )
	{
		var found = await _dbContext.Tokens
			.Where( t => t.Token == token )
			.FirstOrDefaultAsync();

		if ( found is null )
		{
			throw new TokenInvalidException();
		}

		found.Revoked = true;

		_ = await _dbContext.SaveChangesAsync();
	}




	static readonly SHA512 sha = SHA512.Create();
	public static async Task<string> HashPasswordAsync( string password )
	{
		await Task.CompletedTask;
		// todo: change to proper algorythm

		var passwordHash = sha.ComputeHash( Encoding.UTF8.GetBytes( password ) );
		return Convert.ToBase64String( passwordHash );
	}


	public Task<TokenModel?> GetTokenAsync( string token )
	{
		return _dbContext.Tokens
			.Include( t => t.User! )
			.ThenInclude( u => u.Roles! )
			.Where( t => t.Token == token )
			.FirstOrDefaultAsync();
	}



	[Serializable]
	public class NotAuthorizedException : Exception
	{
		public NotAuthorizedException() { }
		public NotAuthorizedException( string message ) : base( message ) { }
		public NotAuthorizedException( string message, Exception inner ) : base( message, inner ) { }
		protected NotAuthorizedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
	}

}
