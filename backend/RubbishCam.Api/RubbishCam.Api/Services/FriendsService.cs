using Microsoft.EntityFrameworkCore;
using RubbishCam.Data;
using RubbishCam.Data.Repositories;
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
	//private readonly AppDbContext _dbContext;
	private readonly IFriendshipsRepository _friendshipsRepo;
	private readonly IUserRepository _usersRepo;
	private readonly ILogger<FriendsService> _logger;

	public FriendsService( /*AppDbContext dbContext,*/ IFriendshipsRepository friendshipRepo, IUserRepository usersRepo, ILogger<FriendsService> logger )
	{
		//_dbContext = dbContext;
		_friendshipsRepo = friendshipRepo;
		_usersRepo = usersRepo;
		_logger = logger;
	}

	public async Task<GetFriendshipDto[]> GetFriendshipsAsync( string uuid )
	{
		//return await _dbContext.Friendships
		//	.Where( f => f.InitiatorUuid == uuid || f.TargetUuid == uuid )
		//	.Select( GetFriendshipDto.FromFriendshipExp )
		//	.ToArrayAsync();
		return await _friendshipsRepo.GetFriendships()
			.FilterByAnySide( uuid )
			.Select( GetFriendshipDto.FromFriendshipExp )
			.ToArrayAsync( _friendshipsRepo );
	}
	public async Task<GetFriendshipDto[]> GetAcceptedFriendshipsAsync( string uuid )
	{
		//return await _dbContext.Friendships
		//	.Where( f => f.InitiatorUuid == uuid || f.TargetUuid == uuid )
		//	.Where( f => f.Accepted )
		//	.Select( GetFriendshipDto.FromFriendshipExp )
		//	.ToArrayAsync();
		return await _friendshipsRepo.GetFriendships()
			.FilterByAnySide( uuid )
			.FilterByAccepted( true )
			.Select( GetFriendshipDto.FromFriendshipExp )
			.ToArrayAsync( _friendshipsRepo );
	}
	public async Task<GetFriendshipDto?> GetFriendshipAsync( int id )
	{
		//return await _dbContext.Friendships
		//	.Where( f => f.Id == id )
		//	.Select( GetFriendshipDto.FromFriendshipExp )
		//	.FirstOrDefaultAsync();
		return await _friendshipsRepo.GetFriendships()
			.FilterById( id )
			.Select( GetFriendshipDto.FromFriendshipExp )
			.FirstOrDefaultAsync( _friendshipsRepo );
	}
	public async Task<GetFriendshipDto?> GetFriendshipAsync( string firstUuid, string secondUuid )
	{
		//return await _dbContext.Friendships
		//	.Where( f =>
		//		( f.InitiatorUuid == firstUuid
		//		&& f.TargetUuid == secondUuid )
		//		|| ( f.InitiatorUuid == secondUuid
		//		&& f.TargetUuid == firstUuid ) )
		//	.Select( GetFriendshipDto.FromFriendshipExp )
		//	.FirstOrDefaultAsync();
		return await _friendshipsRepo.GetFriendships()
			.FilterByPair( firstUuid, secondUuid )
			.Select( GetFriendshipDto.FromFriendshipExp )
			.FirstOrDefaultAsync( _friendshipsRepo );
	}

	public async Task<GetFriendshipDto> CreateFriendshipAsync( string initiatorUuid, string targetUuid )
	{
		//var usersExist = await _dbContext.Users.Where( u => u.Uuid == initiatorUuid || u.Uuid == targetUuid ).CountAsync() > 2;
		var usersExist = await _usersRepo.GetUsers()
			.Where( u => u.Uuid == initiatorUuid || u.Uuid == targetUuid )
			.CountAsync( _usersRepo ) >= 2;

		if ( !usersExist )
		{
			throw new NotFoundException();
		}

		//var exists = await _dbContext.Friendships.Where( f =>
		//							 ( f.InitiatorUuid == initiatorUuid
		//							 && f.TargetUuid == targetUuid )
		//							 || ( f.InitiatorUuid == targetUuid
		//							 && f.TargetUuid == initiatorUuid ) ).AnyAsync();
		var exists = await _friendshipsRepo.GetFriendships()
			 .FilterByPair( targetUuid, initiatorUuid )
			 .AnyAsync( _friendshipsRepo );

		if ( exists )
		{
			throw new ConflictException();
		}

		FriendshipModel friendship = new( initiatorUuid, targetUuid );

		//_ = await _dbContext.Friendships.AddAsync( friendship );
		await _friendshipsRepo.AddFriendshipsAsync( friendship );
		//_ = await _dbContext.SaveChangesAsync();
		_ = await _friendshipsRepo.SaveAsync();

		return GetFriendshipDto.FromFriendship( friendship );
	}

	public async Task AcceptFriendshipAsync( int id )
	{
		//var friendship = await _dbContext.Friendships.FindAsync( id );
		var friendship = await _friendshipsRepo.GetFriendships()
			.FilterById( id )
			.FirstOrDefaultAsync( _friendshipsRepo );

		if ( friendship is null )
		{
			throw new NotFoundException();
		}

		friendship.Rejected = false;
		friendship.Accepted = true;

		//_ = await _dbContext.SaveChangesAsync();
		_ = await _friendshipsRepo.SaveAsync();
	}
	public async Task RejectFriendshipAsync( int id )
	{
		//var friendship = await _dbContext.Friendships.FindAsync( id );
		var friendship = await _friendshipsRepo.GetFriendships()
			.FilterById( id )
			.FirstOrDefaultAsync( _friendshipsRepo );

		if ( friendship is null )
		{
			throw new NotFoundException();
		}
		if ( friendship.Accepted )
		{
			throw new ConflictException();
		}

		friendship.Rejected = true;

		//_ = await _dbContext.SaveChangesAsync();
		_ = await _friendshipsRepo.SaveAsync();
	}
	public async Task DeleteFriendshipAsync( int id )
	{
		//var friendship = await _dbContext.Friendships.FindAsync( id );
		var friendship = await _friendshipsRepo.GetFriendships()
			.FilterById( id )
			.FirstOrDefaultAsync( _friendshipsRepo );

		if ( friendship is null )
		{
			throw new NotFoundException();
		}

		//_ = _dbContext.Friendships.Remove( friendship );
		await _friendshipsRepo.RemoveFriendshipsAsync( friendship );
		//_ = await _dbContext.SaveChangesAsync();
		_ = await _friendshipsRepo.SaveAsync();
	}

}
