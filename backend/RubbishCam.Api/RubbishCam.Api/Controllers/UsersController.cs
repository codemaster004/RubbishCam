using Microsoft.AspNetCore.Mvc;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.User;
using RubbishCam.Domain.Models;

namespace RubbishCam.Api.Controllers;

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
	public async Task<ActionResult<GetUserDto[]>> GetAll()
	{
		return await _usersService.GetUsersAsync();
	}

	// GET api/<UsersController>/5
	[HttpGet( "{uuid}" )]
	public async Task<ActionResult<GetUserDetailsDto>> Get( string uuid )
	{
		GetUserDetailsDto? user = await _usersService.GetUserAsync( uuid );

		if ( user is null )
		{
			return NotFound( "User not found" );
		}

		return user;
	}

	// POST api/<UsersController>
	[HttpPost]
	public async Task<ActionResult<GetUserDto>> Create( [FromBody] CreateUserDto dto )
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

		return CreatedAtAction( nameof( Get ), new { user.Uuid }, user );
	}

	// DELETE api/<UsersController>/5
	[HttpDelete( "{uuid}" )]
	public async Task<IActionResult> Delete( string uuid )
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


}
