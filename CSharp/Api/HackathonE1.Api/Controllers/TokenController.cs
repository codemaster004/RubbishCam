﻿using HackathonE1.Api.Services;
using HackathonE1.Domain.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Controllers
{
	[Authorize]
	[ApiController]
	[Route( "auth/[controller]" )]
	public class TokenController : ControllerBase
	{
		private readonly ILogger<TokenController> _logger;
		private readonly IJwtService _jwtService;

		public TokenController( ILogger<TokenController> logger, IJwtService jwtService )
		{
			_logger = logger;
			_jwtService = jwtService;
		}

		[HttpPost()]
		[AllowAnonymous]
		public async Task<ActionResult<string>> CreateToken( [FromBody] LoginDto model )
		{
			string token = await _jwtService.AuthenticateAsync( model.Username, model.Password );

			if ( string.IsNullOrEmpty( token ) )
			{
				_logger.LogInformation( $"Failed token request for {model.Username}" );
				return Unauthorized();
			}

			Response.Cookies.Append( Constants.AuthCookieName, token, new()
			{
				Expires = DateTime.UtcNow.AddMinutes( 15 ),
				Path = "/",
				Domain = "",
				IsEssential = true,
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.None
			} );

			_logger.LogInformation( $"User {model.Username} requested token" );
			return token;
		}

		[HttpPost( "Logout" )]
		public async Task<IActionResult> Logout()
		{
			await Task.CompletedTask;

			Response.Cookies.Delete( Constants.AuthCookieName );

			return NoContent();
		}


		[HttpPost( "refresh" )]
		public async Task<IActionResult> RefreshToken()
		{
			string @new = await _jwtService.RefreshAsync( User );

			if ( string.IsNullOrEmpty( @new ) )
			{
				_logger.LogInformation( $"Failed token request for {User.Identity.Name}" );
				return Unauthorized();
			}

			_logger.LogInformation( $"User {User.Identity.Name} requested token" );
			return Ok( @new );
		}


		[HttpGet( "check" )]
		public async Task<ActionResult<string>> CheckToken()
		{
			await Task.CompletedTask;
			var accessToken = await HttpContext.GetTokenAsync( "access_token" );
			if ( string.IsNullOrEmpty(accessToken) )
			{
				return Unauthorized();
			}

			return accessToken;
		}

	}
}
