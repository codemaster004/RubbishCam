using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubbishCam.Api.Extensions;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.Group;
using RubbishCam.Domain.Dtos.Group.Membership;

namespace RubbishCam.Api.Controllers;

[Authorize]
[ApiController]
[Route( "api/[controller]" )]
public class GroupsController : ExtendedControllerBase
{
	private readonly IGroupsService _groupsService;

	public GroupsController( IGroupsService groupsService )
	{
		_groupsService = groupsService;
	}

	[HttpGet]
	public async Task<ActionResult<GetGroupDto[]>> GetGroups()
	{
		var uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		return await _groupsService.GetGroupsAsync( uuid );
	}

	[HttpGet( "owned" )]
	public async Task<ActionResult<GetGroupDto[]>> GetOwnedGroups()
	{
		var uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		return await _groupsService.GetOwnedGroupsAsync( uuid );
	}

	[HttpGet( "{id}" )]
	public async Task<ActionResult<GetGroupDetailsDto>> GetGroup( int id )
	{
		var uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		GetGroupDetailsDto? group;
		try
		{
			group = await _groupsService.GetGroupAsync( id, uuid );
		}
		catch ( NotAuthorizedException )
		{
			return Forbidden( "you do not belong to this group" );
		}

		if ( group is null )
		{
			return NotFound( "Group not found" );
		}

		return group;
	}

	[HttpPost]
	public async Task<IActionResult> CreateGroup( [FromBody] CreateGroupDto dto )
	{
		var uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		var group = await _groupsService.CreateGroupAsync( dto, uuid );

		return CreatedAtAction( nameof( GetGroup ), new { group.Id }, group );
	}

	[HttpGet("{id}/members")]
	public async Task<ActionResult<GetGroupMembershipDto[]>> GetMembers( int id )
	{
		var uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		try
		{
			return await _groupsService.GetGroupMembersAsync( id, uuid );
		}
		catch ( NotAuthorizedException )
		{
			return Forbidden( "You do not have permission to access to this group" );
		}
		catch ( NotFoundException )
		{
			return NotFound( "Group does not exist" );
		}
	}

	[HttpPost( "{id}/members/add" )]
	public async Task<IActionResult> AddMember( int id, [FromBody] string targetUuid )
	{
		var uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		try
		{
			await _groupsService.AddToGroupAsync( id, targetUuid, uuid );
		}
		catch ( NotAuthorizedException )
		{
			return Forbidden( "You do not have permission to add users to this group" );
		}
		catch ( ConflictException )
		{
			return Conflict( "Given user is already member of this group" );
		}
		catch ( NotFoundException )
		{
			return NotFound( "Group or user does not exist" );
		}

		return NoContent();
	}

	[HttpPost( "{id}/members/remove" )]
	public async Task<IActionResult> RemoveMember( int id, [FromBody] string targetUuid )
	{
		var uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		try
		{
			await _groupsService.RemoveFromGroupAsync( id, targetUuid, uuid );
		}
		catch ( NotAuthorizedException )
		{
			return Forbidden( "You do not have permission to remove users from this group" );
		}
		catch ( ConflictException )
		{
			return Conflict( "Given user is not member of this group" );
		}
		catch ( NotFoundException )
		{
			return NotFound( "Group does not exist" );
		}

		return NoContent();
	}


}
