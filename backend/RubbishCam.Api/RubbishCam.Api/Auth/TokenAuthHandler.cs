using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RubbishCam.Data;
using RubbishCam.Domain.Models;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace RubbishCam.Api.Auth;

public class TokenAuthHandler<T> : AuthenticationHandler<TokenOptions> where T : class, IAuthDataProvider
{
	private readonly IAuthDataProvider _provider;

	public TokenAuthHandler( IAuthDataProvider provider, IOptionsMonitor<TokenOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock )
		: base( options, logger, encoder, clock )
	{
		_provider = provider ?? throw new ArgumentNullException( nameof( provider ) );
	}


	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		await Task.CompletedTask;

		var sentTokenValue = ReadAuthHeader();

		if ( sentTokenValue is null )
		{
			return AuthenticateResult.NoResult();
		}

		TokenModel? foundToken = _provider.Tokens
			.Include( t => t.User! )
			.ThenInclude( u => u.Roles! )
			.Where( t => t.Token == sentTokenValue )
			.FirstOrDefault();

		if ( foundToken is null )
		{
			return AuthenticateResult.NoResult();
		}

		if ( foundToken.User is null )
		{
			throw new NullReferenceException();
		}

		Claim[] claims = GetClaims( foundToken.User ).ToArray();

		ClaimsIdentity identity = new( claims, "Bearer" );

		ClaimsPrincipal principal = new( identity );

		AuthenticationTicket ticket = new( principal, "Bearer" );

		return AuthenticateResult.Success( ticket );

	}

	private string? ReadAuthHeader()
	{
		string authorization = Request.Headers["Authorization"];

		if ( string.IsNullOrEmpty( authorization ) )
		{
			return null;
		}

		if ( !authorization.StartsWith( "Bearer ", StringComparison.OrdinalIgnoreCase ) )
		{
			return null;
		}

		var token = authorization["Bearer ".Length..].Trim();

		if ( string.IsNullOrEmpty( token ) )
		{
			return null;
		}

		return token;
	}

	private static IEnumerable<Claim> GetClaims( UserModel user )
	{
		yield return new( ClaimTypes.Name, user.Uuid );
		yield return new( ClaimTypes.GivenName, user.FirstName );
		yield return new( ClaimTypes.Surname, user.LastName );

		foreach ( var role in user.Roles )
		{
			yield return new( ClaimTypes.Role, role.Name );
		}
	}


}
