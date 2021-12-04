using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Dtos.Group;
using RubbishCam.Domain.Dtos.Group.Membership;

namespace RubbishCam.Api.Services;

public interface IGroupsService
{
	Task<GetGroupDto[]> GetGroupsAsync();
	Task<GetGroupDto[]> GetGroupsAsync( string userUuid );
	Task<GetGroupDto[]> GetOwnedGroupsAsync( string userUuid );
	Task<GetGroupDetailsDto?> GetGroupAsync( int id, string userUuid );
	Task<GetGroupDetailsDto> CreateGroupAsync( CreateGroupDto dto, string userUuid );
	Task<GetGroupMembershipDto[]> GetGroupMembersAsync( int groupId, string requestorUuid );
	Task AddToGroupAsync( int groupId, string targetUuid, string requestorUuid );
	Task RemoveFromGroupAsync( int groupId, string targetUuid, string requestorUuid );
	Task<GetGroupMembershipDto[]> GetOwnersAsync( int groupId, string requestorUuid );
	Task AddAsOwnerAsync( int groupId, string targetUuid, string requestorUuid );
	Task RemoveAsOwnerAsync( int groupId, string targetUuid, string requestorUuid );
}

public class GroupsService : IGroupsService
{
	private readonly IGroupsRepository _groupsRepo;
	private readonly IGroupsMembersRepository _grMeRepo;
	private readonly IUsersRepository _userRepo;
	private readonly ILogger<GroupsService> _logger;

	public GroupsService( IGroupsRepository groupsRepo, IGroupsMembersRepository grMeRepo, IUsersRepository userRepo, ILogger<GroupsService> logger )
	{
		_groupsRepo = groupsRepo;
		_grMeRepo = grMeRepo;
		_userRepo = userRepo;
		_logger = logger;
	}

	public Task<GetGroupDto[]> GetGroupsAsync()
	{
		return _groupsRepo.GetGroups()
			.Select( GetGroupDto.FromGroupExp )
			.ToArrayAsync( _groupsRepo );
	}

	public Task<GetGroupDto[]> GetGroupsAsync( string userUuid )
	{
		return _grMeRepo.GetGroupsMembersAsync()
			.FilterByUserUuid( userUuid )
			.Select( gm => gm.Group! )
			.Select( GetGroupDto.FromGroupExp )
			.ToArrayAsync( _grMeRepo );
	}

	public Task<GetGroupDto[]> GetOwnedGroupsAsync( string userUuid )
	{
		return _grMeRepo.GetGroupsMembersAsync()
			.FilterByUserUuid( userUuid )
			.FilterByOwnerships( true )
			.Select( gm => gm.Group! )
			.Select( GetGroupDto.FromGroupExp )
			.ToArrayAsync( _grMeRepo );
	}

	public async Task<GetGroupDetailsDto?> GetGroupAsync( int id, string userUuid )
	{
		var group = await _groupsRepo.GetGroups()
			.FilterById( id )
			.Select( GetGroupDetailsDto.FromGroupExp )
			.FirstOrDefaultAsync( _groupsRepo );

		if ( group is null )
		{
			return null;
		}

		var permited = await _grMeRepo.GetGroupsMembersAsync()
			.FilterGroupId( id )
			.FilterByUserUuid( userUuid )
			.AnyAsync( _grMeRepo );

		if ( !permited )
		{
			throw new NotAuthorizedException();
		}

		return group;
	}

	public async Task<GetGroupDetailsDto> CreateGroupAsync( CreateGroupDto dto, string userUuid )
	{
		var userExicts = await _userRepo.GetUsers()
			.FilterById( userUuid )
			.AnyAsync( _userRepo );
		if ( !userExicts )
		{
			throw new NotFoundException();
		}

		var group = dto.ToGroup( userUuid );

		await _groupsRepo.AddGroupAsync( group );
		_ = await _groupsRepo.SaveAsync();

		return GetGroupDetailsDto.FromGroup( group );
	}


	public async Task<GetGroupMembershipDto[]> GetGroupMembersAsync( int groupId, string requestorUuid )
	{
		var groupExists = await _groupsRepo.GetGroups()
			.FilterById( groupId )
			.AnyAsync( _groupsRepo );
		if ( !groupExists )
		{
			throw new NotFoundException();
		}

		var isMember = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.UserUuid == requestorUuid )
			.AnyAsync( _grMeRepo );
		if ( !isMember )
		{
			throw new NotAuthorizedException();
		}

