using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Moq;
using RubbishCam.Api.Exceptions;
using RubbishCam.Api.Services;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Dtos.Friendship;
using RubbishCam.Domain.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RubbishCam.UnitTests.Services;

public class FriendsServiceTests
{
	private readonly FriendsService _sut;
	private readonly Mock<IFriendshipsRepository> _friendshipsRepoMock = new( MockBehavior.Strict );
	private readonly Mock<IUserRepository> _userRepoMock = new( MockBehavior.Strict );
	private readonly Mock<ILogger<FriendsService>> _loggerMock = new();

	public FriendsServiceTests()
	{
		_sut = new( _friendshipsRepoMock.Object, _userRepoMock.Object, _loggerMock.Object );
	}

	[Fact]
	public async Task GetFriendshipsAsync_ShouldReturnAllFriendsOfUser_WhenUserExists()
	{
		var uuid = GenerateUuid();
		var initiated = Enumerable.Range( 0, 3 )
			.Select( x => new FriendshipModel( uuid, GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();
		var targeting = Enumerable.Range( 0, 4 )
			.Select( x => new FriendshipModel( GenerateUuid(), uuid ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.Union( initiated )
			.Union( targeting )
			.ToArray();

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.ToArrayAsync( It.IsAny<IQueryable<GetFriendshipDto>>() ) ).CallBase();

		// act
		var returned = await _sut.GetFriendshipsAsync( uuid );


		// assert
		Assert.NotNull( returned );
		Assert.Equal( initiated.Length + targeting.Length, returned.Length );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
	}

	[Fact]
	public async Task GetFriendshipsAsync_ShouldReturnEmpty_WhenUserDoesNotExist()
	{
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.ToArrayAsync( It.IsAny<IQueryable<GetFriendshipDto>>() ) ).CallBase();

		// act
		var returned = await _sut.GetFriendshipsAsync( GenerateUuid() );


		// assert
		Assert.NotNull( returned );
		Assert.Empty( returned );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
	}

	[Fact]
	public async Task GetAcceptedFriendshipsAsync_ShouldReturnAllAcceptedFriendsOfUser_WhenUserExists()
	{
		var uuid = GenerateUuid();
		var initiated = Enumerable.Range( 0, 4 )
			.Select( x => new FriendshipModel( uuid, GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();
		var targeting = Enumerable.Range( 0, 4 )
			.Select( x => new FriendshipModel( GenerateUuid(), uuid ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.Union( initiated )
			.Union( targeting )
			.ToArray();

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.ToArrayAsync( It.IsAny<IQueryable<GetFriendshipDto>>() ) ).CallBase();

		// act
		var returned = await _sut.GetAcceptedFriendshipsAsync( uuid );


		// assert
		Assert.NotNull( returned );
		Assert.Equal( initiated.Length / 2 + targeting.Length / 2, returned.Length );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
	}

	[Fact]
	public async Task GetAcceptedFriendshipsAsync_ShouldReturnEmpty_WhenUserDoesNotExist()
	{
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.ToArrayAsync( It.IsAny<IQueryable<GetFriendshipDto>>() ) ).CallBase();

		// act
		var returned = await _sut.GetAcceptedFriendshipsAsync( GenerateUuid() );


		// assert
		Assert.NotNull( returned );
		Assert.Empty( returned );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
	}

	[Fact]
	public async Task GetFriendshipAsync_ShouldReturnFriendship_WhenExists()
	{
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		int id = 317;
		FriendshipModel friendship = new( GenerateUuid(), GenerateUuid() ) { Id = id };
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<GetFriendshipDto>>() ) ).CallBase();

		// act
		var returned = await _sut.GetFriendshipAsync( id );


		// assert
		Assert.NotNull( returned );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		Assert.Equal( friendship.Id, returned!.Id );
		Assert.Equal( friendship.InitiatorUuid, returned.InitiatorUuid );
		Assert.Equal( friendship.TargetUuid, returned.TargetUuid );
		Assert.Equal( friendship.Accepted, returned.Accepted );
		Assert.Equal( friendship.Rejected, returned.Rejected );
	}

	[Fact]
	public async Task GetFriendshipAsync_ShouldReturnNothing_WhenDoesNotExist()
	{
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		int id = 317;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<GetFriendshipDto>>() ) ).CallBase();

		// act
		var returned = await _sut.GetFriendshipAsync( id );


		// assert
		Assert.Null( returned );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
	}

