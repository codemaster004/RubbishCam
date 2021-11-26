using RubbishCam.Domain.Relations;

namespace RubbishCam.Data.Repositories;

public interface IGroupsMembersRepository : IRepository
{
	Task AddGroupMemberAsync( GroupMembersRelation group );
	IQueryable<GroupMembersRelation> GetGroupsMembers();
	Task<int> SaveAsync();
}

public class GroupsMembersRepository : IGroupsMembersRepository
{
	private readonly AppDbContext _dbContext;

	public GroupsMembersRepository( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	public IQueryable<GroupMembersRelation> GetGroupsMembers()
	{
		return _dbContext.GroupsMembers;
	}

	public async Task AddGroupMemberAsync( GroupMembersRelation group )
	{
		_ = await _dbContext.GroupsMembers.AddAsync( group );
	}

	public Task<int> SaveAsync()
	{
		return _dbContext.SaveChangesAsync();
	}
}

public static class GroupsMembersRepositoryExtensions
{
	public static IQueryable<GroupMembersRelation> FilterByUserUuid( this IQueryable<GroupMembersRelation> source, string uuid )
	{
		return source.Where( gm => gm.UserUuid == uuid );
	}
	public static IQueryable<GroupMembersRelation> FilterByOwnerships( this IQueryable<GroupMembersRelation> source, bool isOwnership )
	{
		return source.Where( gm => gm.IsOwner == isOwnership );
	}
	public static IQueryable<GroupMembersRelation> FilterGroupId( this IQueryable<GroupMembersRelation> source, int id )
	{
		return source.Where( gm => gm.GroupId == id );
	}
}