using RubbishCam.Domain.Relations;

namespace RubbishCam.Data.Repositories;

public interface IFriendshipsRepository : IRepository
{
	IQueryable<FriendshipRelation> GetFriendships();
	Task AddFriendshipsAsync( FriendshipRelation friendship );
	Task RemoveFriendshipsAsync( FriendshipRelation friendship );
	Task<int> SaveAsync();
}

public class FriendshipsRepository : IFriendshipsRepository
{
	private readonly AppDbContext _dbContext;

	public FriendshipsRepository( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	public IQueryable<FriendshipRelation> GetFriendships()
	{
		return _dbContext.Friendships;
	}

	public async Task AddFriendshipsAsync( FriendshipRelation friendship )
	{
		_ = await _dbContext.Friendships.AddAsync( friendship );
	}

	public async Task RemoveFriendshipsAsync( FriendshipRelation friendship )
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
	public static IQueryable<FriendshipRelation> FilterById( this IQueryable<FriendshipRelation> source, int id )
	{
		return source.Where( f => f.Id == id );
	}
	public static IQueryable<FriendshipRelation> FilterByAnySide( this IQueryable<FriendshipRelation> source, string uuid )
	{
		return source.Where( f => f.InitiatorUuid == uuid || f.TargetUuid == uuid );
	}
	public static IQueryable<FriendshipRelation> FilterByAccepted( this IQueryable<FriendshipRelation> source, bool accepted )
	{
		return source.Where( f => f.Accepted == accepted );
	}
	public static IQueryable<FriendshipRelation> FilterByPair( this IQueryable<FriendshipRelation> source, string firstUuid, string secondUuid )
	{
		return source.Where( f =>
				( f.InitiatorUuid == firstUuid
				&& f.TargetUuid == secondUuid )
				|| ( f.InitiatorUuid == secondUuid
				&& f.TargetUuid == firstUuid ) );
	}
}
