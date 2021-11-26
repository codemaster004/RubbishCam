using RubbishCam.Domain.Models;

namespace RubbishCam.Data.Repositories;

public interface IGroupsRepository : IRepository
{
	IQueryable<GroupModel> GetGroups();
	Task AddGroupAsync( GroupModel group );
	Task<int> SaveAsync();
}

public class GroupsRepository : IGroupsRepository
{
	private readonly AppDbContext _dbContext;

	public GroupsRepository( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	public IQueryable<GroupModel> GetGroups()
	{
		return _dbContext.Groups;
	}

	public async Task AddGroupAsync( GroupModel group )
	{
		_ = await _dbContext.Groups.AddAsync( group );
	}

	public Task<int> SaveAsync()
	{
		return _dbContext.SaveChangesAsync();
	}

}

public static class GroupsRepositoryExtensions
{
	public static IQueryable<GroupModel> FilterById( this IQueryable<GroupModel> source, int id )
	{
		return source.Where( gm => gm.Id == id );
	}
}
