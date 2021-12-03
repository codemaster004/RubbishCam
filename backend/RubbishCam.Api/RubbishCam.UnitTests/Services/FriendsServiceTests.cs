using Microsoft.Extensions.Logging;
using Moq;
using RubbishCam.Api.Exceptions;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.Friendship;
using RubbishCam.Domain.Models;
using RubbishCam.UnitTests.Mocks.Repos;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static RubbishCam.UnitTests.Helper;

namespace RubbishCam.UnitTests.Services;

public class FriendsServiceTests
{
	private readonly FriendsService _sut;
	private readonly UsersRepoMock _usersRepoMock = new( MockBehavior.Strict );
	private readonly FriendshipsRepoMock _friendshipsRepoMock = new( MockBehavior.Strict );
	private readonly Mock<ILogger<FriendsService>> _loggerMock = new();

	public FriendsServiceTests()
	{
		_usersRepoMock.InitializeUsers();
		_friendshipsRepoMock.InitializeFriendships();

		_sut = new( _friendshipsRepoMock.Object, _usersRepoMock.Object, _loggerMock.Object );
	}

	[Fact]
	public async Task GetFriendshipsAsync_ShouldReturnAllFriendsOfUser_WhenUserExists()
	{
		// arrange
		var uuid = GenerateUuid();
		var initiated = Enumerable.Range( 0, 3 )
			.Select( x => new FriendshipModel( uuid, GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();
		var targeting = Enumerable.Range( 0, 4 )
			.Select( x => new FriendshipModel( GenerateUuid(), uuid ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		_friendshipsRepoMock.AddManyKnownFriendships( targeting );
		_friendshipsRepoMock.AddManyKnownFriendships( initiated );

		_friendshipsRepoMock.SetupGet();

		_friendshipsRepoMock.SetupToArray<GetFriendshipDto>();

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
		// arrange
		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupToArray<GetFriendshipDto>();

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
		// arrange
		var uuid = GenerateUuid();
		var initiated = Enumerable.Range( 0, 4 )
			.Select( x => new FriendshipModel( uuid, GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();
		var targeting = Enumerable.Range( 0, 4 )
			.Select( x => new FriendshipModel( GenerateUuid(), uuid ) { Accepted = ( x & 1 ) > 0 } )
			.ToArray();

		_friendshipsRepoMock.AddManyKnownFriendships( targeting );
		_friendshipsRepoMock.AddManyKnownFriendships( initiated );

		_friendshipsRepoMock.SetupGet();

		_friendshipsRepoMock.SetupToArray<GetFriendshipDto>();

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
		// arrange
		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupToArray<GetFriendshipDto>();

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
		// arrange
		int id = 317;
		FriendshipModel friendship = new( GenerateUuid(), GenerateUuid() ) { Id = id };
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();

		_friendshipsRepoMock.SetupFirstOrDefault<GetFriendshipDto>();

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
		// arrange
		int id = 317;

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupFirstOrDefault<GetFriendshipDto>();

		// act
		var returned = await _sut.GetFriendshipAsync( id );


		// assert
		Assert.Null( returned );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
	}

	[Fact]
	public async Task GetFriendshipAsync_ShouldReturnFriendship_WhenUsersExist()
	{
		// arrange
		string initiator = GenerateUuid();
		string target = GenerateUuid();
		FriendshipModel friendship = new( initiator, target );
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();

		_friendshipsRepoMock.SetupFirstOrDefault<GetFriendshipDto>();

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
		// arrange
		string initiator = GenerateUuid();
		string target = GenerateUuid();
		FriendshipModel friendship = new( initiator, target );
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();

		_friendshipsRepoMock.SetupFirstOrDefault<GetFriendshipDto>();

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
		// arrange
		_friendshipsRepoMock.SetupGet();

		_friendshipsRepoMock.SetupFirstOrDefault<GetFriendshipDto>();

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
		string initiatorUuid = GenerateUuid();
		UserModel initiator = new(
				initiatorUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );

		string targetUuid = GenerateUuid();
		UserModel target = new(
				targetUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );


		_ = _usersRepoMock.AddKnownUser( initiator );
		_ = _usersRepoMock.AddKnownUser( target );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupCount<UserModel>();


		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupAny<FriendshipModel>();

		_friendshipsRepoMock.SetupAdd();


		// act
		var returned = await _sut.CreateFriendshipAsync( initiatorUuid, targetUuid );

		// assert
		Assert.NotNull( returned );
		Assert.Equal( initiatorUuid, returned.InitiatorUuid );
		Assert.Equal( targetUuid, returned.TargetUuid );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_friendshipsRepoMock.Verify( x => x.AddFriendshipsAsync( It.IsAny<FriendshipModel>() ), Times.Once );
		_friendshipsRepoMock.Verify( x => x.SaveAsync(), Times.Once );
		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );

		var saved = Assert.Single( _friendshipsRepoMock.AddSaved );

		Assert.Equal( initiator.Uuid, saved.InitiatorUuid );
		Assert.Equal( target.Uuid, saved.TargetUuid );
		Assert.False( saved.Accepted );
		Assert.False( saved.Rejected );
	}

	[Fact]
	public async Task CreateFriendshipAsync_ShouldThrow_WhenInitiatorDoesNotExist()
	{
		// arrange
		string targetUuid = GenerateUuid();
		UserModel target = new(
				targetUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		_ = _usersRepoMock.AddKnownUser( target );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupCount<UserModel>();

		// act
		var act = () => _sut.CreateFriendshipAsync( GenerateUuid(), targetUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
	}

	[Fact]
	public async Task CreateFriendshipAsync_ShouldThrow_WhenTargetDoesNotExist()
	{
		// arrange
		string initiatorUuid = GenerateUuid();
		UserModel initiator = new(
				initiatorUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		_ = _usersRepoMock.AddKnownUser( initiator );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupCount<UserModel>();

		// act
		var act = () => _sut.CreateFriendshipAsync( initiatorUuid, GenerateUuid() );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
	}

	[Fact]
	public async Task CreateFriendshipAsync_ShouldThrow_WhenFriendshipAlreadyExist()
	{
		// arrange
		string initiatorUuid = GenerateUuid();
		UserModel initiator = new(
				initiatorUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		_ = _usersRepoMock.AddKnownUser( initiator );

		string targetUuid = GenerateUuid();
		UserModel target = new(
				targetUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		_ = _usersRepoMock.AddKnownUser( target );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupCount<UserModel>();


		FriendshipModel friendship = new( initiatorUuid, targetUuid );
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupAny<FriendshipModel>();

		// act
		var act = () => _sut.CreateFriendshipAsync( initiatorUuid, targetUuid );

		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
	}

	[Fact]
	public async Task CreateFriendshipAsync_ShouldThrow_WhenReversedFriendshipAlreadyExist()
	{
		// arrange
		string initiatorUuid = GenerateUuid();
		UserModel initiator = new(
				initiatorUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		_ = _usersRepoMock.AddKnownUser( initiator );

		string targetUuid = GenerateUuid();
		UserModel target = new(
				targetUuid,
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() );
		_ = _usersRepoMock.AddKnownUser( target );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupCount<UserModel>();


		FriendshipModel friendship = new( initiatorUuid, targetUuid );
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupAny<FriendshipModel>();

		// act
		var act = () => _sut.CreateFriendshipAsync( targetUuid, initiatorUuid );

		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );

		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.AtMostOnce );
		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
	}

	[Fact]
	public async Task AcceptFriendshipAsync_ShouldSucceed_WhenExists()
	{
		// arrange
		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id };
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();

		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

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
		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id, Accepted = true };
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();

		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

		// act
		await _sut.AcceptFriendshipAsync( id );

		// assert
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );

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
		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id, Rejected = true };
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();

		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

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
		int id = 317;

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

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
		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id };
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

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
		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id, Rejected = true };
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

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
		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id, Accepted = true };
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

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
		int id = 317;

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

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
		string initiatorUuid = GenerateUuid();
		string targetUuid = GenerateUuid();
		int id = 317;

		FriendshipModel friendship = new( initiatorUuid, targetUuid ) { Id = id };
		_ = _friendshipsRepoMock.AddKnownFriendship( friendship );

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

		_friendshipsRepoMock.SetupDelete();

		// act
		await _sut.DeleteFriendshipAsync( id );

		// assert
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );
		_friendshipsRepoMock.Verify( x => x.RemoveFriendshipsAsync( It.IsAny<FriendshipModel>() ), Times.Once );
		_friendshipsRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		var actual = Assert.Single( _friendshipsRepoMock.DeleteSaved );
		Assert.Equal( id, actual.Id );

	}

	[Fact]
	public async Task DeleteFriendshipAsync_ShouldThrow_WhenDoesNotExist()
	{
		// arrange
		int id = 317;

		_friendshipsRepoMock.SetupGet();
		_friendshipsRepoMock.SetupFirstOrDefault<FriendshipModel>();

		// act
		var act = () => _sut.DeleteFriendshipAsync( id );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );
		_friendshipsRepoMock.Verify( x => x.GetFriendships(), Times.Once );

	}

}
