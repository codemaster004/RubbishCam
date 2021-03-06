using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using RubbishCam.Api.Exceptions.Auth;
using RubbishCam.Api.Extensions;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace RubbishCam.Api.Auth;

public class TokenAuthHandler : AuthenticationHandler<TokenOptions>
{
	private readonly IAuthService _authService;
	private readonly ProblemDetailsFactory _detailsFactory;
	private readonly IActionResultExecutor<ObjectResult> _executor;

	public TokenAuthHandler( IAuthService authService,
						 IActionResultExecutor<ObjectResult> executor,
						 ProblemDetailsFactory detailsFactory,
						 IOptionsMonitor<TokenOptions> options,
						 ILoggerFactory logger,
						 UrlEncoder encoder,
						 ISystemClock clock )
		: base( options, logger, encoder, clock )
	{
		_authService = authService ?? throw new ArgumentNullException( nameof( authService ) );
		_detailsFactory = detailsFactory ?? throw new ArgumentNullException( nameof( detailsFactory ) );
		_executor = executor ?? throw new ArgumentNullException( nameof( executor ) );
	}


	protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		await Task.CompletedTask;

		var sentTokenValue = ReadAuthHeader();

		if ( sentTokenValue is null )
		{
			return AuthenticateResult.NoResult();
		}

		TokenModel? foundToken = await _authService.GetTokenAsync( sentTokenValue );

		if ( foundToken is null )
		{
			return AuthenticateResult.Fail( new TokenInvalidException() );
		}

		if ( foundToken.User is null )
		{
			throw new NullReferenceException();
		}

		if ( foundToken.ValidUntil < DateTimeOffset.Now )
		{
			return AuthenticateResult.Fail( new TokenExpiredException() );
		}

		if ( foundToken.Revoked )
		{
			return AuthenticateResult.Fail( new TokenRevokedException() );
		}

		Claim[] claims = GetClaims( foundToken.User ).ToArray();
		ClaimsIdentity identity = new( claims, Scheme.Name );
		ClaimsPrincipal principal = new( identity );

		var tokenValidatedContext = new TokenValidatedContext( Context, Scheme, Options )
		{
			Principal = principal
		};

		tokenValidatedContext.Properties.StoreTokens( new[]
		{
			new AuthenticationToken { Name = Constants.Auth.TokenName, Value = sentTokenValue }
		} );
		tokenValidatedContext.Properties.StoreToken( Constants.Auth.TokenName, foundToken );

		tokenValidatedContext.Success();

		return tokenValidatedContext.Result;
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


	protected override async Task HandleChallengeAsync( AuthenticationProperties properties )
	{
		Response.StatusCode = 401;

		AuthenticateResult authResult = await HandleAuthenticateOnceSafeAsync();
		if ( !authResult.None && authResult.Failure is null )
		{
			return;
		}

		// https://datatracker.ietf.org/doc/html/rfc6750#section-3.1
		StringBuilder wwwauth = new( $"{Scheme.Name} realm=\"RubbishCamDefault\"" );

		if ( authResult.None )
		{
			Response.Headers.Append( HeaderNames.WWWAuthenticate, wwwauth.ToString() );
			return;
		}

		string? detail = CreateErrorDescription( authResult.Failure! );
		if ( detail is not null )
		{
			_ = wwwauth.Append( "error=\"invalid_token\", "
					   + $"error_description=\"{detail}\"" );
		}

		Response.Headers.Append( HeaderNames.WWWAuthenticate, wwwauth.ToString() );

		await FillResponse( Response.StatusCode, detail );

	}

	protected override async Task HandleForbiddenAsync( AuthenticationProperties properties )
	{
		Response.StatusCode = 403;

		// https://datatracker.ietf.org/doc/html/rfc6750#section-3.1
		Response.Headers.Append( HeaderNames.WWWAuthenticate, $"{Scheme.Name} realm=\"RubbishCamDefault\", error=\"insufficient_scope\"" );

		await FillResponse( Response.StatusCode, "Insufficient scope" );
	}


	private async Task FillResponse( int code, string? detail )
	{
		ActionContext actionContext = new( Context, Context.GetRouteData(), new ActionDescriptor() );

		ProblemDetails problemDetails = _detailsFactory.CreateProblemDetails( Context, code, detail: detail );
		ObjectResult result = new( problemDetails )
		{
			StatusCode = problemDetails.Status
		};

		await _executor.ExecuteAsync( actionContext, result );
	}
	private static string? CreateErrorDescription( Exception authFailure )
	{
		return authFailure switch
		{
			TokenInvalidException =>
				"The token is invalid",
			TokenExpiredException =>
				"The token is expired",
			TokenRevokedException =>
				"The token is revoked",
			_ => null
		};
	}

}
