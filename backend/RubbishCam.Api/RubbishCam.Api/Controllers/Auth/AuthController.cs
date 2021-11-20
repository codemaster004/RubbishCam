using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubbishCam.Api.Exceptions.Auth;
using RubbishCam.Api.Extensions;
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
		catch ( NotAuthorizedException )
		{
			return Unauthorized( "Incorrect password" );
		}

		return token;
	}

	[HttpPost( "logout" )]
	public async Task<IActionResult> Logout()
	{
		var token = await HttpContext.GetTokenAsync<Domain.Models.TokenModel>( "access_token" );

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

	[AllowAnonymous]
	[HttpPost( "refresh" )]
	public async Task<ActionResult<GetTokenDto>> Refresh( [FromBody] RefreshModel token )
	{
		GetTokenDto? @new;

		if ( token is null )
		{
			return InternalServerError( "Error occured" );
			throw new Exception();
		}

		try
		{
			@new = await _authService.RefreshTokenAsync( token.RefreshToken );
		}
		catch ( TokenInvalidException )
		{
			return Unauthorized( "Invalid refresh token" );
		}

		return @new;
	}

#nullable disable warnings
	public class LoginModel
	{
		[Required]
		public string Username { get; set; }
		[Required]
		public string Password { get; set; }
	}

	public class RefreshModel
	{
		public string RefreshToken { get; set; }
	}
#nullable restore

}
