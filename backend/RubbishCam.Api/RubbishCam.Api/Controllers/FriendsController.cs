using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RubbishCam.Api.Extensions;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.Friendship;
using System.Security.Claims;

namespace RubbishCam.Api.Controllers;

[Authorize]
[ApiController]
[Route( "api/[controller]" )]
public class FriendsController : ExtendedControllerBase
{
	private readonly IFriendsService _friendsService;
	private readonly ILogger<FriendsController> _logger;

	public FriendsController( IFriendsService friendsService, ILogger<FriendsController> logger )
	{
		_friendsService = friendsService;
		_logger = logger;
	}

	[HttpGet]
	public async Task<ActionResult<GetFriendshipDto[]>> GetFriendships()
	{
		string? uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		return await _friendsService.GetFriendshipsAsync( uuid );
	}

	[HttpGet( "accepted" )]
	public async Task<ActionResult<GetFriendshipDto[]>> GetAcceptedFriendships()
	{
		string? uuid = User.GetUserUuid();

		if ( uuid is null )
		{
			return InternalServerError();
		}

		return await _friendsService.GetAcceptedFriendshipsAsync( uuid );
	}

	[HttpGet( "{id}" )]
	public async Task<ActionResult<GetFriendshipDto>> GetFriendship( int id )
	{
		var friendship = await _friendsService.GetFriendshipAsync( id );

		if ( friendship is null )
		{
			return NotFound();
		}

		return friendship;
	}

	[HttpPost( "{targetUuid}" )]
	public async Task<ActionResult<GetFriendshipDto>> CreateFriendship( string targetUuid )
	{
		string? initiatorUuid = User.GetUserUuid();
		if ( initiatorUuid is null )
		{
			return InternalServerError();
		}

		GetFriendshipDto friendship;
		try
		{
			friendship = await _friendsService.CreateFriendshipAsync( initiatorUuid, targetUuid );
		}
		catch ( NotFoundException )
		{
			return NotFound( "User not found" );
		}
		catch ( ConflictException )
		{
			return Conflict( "User already added to friends" );
		}

		return friendship;
	}

	[HttpPost( "{id}/accept" )]
	public async Task<IActionResult> AcceptFriendship( int id )
	{
		try
		{
			await _friendsService.AcceptFriendshipAsync( id );
		}
		catch ( NotFoundException )
		{
			return NotFound();
		}

		return NoContent();
	}

	[HttpPost( "{id}/reject" )]
	public async Task<IActionResult> RejectFriendship( int id )
	{
		try
		{
			await _friendsService.RejectFriendshipAsync( id );
		}
		catch ( NotFoundException )
		{
			return NotFound();
		}
		catch ( ConflictException )
		{
			return Conflict( "Friendship is accepted" );
		}
		return NoContent();
	}

	[HttpDelete( "{id}" )]
	public async Task<IActionResult> DeleteFriendship( int id )
	{
		try
		{
			await _friendsService.DeleteFriendshipAsync( id );
		}
		catch ( NotFoundException )
		{
			return NotFound();
		}

		return NoContent();
	}


}
