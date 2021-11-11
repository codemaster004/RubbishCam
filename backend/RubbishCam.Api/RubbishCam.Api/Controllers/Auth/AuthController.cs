using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.Token;
using System.ComponentModel.DataAnnotations;

namespace RubbishCam.Api.Controllers.Auth;

[Route( "auth/token" )]
[ApiController]
public class AuthController : ExtendedControllerBase
{
	private readonly IAuthService _authService;

	public AuthController( IAuthService authService )
	{
		_authService = authService;
	}

	[HttpPost]
	public async Task<ActionResult<GetTokenDto>> LogIn( [FromBody] LoginModel data )
	{
		GetTokenDto? token;
		try
		{
			token = await _authService.LogIn( data.Username, data.Password );
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
