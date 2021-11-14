using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.User;
using RubbishCam.Domain.Models;
using System.Security.Claims;

namespace RubbishCam.Api.Controllers;

[Authorize]
[ApiController]
[Route( "api/[controller]" )]
public class UsersController : ExtendedControllerBase
{
	private readonly IUsersService _usersService;
	private readonly ILogger<UsersController> _logger;

	public UsersController( IUsersService usersService, ILogger<UsersController> logger )
	{
		_usersService = usersService ?? throw new ArgumentNullException( nameof( usersService ) );
		_logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
	}


	// GET: api/<UsersController>
	[HttpGet]
	[Authorize( Roles = "Admin" )]
	public async Task<ActionResult<GetUserDto[]>> GetAllUsers()
	{
		return await _usersService.GetUsersAsync();
	}

	// GET api/<UsersController>/5
	[HttpGet( "{uuid}" )]
	[Authorize( Roles = Constants.Auth.AdminRole )]
	public async Task<ActionResult<GetUserDetailsDto>> GetUser( string uuid )
	{
		GetUserDetailsDto? user = await _usersService.GetUserAsync( uuid );

		if ( user is null )
		{
			return NotFound( "User not found" );
		}

		return user;
	}

	// GET api/<UsersController>/current
	[HttpGet( "current" )]
	public async Task<ActionResult<GetUserDetailsDto>> GetCurrentUser()
	{
		string? uuid = User.Claims.Where( c => c.Type == ClaimTypes.Name ).FirstOrDefault()?.Value;
		if ( uuid is null )
		{
			return InternalServerError( "Unexpected error occured" );
		}

		GetUserDetailsDto? user = await _usersService.GetUserAsync( uuid );

		if ( user is null )
		{
			return InternalServerError( "Unexpected error occured" );
		}

		return user;
	}

	// POST api/<UsersController>
	[HttpPost]
	[AllowAnonymous]
	public async Task<ActionResult<GetUserDto>> CreateUser( [FromBody] CreateUserDto dto )
	{
		GetUserDto user;
		try
		{
			user = await _usersService.CreateUserAsync( dto );
		}
		catch ( Exception )
		{
			return InternalServerError( "Unexpected error occured. Try again." );
		}

		if ( User?.IsInRole( Constants.Auth.AdminRole ) is true )
		{
			return CreatedAtAction( nameof( GetUser ), new { user.Uuid }, user );

		}
		return CreatedAtAction( nameof( GetCurrentUser ), user );
	}

	// DELETE api/<UsersController>/5
	[HttpDelete( "{uuid}" )]
	[Authorize( Roles = Constants.Auth.AdminRole )]
	public async Task<IActionResult> DeleteUser( string uuid )
	{
		try
		{
			await _usersService.DeleteUserAsync( uuid );
		}
		catch ( NotFoundException )
		{
			return NotFound( "User not found" );
		}
		catch ( Exception )
		{
			return InternalServerError( "Unexpected error occured. Try again." );
		}

		return NoContent();
	}

	// DELETE api/<UsersController>/5
	[HttpDelete( "current" )]
	public async Task<IActionResult> DeleteCurrentUser()
	{
		string? uuid = User.Claims.Where( c => c.Type == ClaimTypes.Name ).FirstOrDefault()?.Value;
		if ( uuid is null )
		{
			return InternalServerError( "Unexpected error occured" );
		}

		try
		{
			await _usersService.DeleteUserAsync( uuid );
		}
		catch ( NotFoundException )
		{
			return NotFound( "User not found" );
		}
		catch ( Exception )
		{
			return InternalServerError( "Unexpected error occured. Try again." );
		}

		return NoContent();
	}


}
