using Microsoft.EntityFrameworkCore;
using RubbishCam.Data;
using RubbishCam.Domain.Dtos.Friendship;
using RubbishCam.Domain.Models;

namespace RubbishCam.Api.Services;

public interface IFriendsService
{
	Task<GetFriendshipDto[]> GetFriendshipsAsync( string uuid );
	Task<GetFriendshipDto[]> GetAcceptedFriendshipsAsync( string uuid );
	Task<GetFriendshipDto?> GetFriendshipAsync( string firstUuid, string secondUuid );
	Task<GetFriendshipDto?> GetFriendshipAsync( int id );
	Task<GetFriendshipDto> CreateFriendshipAsync( string initiatorUuid, string targetUuid );
	Task AcceptFriendshipAsync( int id );
	Task RejectFriendshipAsync( int id );
	Task DeleteFriendshipAsync( int id );
}

public class FriendsService : IFriendsService
{
	private readonly AppDbContext _dbContext;

	public FriendsService( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	public async Task<GetFriendshipDto[]> GetFriendshipsAsync( string uuid )
	{
		return await _dbContext.Friendships
			.Where( f => f.InitiatorUuid == uuid || f.TargetUuid == uuid )
			.Select( GetFriendshipDto.FromFriendshipExp )
			.ToArrayAsync();
	}
	public async Task<GetFriendshipDto[]> GetAcceptedFriendshipsAsync( string uuid )
	{
		return await _dbContext.Friendships
			.Where( f => f.InitiatorUuid == uuid || f.TargetUuid == uuid )
			.Where( f => f.Accepted )
			.Select( GetFriendshipDto.FromFriendshipExp )
			.ToArrayAsync();
	}
	public async Task<GetFriendshipDto?> GetFriendshipAsync( int id )
	{
		return await _dbContext.Friendships
			.Where( f => f.Id == id )
			.Select( GetFriendshipDto.FromFriendshipExp )
			.FirstOrDefaultAsync();
	}
	public async Task<GetFriendshipDto?> GetFriendshipAsync( string firstUuid, string secondUuid )
	{
		return await _dbContext.Friendships
			.Where( f =>
				( f.InitiatorUuid == firstUuid
				&& f.TargetUuid == secondUuid )
				|| ( f.InitiatorUuid == secondUuid
				&& f.TargetUuid == firstUuid ) )
			.Select( GetFriendshipDto.FromFriendshipExp )
			.FirstOrDefaultAsync();
	}

	public async Task<GetFriendshipDto> CreateFriendshipAsync( string initiatorUuid, string targetUuid )
	{
		var usersExist = await _dbContext.Users.Where( u => u.Uuid == initiatorUuid || u.Uuid == targetUuid ).CountAsync() > 2;

		if ( !usersExist )
		{
			throw new NotFoundException();
		}

		var exists = await _dbContext.Friendships.Where( f =>
									 ( f.InitiatorUuid == initiatorUuid
									 && f.TargetUuid == targetUuid )
									 || ( f.InitiatorUuid == targetUuid
									 && f.TargetUuid == initiatorUuid ) ).AnyAsync();

		if ( exists )
		{
			throw new ConflictException();
		}

		FriendshipModel friendship = new( initiatorUuid, targetUuid );

		_ = await _dbContext.Friendships.AddAsync( friendship );
		_ = await _dbContext.SaveChangesAsync();

		return GetFriendshipDto.FromFriendship( friendship );
	}

	public async Task AcceptFriendshipAsync( int id )
	{
		var friendship = await _dbContext.Friendships.FindAsync( id );

		if ( friendship is null )
		{
			throw new NotFoundException();
		}

		friendship.Rejected = false;
		friendship.Accepted = true;

		_ = await _dbContext.SaveChangesAsync();
	}
	public async Task RejectFriendshipAsync( int id )
	{
		var friendship = await _dbContext.Friendships.FindAsync( id );

		if ( friendship is null )
		{
			throw new NotFoundException();
		}
		if ( friendship.Accepted )
		{
			throw new ConflictException();
		}

		friendship.Rejected = true;

		_ = await _dbContext.SaveChangesAsync();
	}
	public async Task DeleteFriendshipAsync( int id )
	{
		var friendship = await _dbContext.Friendships.FindAsync( id );

		if ( friendship is null )
		{
			throw new NotFoundException();
		}

		_ = _dbContext.Friendships.Remove( friendship );
		_ = await _dbContext.SaveChangesAsync();
	}

}
