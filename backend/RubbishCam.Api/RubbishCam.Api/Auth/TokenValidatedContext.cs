using Microsoft.AspNetCore.Authentication;

namespace RubbishCam.Api.Auth;

public class TokenValidatedContext : ResultContext<TokenOptions>
{
	public TokenValidatedContext( HttpContext context, AuthenticationScheme scheme, TokenOptions options ) 
		: base( context, scheme, options )
	{
	}
}