	[Fact]
	public async Task GetFriendshipAsync_ShouldReturnFriendship_WhenUsersExist()
	{
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiator = GenerateUuid();
		string target = GenerateUuid();
		FriendshipModel friendship = new( initiator, target );
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<GetFriendshipDto>>() ) ).CallBase();

		// act
		var returned = await _sut.GetFriendshipAsync( initiator, target );


		// assert
		Assert.NotNull( returned );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		Assert.Equal( friendship.Id, returned!.Id );
		Assert.Equal( friendship.InitiatorUuid, returned.InitiatorUuid );
		Assert.Equal( friendship.TargetUuid, returned.TargetUuid );
		Assert.Equal( friendship.Accepted, returned.Accepted );
		Assert.Equal( friendship.Rejected, returned.Rejected );
	}

	[Fact]
	public async Task GetFriendshipAsync_ShouldReturnFriendship_WhenDifferentOrder()
	{
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiator = GenerateUuid();
		string target = GenerateUuid();
		FriendshipModel friendship = new( initiator, target );
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<GetFriendshipDto>>() ) ).CallBase();

		// act
		var returned = await _sut.GetFriendshipAsync( target, initiator );


		// assert
		Assert.NotNull( returned );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		Assert.Equal( friendship.Id, returned!.Id );
		Assert.Equal( friendship.InitiatorUuid, returned.InitiatorUuid );
		Assert.Equal( friendship.TargetUuid, returned.TargetUuid );
		Assert.Equal( friendship.Accepted, returned.Accepted );
		Assert.Equal( friendship.Rejected, returned.Rejected );
	}

	[Fact]
	public async Task GetFriendshipAsync_ShouldReturnNothing_WhenUsersDoNotExist()
	{
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<GetFriendshipDto>>() ) ).CallBase();

		// act
		var returned = await _sut.GetFriendshipAsync( GenerateUuid(), GenerateUuid() );


		// assert
		Assert.Null( returned );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
	}

	[Fact]
	public async Task CreateFriendshipAsync_ShouldReturnFriendship_WhenUsersExist()
	{
		// arrange 
		#region users
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		UserModel initiator = new(
				initiatorUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		users[users.Length / 2 + 1] = initiator;

		string targetUuid = GenerateUuid();
		UserModel target = new(
				targetUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		users[users.Length / 2] = target;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.CountAsync( It.IsAny<IQueryable<UserModel>>() ) ).CallBase();

		#endregion

		#region friendships
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		FriendshipModel? passed = null;
		int called = 0;
		FriendshipModel? saved = null;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.AnyAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		_ = _friendshipsRepoMock.Setup( x => x.AddFriendshipsAsync( It.IsAny<FriendshipModel>() ) )
			.Callback( ( FriendshipModel x ) => { passed = x; called++; } )
			.Returns( Task.CompletedTask );

		_ = _friendshipsRepoMock.Setup( x => x.SaveAsync() )
			.Callback( () => saved = passed )
			.Returns( Task.FromResult( called ) );

		#endregion

		// act
		var returned = await _sut.CreateFriendshipAsync( initiatorUuid, targetUuid );

		// assert
		Assert.NotNull( returned );
		Assert.Equal( initiatorUuid, returned.InitiatorUuid );
		Assert.Equal( targetUuid, returned.TargetUuid );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_friendshipsRepoMock.Verify( x => x.AddFriendshipsAsync( It.IsAny<FriendshipModel>() ), Times.Once );
		_friendshipsRepoMock.Verify( x => x.SaveAsync(), Times.Once );
		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );

		Assert.NotNull( saved );
		Assert.Equal( initiator.Uuid, saved!.InitiatorUuid );
		Assert.Equal( target.Uuid, saved.TargetUuid );
		Assert.False( saved.Accepted );
		Assert.False( saved.Rejected );
	}

	[Fact]
	public async Task CreateFriendshipAsync_ShouldThrow_WhenInitiatorDoesNotExist()
	{
		// arrange 
		#region users
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		string targetUuid = GenerateUuid();
		UserModel target = new(
				targetUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		users[users.Length / 2] = target;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.CountAsync( It.IsAny<IQueryable<UserModel>>() ) ).CallBase();

		#endregion

		#region friendships
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.AnyAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		#endregion

		// act
		var act = () => _sut.CreateFriendshipAsync( GenerateUuid(), targetUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.AtMostOnce );
		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
	}

	[Fact]
	public async Task CreateFriendshipAsync_ShouldThrow_WhenTargetDoesNotExist()
	{
		// arrange 
		#region users
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		UserModel initiator = new(
				initiatorUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		users[users.Length / 2] = initiator;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.CountAsync( It.IsAny<IQueryable<UserModel>>() ) ).CallBase();

		#endregion

		#region friendships
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.AnyAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		#endregion

		// act
		var act = () => _sut.CreateFriendshipAsync( initiatorUuid, GenerateUuid() );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.AtMostOnce );
		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
	}

	[Fact]
	public async Task CreateFriendshipAsync_ShouldThrow_WhenFriendshipAlreadyExist()
	{
		// arrange 
		#region users
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		UserModel initiator = new(
				initiatorUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		users[users.Length / 2 + 1] = initiator;

		string targetUuid = GenerateUuid();
		UserModel target = new(
				targetUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		users[users.Length / 2] = target;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.CountAsync( It.IsAny<IQueryable<UserModel>>() ) ).CallBase();

		#endregion

		#region friendships
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		FriendshipModel friendship = new( initiatorUuid, targetUuid );
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.AnyAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		#endregion

		// act
		var act = () => _sut.CreateFriendshipAsync( initiatorUuid, targetUuid );

		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.AtMostOnce );
		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
	}

	[Fact]
	public async Task CreateFriendshipAsync_ShouldThrow_WhenReversedFriendshipAlreadyExist()
	{
		// arrange 
		#region users
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		UserModel initiator = new(
				initiatorUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		users[users.Length / 2 + 1] = initiator;

		string targetUuid = GenerateUuid();
		UserModel target = new(
				targetUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		users[users.Length / 2] = target;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.CountAsync( It.IsAny<IQueryable<UserModel>>() ) ).CallBase();

		#endregion

		#region friendships
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		FriendshipModel friendship = new( initiatorUuid, targetUuid );
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.AnyAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		#endregion

		// act
		var act = () => _sut.CreateFriendshipAsync( targetUuid, initiatorUuid );

		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.AtMostOnce );
		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
	}

	[Fact]
	public async Task AcceptFriendshipAsync_ShouldSucceed_WhenExists()
	{
		// arrange 
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id };
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		_ = _friendshipsRepoMock.Setup( x => x.SaveAsync() ).Returns( () => Task.FromResult( 1 ) );


		// act
		await _sut.AcceptFriendshipAsync( id );

		// assert
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_friendshipsRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.Equal( id, friendship.Id );
		Assert.Equal( initiatorUuid, friendship.InitiatorUuid );
		Assert.Equal( targetUuid, friendship.TargetUuid );

		Assert.True( friendship.Accepted );
		Assert.False( friendship.Rejected );

	}

	[Fact]
	public async Task AcceptFriendshipAsync_ShouldSucceed_WhenAlreadyAccepted()
	{
		// arrange
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id, Accepted = true };
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		_ = _friendshipsRepoMock.Setup( x => x.SaveAsync() ).Returns( () => Task.FromResult( 1 ) );


		// act
		await _sut.AcceptFriendshipAsync( id );

		// assert
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_friendshipsRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.Equal( id, friendship.Id );
		Assert.Equal( initiatorUuid, friendship.InitiatorUuid );
		Assert.Equal( targetUuid, friendship.TargetUuid );

		Assert.True( friendship.Accepted );
		Assert.False( friendship.Rejected );

	}

	[Fact]
	public async Task AcceptFriendshipAsync_ShouldSucceed_WhenRejected()
	{
		// arrange
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id, Rejected = true };
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		_ = _friendshipsRepoMock.Setup( x => x.SaveAsync() ).Returns( () => Task.FromResult( 1 ) );


		// act
		await _sut.AcceptFriendshipAsync( id );

		// assert
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_friendshipsRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.Equal( id, friendship.Id );
		Assert.Equal( initiatorUuid, friendship.InitiatorUuid );
		Assert.Equal( targetUuid, friendship.TargetUuid );

		Assert.True( friendship.Accepted );
		Assert.False( friendship.Rejected );

	}

	[Fact]
	public async Task AcceptFriendshipAsync_ShouldThrow_WhenDoesNotExist()
	{
		// arrange
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		int id = 317;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();


		// act
		var act = () => _sut.AcceptFriendshipAsync( id );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );

	}

	[Fact]
	public async Task RejectFriendshipAsync_ShouldSucceed_WhenExists()
	{
		// arrange 
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id };
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		_ = _friendshipsRepoMock.Setup( x => x.SaveAsync() ).Returns( () => Task.FromResult( 1 ) );


		// act
		await _sut.RejectFriendshipAsync( id );

		// assert
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_friendshipsRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.Equal( id, friendship.Id );
		Assert.Equal( initiatorUuid, friendship.InitiatorUuid );
		Assert.Equal( targetUuid, friendship.TargetUuid );

		Assert.True( friendship.Rejected );
		Assert.False( friendship.Accepted );
	}

	[Fact]
	public async Task RejectFriendshipAsync_ShouldSucceed_WhenAlreadyRejected()
	{
		// arrange 
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id, Rejected = true };
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		_ = _friendshipsRepoMock.Setup( x => x.SaveAsync() ).Returns( () => Task.FromResult( 1 ) );


		// act
		await _sut.RejectFriendshipAsync( id );

		// assert
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_friendshipsRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.Equal( id, friendship.Id );
		Assert.Equal( initiatorUuid, friendship.InitiatorUuid );
		Assert.Equal( targetUuid, friendship.TargetUuid );

		Assert.True( friendship.Rejected );
		Assert.False( friendship.Accepted );
	}

	[Fact]
	public async Task RejectFriendshipAsync_ShouldThrow_WhenAccepted()
	{
		// arrange
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id, Accepted = true };
		friendships[friendships.Length / 2] = friendship;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();


		// act
		var act = () => _sut.RejectFriendshipAsync( id );

		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );

	}

	[Fact]
	public async Task RejectFriendshipAsync_ShouldThrow_WhenDoesNotExist()
	{
		// arrange
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		int id = 317;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();


		// act
		var act = () => _sut.RejectFriendshipAsync( id );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
	}

	[Fact]
	public async Task DeleteFriendshipAsync_ShouldSucceed_WhenExists()
	{
		// arrange 
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id };
		friendships[friendships.Length / 2] = friendship;

		FriendshipModel? passed = null;
		int called = 0;
		FriendshipModel? saved = null;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();

		_ = _friendshipsRepoMock.Setup( x => x.RemoveFriendshipsAsync( It.IsAny<FriendshipModel>() ) )
			.Callback( ( FriendshipModel x ) => { passed = x; called++; } )
			.Returns( Task.CompletedTask );

		_ = _friendshipsRepoMock.Setup( x => x.SaveAsync() )
			.Callback( () => saved = passed )
			.Returns( () => Task.FromResult( called ) );


		// act
		await _sut.DeleteFriendshipAsync( id );

		// assert
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_friendshipsRepoMock.Verify( x => x.RemoveFriendshipsAsync( It.IsAny<FriendshipModel>() ), Times.Once );
		_friendshipsRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.NotNull( saved );
		Assert.Equal( id, saved!.Id );

	}

	[Fact]
	public async Task DeleteFriendshipAsync_ShouldThrow_WhenDoesNotExist()
	{
		// arrange 
		var friendships = Enumerable.Range( 0, 10 )
			.Select( x => new FriendshipModel( GenerateUuid(), GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		_ = _friendshipsRepoMock.Setup( x => x.GetFriendships() )
			.Returns( friendships.ToArray().AsQueryable() );

		_ = _friendshipsRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<FriendshipModel>>() ) ).CallBase();


		// act
		var act = () => _sut.DeleteFriendshipAsync( id );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );

	}




	private static string GenerateUuid()
	{
		return Base64UrlTextEncoder.Encode( Guid.NewGuid().ToByteArray() );
	}
	private static readonly SHA512 sha = SHA512.Create();
	private static string GenerateHash()
	{
		return Hash( Faker.Lorem.Paragraph() );
	}
	private static string Hash( string source )
	{
		byte[] buffer = Encoding.UTF8.GetBytes( source );
		return Convert.ToBase64String( sha.ComputeHash( buffer ) );
	}
}
