using HackathonE1.Api.Services;
using HackathonE1.Domain.Dtos.User;
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
	[ApiController]
	[Authorize]
	[Route( "api/[controller]" )]
	public class UsersController : ExtendedControllerBase
	{
		private readonly ILogger<UsersController> _logger;
		private readonly IUsersService _usersService;

		public UsersController( ILogger<UsersController> logger, IUsersService usersService )
		{
			_logger = logger;
			_usersService = usersService;
		}

		[HttpGet]
		[Authorize( Roles = "Admin" )]
		public async Task<ActionResult<GetUserDto[]>> GetUsers()
		{
			_logger.LogInformation( $"User {UserName} requested al users' info." );
			return await _usersService.GetUsersAsync();
		}

		[HttpGet( "{identifier}" )]
		[Authorize( Roles = "Admin" )]
		public async Task<ActionResult<GetUserDto>> GetUser( string identifier )
		{
			var user = await _usersService.GetUserAsync( identifier );
			if ( user is null )
			{
				_logger.LogInformation( $"User {UserName} requested unknown user's {identifier} info." );
				return NotFound( ProblemConstants.UserNotFound );
			}

			_logger.LogInformation( $"User {UserName} requested user's {identifier} info." );
			return user;
		}

		[HttpGet( "current" )]
		public async Task<ActionResult<GetUserDto>> GetCurrentUser()
		{
			var user = await _usersService.GetUserAsync( UserName );
			if ( user is null )
			{
				_logger.LogInformation( $"Unknown user {UserName} requested their info." );
				return NotFound( ProblemConstants.UserNotFound );
			}

			_logger.LogInformation( $"User {UserName} requested their info." );
			return user;
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<ActionResult<GetUserDto>> CreateUser( [FromBody] CreateUserDto userDto )
		{
			var user = await _usersService.AddUserAsync( userDto );
			if ( user is null )
			{
				_logger.LogInformation( $"Attempted creating user account for email {user.Email}." );
				return Conflict( ProblemConstants.UserEmailTaken );
			}

			_logger.LogInformation( $"Created user account for email {user.Email} with identifier {user.Identifier}." );
			return CreatedAtAction( nameof( GetCurrentUser ), user );
		}

		[HttpDelete( "{identifier}" )]
		[Authorize( Roles = "Admin" )]
		public async Task<IActionResult> DeleteUser( string identifier )
		{
			var deleted = await _usersService.DeleteUserAsync( identifier );
			if ( !deleted )
			{
				_logger.LogInformation( $"User {UserName} failed deleting user account with identifier {identifier}: user not found." );
				return NotFound( ProblemConstants.UserNotFound );
			}

			_logger.LogInformation( $"User {UserName} Deleted user account with identifier {identifier}." );
			return NoContent();
		}

		[HttpDelete( "current" )]
		public async Task<IActionResult> DeleteCurrentUser()
		{
			var deleted = await _usersService.DeleteUserAsync( UserName );
			if ( !deleted )
			{
				_logger.LogInformation( $"Failed deleting user account with identifier {UserName}: user not found." );
				return NotFound( ProblemConstants.UserNotFound );
			}

			_logger.LogInformation( $"Deleted user account with identifier {UserName}." );
			return NoContent();
		}

	}
}