		return await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Select( GetGroupMembershipDto.FromGroupMembersRelationExp )
			.ToArrayAsync( _grMeRepo );
	}

	public async Task AddToGroupAsync( int groupId, string targetUuid, string requestorUuid )
	{
		//todo: transaction lock
		var groupExists = await _groupsRepo.GetGroups()
			.FilterById( groupId )
			.AnyAsync( _groupsRepo );
		if ( !groupExists )
		{
			throw new NotFoundException();
		}

		var isOwner = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.IsOwner )
			.Where( um => um.UserUuid == requestorUuid )
			.AnyAsync( _grMeRepo );
		if ( !isOwner )
		{
			throw new NotAuthorizedException();
		}

		var alreadyAdded = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.UserUuid == targetUuid )
			.AnyAsync( _grMeRepo );
		if ( alreadyAdded )
		{
			throw new ConflictException();
		}

		var userExists = await _userRepo.GetUsers()
			.Where( um => um.Uuid == targetUuid )
			.AnyAsync( _userRepo );
		if ( !userExists )
		{
			throw new NotFoundException();
		}

		await _grMeRepo.AddGroupMemberAsync( new( targetUuid ) { GroupId = groupId, IsOwner = false } );
		_ = await _grMeRepo.SaveAsync();

	}

	public async Task RemoveFromGroupAsync( int groupId, string targetUuid, string requestorUuid )
	{
		var groupExists = await _groupsRepo.GetGroups()
			.FilterById( groupId )
			.AnyAsync( _groupsRepo );
		if ( !groupExists )
		{
			throw new NotFoundException();
		}

		var isOwner = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.IsOwner )
			.Where( um => um.UserUuid == requestorUuid )
			.AnyAsync( _grMeRepo );
		if ( !isOwner )
		{
			throw new NotAuthorizedException();
		}

		var membership = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.UserUuid == targetUuid )
			.FirstOrDefaultAsync( _grMeRepo );
		if ( membership is null )
		{
			throw new ConflictException();
		}

		await _grMeRepo.RemoveGroupMemberAsync( membership );
		_ = await _grMeRepo.SaveAsync();

	}


	public async Task<GetGroupMembershipDto[]> GetOwnersAsync( int groupId, string requestorUuid )
	{
		var groupExists = await _groupsRepo.GetGroups()
			.FilterById( groupId )
			.AnyAsync( _groupsRepo );
		if ( !groupExists )
		{
			throw new NotFoundException();
		}

		var isMember = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.UserUuid == requestorUuid )
			.AnyAsync( _grMeRepo );
		if ( !isMember )
		{
			throw new NotAuthorizedException();
		}

		return await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.IsOwner )
			.Select( GetGroupMembershipDto.FromGroupMembersRelationExp )
			.ToArrayAsync( _grMeRepo );
	}

	public async Task AddAsOwnerAsync( int groupId, string targetUuid, string requestorUuid )
	{
		var isOwner = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.IsOwner )
			.Where( um => um.UserUuid == requestorUuid )
			.AnyAsync( _grMeRepo );
		if ( !isOwner )
		{
			throw new NotAuthorizedException();
		}

		var membership = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.UserUuid == targetUuid )
			.FirstOrDefaultAsync( _grMeRepo );
		if ( membership is null )
		{
			throw new NotFoundException();
		}

		if ( membership.IsOwner )
		{
			throw new ConflictException();
		}

		membership.IsOwner = true;

		_ = await _grMeRepo.SaveAsync();
	}

	public async Task RemoveAsOwnerAsync( int groupId, string targetUuid, string requestorUuid )
	{
		var isOwner = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.IsOwner )
			.Where( um => um.UserUuid == requestorUuid )
			.AnyAsync( _grMeRepo );
		if ( !isOwner )
		{
			throw new NotAuthorizedException();
		}

		var membership = await _grMeRepo.GetGroupsMembersAsync()
			.Where( um => um.GroupId == groupId )
			.Where( um => um.UserUuid == targetUuid )
			.FirstOrDefaultAsync( _grMeRepo );
		if ( membership is null )
		{
			throw new NotFoundException();
		}

		if ( !membership.IsOwner )
		{
			throw new ConflictException();
		}

		membership.IsOwner = false;

		_ = await _grMeRepo.SaveAsync();
	}

}
