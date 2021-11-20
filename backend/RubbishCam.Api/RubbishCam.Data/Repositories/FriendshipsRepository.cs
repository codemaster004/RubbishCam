using RubbishCam.Domain.Models;

namespace RubbishCam.Data.Repositories;

public interface IFriendshipsRepository : IRepository
{
	IQueryable<FriendshipModel> GetFriendships();
	Task AddFriendshipsAsync( FriendshipModel friendship );
	Task RemoveFriendshipsAsync( FriendshipModel friendship );
	Task<int> SaveAsync();
}

public class FriendshipsRepository : IFriendshipsRepository
{
	private readonly AppDbContext _dbContext;

	public FriendshipsRepository( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	public IQueryable<FriendshipModel> GetFriendships()
	{
		return _dbContext.Friendships;
	}

	public async Task AddFriendshipsAsync( FriendshipModel friendship )
	{
		_ = await _dbContext.Friendships.AddAsync( friendship );
	}

	public async Task RemoveFriendshipsAsync( FriendshipModel friendship )
	{
		await Task.CompletedTask;
		_ = _dbContext.Friendships.Remove( friendship );
	}

	public Task<int> SaveAsync()
	{
		return _dbContext.SaveChangesAsync();
	}
}

public static class FriendshipsRepositoryExtensions
{
	public static IQueryable<FriendshipModel> FilterById( this IQueryable<FriendshipModel> source, int id )
	{
		return source.Where( f => f.Id == id );
	}
	public static IQueryable<FriendshipModel> FilterByAnySide( this IQueryable<FriendshipModel> source, string uuid )
	{
		return source.Where( f => f.InitiatorUuid == uuid || f.TargetUuid == uuid );
	}
	public static IQueryable<FriendshipModel> FilterByAccepted( this IQueryable<FriendshipModel> source, bool accepted )
	{
		return source.Where( f => f.Accepted == accepted );
	}
	public static IQueryable<FriendshipModel> FilterByPair( this IQueryable<FriendshipModel> source, string firstUuid, string secondUuid )
	{
		return source.Where( f =>
				( f.InitiatorUuid == firstUuid
				&& f.TargetUuid == secondUuid )
				|| ( f.InitiatorUuid == secondUuid
				&& f.TargetUuid == firstUuid ) );
	}
}
