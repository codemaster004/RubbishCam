using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Dtos.Group;

namespace RubbishCam.Api.Services;

public interface IGroupsService
{
	Task<GetGroupDto[]> GetGroups();
	Task<GetGroupDto[]> GetGroups( string userUuid );
	Task<GetGroupDto[]> GetOwnedGroups( string userUuid );
	Task<GetGroupDetailsDto?> GetGroup( int id, string userUuid );
	Task<GetGroupDetailsDto> CreateGroup( CreateGroupDto dto, string userUuid );
}

public class GroupsService : IGroupsService
{
	private readonly IGroupsRepository _groupsRepo;
	private readonly IGroupsMembersRepository _grMeRepo;
	private readonly IUserRepository _userRepo;

	public GroupsService( IGroupsRepository groupsRepo, IGroupsMembersRepository grMeRepo, IUserRepository userRepo )
	{
		_groupsRepo = groupsRepo;
		_grMeRepo = grMeRepo;
		_userRepo = userRepo;
	}

	public Task<GetGroupDto[]> GetGroups()
	{
		return _groupsRepo.GetGroups()
			.Select( GetGroupDto.FromGroupExp )
			.ToArrayAsync( _groupsRepo );
	}

	public Task<GetGroupDto[]> GetGroups( string userUuid )
	{
		return _grMeRepo.GetGroupsMembers()
			.FilterByUserUuid( userUuid )
			.Select( gm => gm.Group! )
			.Select( GetGroupDto.FromGroupExp )
			.ToArrayAsync( _grMeRepo );
	}

	public Task<GetGroupDto[]> GetOwnedGroups( string userUuid )
	{
		return _grMeRepo.GetGroupsMembers()
			.FilterByUserUuid( userUuid )
			.FilterByOwnerships( true )
			.Select( gm => gm.Group! )
			.Select( GetGroupDto.FromGroupExp )
			.ToArrayAsync( _grMeRepo );
	}

	public async Task<GetGroupDetailsDto?> GetGroup( int id, string userUuid )
	{
		var group = await _groupsRepo.GetGroups()
			.FilterById( id )
			.Select( GetGroupDetailsDto.FromGroupExp )
			.FirstOrDefaultAsync( _groupsRepo );

		var permited = await _grMeRepo.GetGroupsMembers()
			.FilterGroupId( id )
			.FilterByUserUuid( userUuid )
			.AnyAsync( _grMeRepo );

		if ( !permited )
		{
			throw new NotAuthorizedException();
		}

		return group;
	}

	public async Task<GetGroupDetailsDto> CreateGroup( CreateGroupDto dto, string userUuid )
	{
		var user = await _userRepo.GetUsers()
			.FilterById( userUuid )
			.FirstOrDefaultAsync( _userRepo );
		if ( user is null )
		{
			throw new NotFoundException();
		}

		var group = dto.ToGroup( user.Uuid );

		await _groupsRepo.AddGroupAsync( group );
		_ = await _groupsRepo.SaveAsync();

		return GetGroupDetailsDto.FromGroup( group );
	}
}
