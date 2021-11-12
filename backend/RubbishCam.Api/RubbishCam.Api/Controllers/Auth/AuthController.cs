using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubbishCam.Api.Exceptions.Auth;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.Token;
using System.ComponentModel.DataAnnotations;

namespace RubbishCam.Api.Controllers.Auth;

[Authorize]
[ApiController]
[Route( "auth/token" )]
public class AuthController : ExtendedControllerBase
{
	private readonly IAuthService _authService;

	public AuthController( IAuthService authService )
	{
		_authService = authService;
	}

	[HttpPost( "login" )]
	[AllowAnonymous]
	public async Task<ActionResult<GetTokenDto>> Login( [FromBody] LoginModel data )
	{
		GetTokenDto? token;
		try
		{
			token = await _authService.Login( data.Username, data.Password );
		}
		catch ( NotFoundException )
		{
			return Unauthorized( "Incorrect username" );
		}
		catch ( AuthService.NotAuthorizedException )
		{
			return Unauthorized( "Incorrect password" );
		}

		return token;
	}

	[HttpPost( "logout" )]
	public async Task<IActionResult> Logout()
	{
		string? token = await HttpContext.GetTokenAsync( "access_token" );

		if ( token is null )
		{
			return InternalServerError( "Error occured" );
			throw new Exception();
		}

		try
		{
			await _authService.RevokeTokenAsync( token );
		}
		catch ( TokenInvalidException )
		{
			return Unauthorized( "The token is invalid" );
		}

		return NoContent();
	}

#nullable disable warnings
	public class LoginModel
	{
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
	}
#nullable restore

}
