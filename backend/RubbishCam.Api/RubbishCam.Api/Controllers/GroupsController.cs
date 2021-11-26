using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RubbishCam.Api.Extensions;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.Group;

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

		return await _groupsService.GetGroups( uuid );
	}

	[HttpGet( "owned" )]
	public async Task<ActionResult<GetGroupDto[]>> GetOwnedGroups()
	{
		var uuid = User.GetUserUuid();
		if ( uuid is null )
		{
			return InternalServerError();
		}

		return await _groupsService.GetOwnedGroups( uuid );
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
			group = await _groupsService.GetGroup( id, uuid );
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

		var group = await _groupsService.CreateGroup( dto, uuid );

		return CreatedAtAction( nameof( GetGroup ), new { group.Id }, group );
	}


}
