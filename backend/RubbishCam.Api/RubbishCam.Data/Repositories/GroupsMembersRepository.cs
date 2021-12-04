using RubbishCam.Domain.Relations;

namespace RubbishCam.Data.Repositories;

public interface IGroupsMembersRepository : IRepository
{
	Task AddGroupMemberAsync( GroupMembershipRelation membership );
	IQueryable<GroupMembershipRelation> GetGroupsMembersAsync();
	Task RemoveGroupMemberAsync( GroupMembershipRelation membership );
	Task<int> SaveAsync();
}

public class GroupsMembersRepository : IGroupsMembersRepository
{
	private readonly AppDbContext _dbContext;

	public GroupsMembersRepository( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	public IQueryable<GroupMembershipRelation> GetGroupsMembersAsync()
	{
		return _dbContext.GroupsMemberships;
	}

	public async Task AddGroupMemberAsync( GroupMembershipRelation membership )
	{
		_ = await _dbContext.GroupsMemberships.AddAsync( membership );
	}

	public async Task RemoveGroupMemberAsync( GroupMembershipRelation membership )
	{
		await Task.CompletedTask;
		_ = _dbContext.GroupsMemberships.Remove( membership );
	}

	public Task<int> SaveAsync()
	{
		return _dbContext.SaveChangesAsync();
	}
}

public static class GroupsMembersRepositoryExtensions
{
	public static IQueryable<GroupMembershipRelation> FilterByUserUuid( this IQueryable<GroupMembershipRelation> source, string uuid )
	{
		return source.Where( gm => gm.UserUuid == uuid );
	}
	public static IQueryable<GroupMembershipRelation> FilterByOwnerships( this IQueryable<GroupMembershipRelation> source, bool isOwnership )
	{
		return source.Where( gm => gm.IsOwner == isOwnership );
	}
	public static IQueryable<GroupMembershipRelation> FilterGroupId( this IQueryable<GroupMembershipRelation> source, int id )
	{
		return source.Where( gm => gm.GroupId == id );
	}
}