using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace RubbishCam.Api.Extensions;

public static class AuthenticationExtensions
{
	const string TokenKeyPrefix = ".TokenObject.";
	public static void StoreToken<T>( this AuthenticationProperties properties, string tokenName, T token )
	{
		var tokenKey = TokenKeyPrefix + tokenName;
		properties.SetParameter( tokenKey, token );
	}
	public static Task<T?> GetTokenAsync<T>( this HttpContext context, string tokenName ) where T : class
	{
		return context.RequestServices.GetRequiredService<IAuthenticationService>().GetTokenAsync<T>( context, tokenName );
	}
	public static Task<T?> GetTokenAsync<T>( this IAuthenticationService auth, HttpContext context, string tokenName ) where T : class
			=> auth.GetTokenAsync<T>( context, scheme: null, tokenName: tokenName );

	public static async Task<T?> GetTokenAsync<T>( this IAuthenticationService auth, HttpContext context, string? scheme, string tokenName ) where T : class
	{
		if ( auth is null )
		{
			throw new ArgumentNullException( nameof( auth ) );
		}
		if ( tokenName is null )
		{
			throw new ArgumentNullException( nameof( tokenName ) );
		}

		AuthenticateResult result = await auth.AuthenticateAsync( context, scheme );
		return result.Properties?.GetTokenValue<T>( tokenName );
	}

	public static T? GetTokenValue<T>( this AuthenticationProperties properties, string tokenName )
	{
		if ( properties is null )
		{
			throw new ArgumentNullException( nameof( properties ) );
		}
		if ( tokenName is null )
		{
			throw new ArgumentNullException( nameof( tokenName ) );
		}

		var tokenKey = TokenKeyPrefix + tokenName;
		return properties.GetParameter<T>( tokenKey );
	}

	public static string? GetUserUuid( this ClaimsPrincipal principal )
	{
		return principal.Claims.Where( c => c.Type == ClaimTypes.Name ).FirstOrDefault()?.Value;
	}
}
