using Microsoft.Extensions.Logging;
using Moq;
using RubbishCam.Api.Exceptions;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.Group;
using RubbishCam.Domain.Dtos.Group.Membership;
using RubbishCam.Domain.Models;
using RubbishCam.Domain.Relations;
using RubbishCam.UnitTests.Mocks.Repos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static RubbishCam.UnitTests.Helper;

namespace RubbishCam.UnitTests.Services;

public class GroupsServiceTests
{
	private readonly GroupsService _sut;
	private readonly GroupsRepoMock _groupsRepoMock = new( MockBehavior.Strict );
	private readonly MembersRepoMock _membersRepoMock = new( MockBehavior.Strict );
	private readonly UsersRepoMock _usersRepoMock = new( MockBehavior.Strict );
	private readonly Mock<ILogger<GroupsService>> _loggerMock = new( MockBehavior.Strict );

	public GroupsServiceTests()
	{
		_usersRepoMock.InitializeUsers();
		_groupsRepoMock.InitializeGroups();
		_membersRepoMock.InitializeMemberships();

		_sut = new( _groupsRepoMock.Object, _membersRepoMock.Object, _usersRepoMock.Object, _loggerMock.Object );
	}

	[Fact]
	public async Task GetGroupsAsync_ShouldReturnAllGroups()
	{
		// arrange
		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupToArray<GetGroupDto>();

		// act
		var actual = await _sut.GetGroupsAsync();

		// assert
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );

		Assert.NotNull( actual );
		Assert.Equal( _groupsRepoMock.Groups.Length, actual.Length );

		Assert.True( actual.SequenceEqual( _groupsRepoMock.Groups.Select( x => GetGroupDto.FromGroup( x ) ) ) );

	}


	[Fact]
	public async Task GetOwnedGroupsAsync_ByUuid_ShouldReturnOwnedGroups()
	{
		// arrange
		string uuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		var containing = Enumerable.Range( groupId, 10 )
			.Select( x => new GroupModel( Faker.Company.Name() ) { Id = x, Members = _usersRepoMock.Users.ToList() } )
			.ToArray();

		var memberships = containing.Select( ( x, i ) =>
			new GroupMembershipRelation( uuid )
			{
				GroupId = x.Id,
				User = requestor,
				Group = x,
				IsOwner = ( i & 1 ) > 0
			} );
		_membersRepoMock.AddManyKnownMemberships( memberships );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupToArray<GetGroupDto>();

		// act
		var actual = await _sut.GetOwnedGroupsAsync( uuid );

		// assert
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Once );

		var owned = memberships.Where( x => x.IsOwner ).Select( x => x.Group! ).ToArray();

		Assert.Equal( owned.Length, actual.Length );
		Assert.True( actual.SequenceEqual( owned.Select( x => GetGroupDto.FromGroup( x ) ) ) );

	}

	[Fact]
	public async Task GetOwnedGroupsAsync_ByUuid_ShouldReturnEmpty_WhenNone()
	{
		// arrange
		string uuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		var containing = Enumerable.Range( groupId, 10 )
			.Select( x => new GroupModel( Faker.Company.Name() ) { Id = x, Members = _usersRepoMock.Users.ToList() } )
			.ToArray();

		var memberships = containing.Select( ( x, i ) =>
			new GroupMembershipRelation( uuid )
			{
				GroupId = x.Id,
				User = requestor,
				Group = x,
				IsOwner = false
			} );
		_membersRepoMock.AddManyKnownMemberships( memberships );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupToArray<GetGroupDto>();

		// act
		var actual = await _sut.GetOwnedGroupsAsync( uuid );

		// assert
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Once );

		Assert.NotNull( actual );
		Assert.Empty( actual );

	}


	[Fact]
	public async Task GetGroupsAsync_ByUuid_ShouldReturnOwnedGroups()
	{
		// arrange
		string uuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		var owned = Enumerable.Range( groupId, 10 )
			.Select( x => new GroupModel( Faker.Company.Name() ) { Id = x, Members = _usersRepoMock.Users.ToList() } )
			.ToArray();

		var memberships = owned.Select( x => new GroupMembershipRelation( uuid ) { GroupId = x.Id, User = requestor, Group = x } );
		_membersRepoMock.AddManyKnownMemberships( memberships );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupToArray<GetGroupDto>();

		// act
		var actual = await _sut.GetGroupsAsync( uuid );

		// assert
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Once );
		Assert.Equal( owned.Length, actual.Length );
		Assert.True( actual.SequenceEqual( owned.Select( x => GetGroupDto.FromGroup( x ) ) ) );

	}

	[Fact]
	public async Task GetGroupsAsync_ByUuid_ShouldReturnEmpty_WhenNone()
	{
		// arrange
		string uuid = GenerateUuid();

		UserModel requestor = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupToArray<GetGroupDto>();

		// act
		var actual = await _sut.GetGroupsAsync( uuid );

		// assert
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Once );
		Assert.NotNull( actual );
		Assert.Empty( actual );

	}


	[Fact]
	public async Task GetGroupAsync_ShouldReturnGroup_WhenGroupExists()
	{
		// arrange
		string uuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		GroupModel group = new( "HelloThere" ) { Id = groupId, Members = _usersRepoMock.Users.ToList() };
		_ = _groupsRepoMock.AddKnownGroup( group );

		GroupMembershipRelation membership = new( uuid ) { GroupId = groupId, User = requestor, Group = group };
		_ = _membersRepoMock.AddKnownMembership( membership );

		_usersRepoMock.SetupGet();
		_groupsRepoMock.SetupGet();
		_membersRepoMock.SetupGet();

		_usersRepoMock.SetupFirstOrDefault<UserModel>();
		_groupsRepoMock.SetupFirstOrDefault<GetGroupDetailsDto>();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();

		// act
		var actual = await _sut.GetGroupAsync( groupId, uuid );

		// assert
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );

		Assert.NotNull( actual );
		Assert.Equal( group.Name, actual!.Name );
		Assert.Equal( group.Id, actual.Id );
		Assert.Equal( group.TimeCreated, actual.TimeCreated );
	}

	[Fact]
	public async Task GetGroupAsync_ShouldReturnNull_WhenGroupDoesNotExist()
	{
		// arrange
		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupFirstOrDefault<GetGroupDetailsDto>();
		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();

		// act
		var actual = await _sut.GetGroupAsync( 321, GenerateUuid() );


		// assert
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );

		Assert.Null( actual );
	}

	[Fact]
	public async Task GetGroupAsync_ShouldThrow_WhenUserNotAuthorized()
	{
		// arrange
		string uuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		GroupModel group = new( "HelloThere" ) { Id = groupId, Members = _usersRepoMock.Users.ToList() };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_usersRepoMock.SetupGet();
		_groupsRepoMock.SetupGet();
		_membersRepoMock.SetupGet();

		_usersRepoMock.SetupFirstOrDefault<UserModel>();
		_groupsRepoMock.SetupFirstOrDefault<GetGroupDetailsDto>();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();

		// act
		var act = () => _sut.GetGroupAsync( groupId, uuid );

		// assert
		_ = await Assert.ThrowsAsync<NotAuthorizedException>( act );

	}


	[Fact]
	public async Task GetGroupMembersAsync_ShouldReturnOwnedGroups()
	{
		// arrange
		string requestorUuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( requestorUuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		var members = _usersRepoMock.Users
			.Where( ( x, i ) => ( i & 1 ) > 0 )
			.Union( new[] { requestor } )
			.ToList();


		var group = new GroupModel( Faker.Company.Name() ) { Id = groupId, Members = members };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();

		var memberships = members.Select( ( x, i ) =>
			new GroupMembershipRelation( x.Uuid )
			{
				GroupId = groupId,
				User = x,
				Group = group,
				IsOwner = ( i & 1 ) > 0
			} ).ToArray();
		_membersRepoMock.AddManyKnownMemberships( memberships );


		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupToArray<GetGroupMembershipDto>();

		// act
		var actual = await _sut.GetGroupMembersAsync( groupId, requestorUuid );

		// assert
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

		Assert.Equal( memberships.Length, actual.Length );
		Assert.True( actual.SequenceEqual( memberships.Select( x => GetGroupMembershipDto.FromGroupMembersRelation( x ) ) ) );

	}

	[Fact]
	public async Task GetGroupMembersAsync_ShouldThrow_WhenGroupDoesNotExist()
	{
		// arrange
		string requestorUuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( requestorUuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );
		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();

		// act
		var act = () => _sut.GetGroupMembersAsync( groupId, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );

	}

	[Fact]
	public async Task GetGroupMembersAsync_ShouldThrow_WhenNotAMember()
	{
		// arrange
		string requestorUuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( requestorUuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		var members = _usersRepoMock.Users
			.Where( ( x, i ) => ( i & 1 ) > 0 )
			.Except( new[] { requestor } )
			.ToList();


		var group = new GroupModel( Faker.Company.Name() ) { Id = groupId, Members = members };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();

		var memberships = members.Select( ( x, i ) =>
			new GroupMembershipRelation( x.Uuid )
			{
				GroupId = groupId,
				User = x,
				Group = group,
				IsOwner = ( i & 1 ) > 0
			} ).ToArray();
		_membersRepoMock.AddManyKnownMemberships( memberships );


		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();

		// act
		var act = () => _sut.GetGroupMembersAsync( groupId, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotAuthorizedException>( act );

		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Once );

	}


	[Fact]
	public async Task GetOwnersAsync_ShouldReturnOwnedGroups()
	{
		// arrange
		string requestorUuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( requestorUuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		var members = _usersRepoMock.Users
			.Where( ( x, i ) => ( i & 1 ) > 0 )
			.Union( new[] { requestor } )
			.ToList();


		var group = new GroupModel( Faker.Company.Name() ) { Id = groupId, Members = members };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();

		var memberships = members.Select( ( x, i ) =>
			new GroupMembershipRelation( x.Uuid )
			{
				GroupId = groupId,
				User = x,
				Group = group,
				IsOwner = ( i & 1 ) > 0
			} ).ToArray();
		_membersRepoMock.AddManyKnownMemberships( memberships );


		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupToArray<GetGroupMembershipDto>();

		// act
		var actual = await _sut.GetOwnersAsync( groupId, requestorUuid );

		// assert
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

		var owners = memberships.Where( x => x.IsOwner ).ToArray();

		Assert.Equal( owners.Length, actual.Length );
		Assert.True( actual.SequenceEqual( owners.Select( x => GetGroupMembershipDto.FromGroupMembersRelation( x ) ) ) );

	}

	[Fact]
	public async Task GetOwnersAsync_ShouldReturnEmpty_WhenNone()
	{
		// arrange
		string requestorUuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( requestorUuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		var members = _usersRepoMock.Users
			.Where( ( x, i ) => ( i & 1 ) > 0 )
			.Union( new[] { requestor } )
			.ToList();


		var group = new GroupModel( Faker.Company.Name() ) { Id = groupId, Members = members };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();

		var memberships = members.Select( ( x, i ) =>
			new GroupMembershipRelation( x.Uuid )
			{
				GroupId = groupId,
				User = x,
				Group = group,
				IsOwner = false
			} ).ToArray();
		_membersRepoMock.AddManyKnownMemberships( memberships );


		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupToArray<GetGroupMembershipDto>();

		// act
		var actual = await _sut.GetOwnersAsync( groupId, requestorUuid );

		// assert
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

		Assert.NotNull( actual );
		Assert.Empty( actual );

	}

	[Fact]
	public async Task GetOwnersAsync_ShouldThrow_WhenGroupDoesNotExist()
	{
		// arrange
		string requestorUuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( requestorUuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );
		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();

		// act
		var act = () => _sut.GetOwnersAsync( groupId, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );

	}

	[Fact]
	public async Task GetOwnersAsync_ShouldThrow_WhenNotAMember()
	{
		// arrange
		string requestorUuid = GenerateUuid();
		int groupId = 317;

		UserModel requestor = new( requestorUuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( requestor );

		var members = _usersRepoMock.Users
			.Where( ( x, i ) => ( i & 1 ) > 0 )
			.Except( new[] { requestor } )
			.ToList();


		var group = new GroupModel( Faker.Company.Name() ) { Id = groupId, Members = members };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();

		var memberships = members.Select( ( x, i ) =>
			new GroupMembershipRelation( x.Uuid )
			{
				GroupId = groupId,
				User = x,
				Group = group,
				IsOwner = ( i & 1 ) > 0
			} ).ToArray();
		_membersRepoMock.AddManyKnownMemberships( memberships );


		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();

		// act
		var act = () => _sut.GetOwnersAsync( groupId, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotAuthorizedException>( act );

		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Once );

	}


	[Fact]
	public async Task CreateGroupAsync_ShouldReturnGroup()
	{
		// arrange
		var uuid = GenerateUuid();

		UserModel user = new( uuid, "John", "Smith", GenerateHash(), "JohnSmith" );
		_ = _usersRepoMock.AddKnownUser( user );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupAny<UserModel>();


		_groupsRepoMock.SetupAdd();

		CreateGroupDto dto = new() { Name = "Awesome Group" };


		// act
		var actual = await _sut.CreateGroupAsync( dto, uuid );

		// assert
		Assert.NotNull( actual );
		Assert.Equal( dto.Name, actual.Name );

	}

	[Fact]
	public async Task CreateGroupAsync_ShouldThrow_WhenUserDoesNotExist()
	{
		// arrange
		var uuid = GenerateUuid();

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupAny<UserModel>();


		CreateGroupDto dto = new() { Name = "Awesome Group" };

		// act
		var act = () => _sut.CreateGroupAsync( dto, uuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );

	}


	[Fact]
	public async Task AddToGroupAsync_ShouldSucceed()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();

		UserModel target = new( targetUuid, "Matt", "Stevens", GenerateHash(), "CyberMatt" );
		_ = _usersRepoMock.AddKnownUser( target );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupAny<UserModel>();


		GroupModel group = new( "Awesome Group" ) { Id = groupId };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();

		_membersRepoMock.SetupAdd();

		// act
		await _sut.AddToGroupAsync( groupId, targetUuid, requestorUuid );

		// assert
		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Exactly( 2 ) );

		var saved = Assert.Single( _membersRepoMock.AddSaved );
		Assert.Equal( targetUuid, saved.UserUuid );
		Assert.Equal( groupId, saved.GroupId );
		Assert.False( saved.IsOwner );

	}

	[Fact]
	public async Task AddToGroupAsync_ShouldThrow_WhenNotOwner()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();

		UserModel target = new( targetUuid, "Matt", "Stevens", GenerateHash(), "CyberMatt" );
		_ = _usersRepoMock.AddKnownUser( target );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupAny<UserModel>();


		GroupModel group = new( "Awesome Group" ) { Id = groupId };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = false };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();


		// act
		var act = () => _sut.AddToGroupAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotAuthorizedException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.AtMostOnce );
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.AtMostOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

	}

	[Fact]
	public async Task AddToGroupAsync_ShouldThrow_WhenTargetDoesotExist()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupAny<UserModel>();


		GroupModel group = new( "Awesome Group" ) { Id = groupId };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();

		// act
		var act = () => _sut.AddToGroupAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.AtMostOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

	}

	[Fact]
	public async Task AddToGroupAsync_ShouldThrow_WhenAlreadyAdded()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();

		UserModel target = new( targetUuid, "Matt", "Stevens", GenerateHash(), "CyberMatt" );
		_ = _usersRepoMock.AddKnownUser( target );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupAny<UserModel>();


		GroupModel group = new( "Awesome Group" ) { Id = groupId };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		GroupMembershipRelation targetMembership = new( targetUuid ) { GroupId = groupId };
		_ = _membersRepoMock.AddKnownMembership( targetMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();

		// act
		var act = () => _sut.AddToGroupAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.AtMostOnce );
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.AtMostOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtLeastOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

	}

	[Fact]
	public async Task AddToGroupAsync_ShouldThrow_WhenGroupDoesNotExist()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();

		UserModel target = new( targetUuid, "Matt", "Stevens", GenerateHash(), "CyberMatt" );
		_ = _usersRepoMock.AddKnownUser( target );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupAny<UserModel>();


		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();

		// act
		var act = () => _sut.AddToGroupAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.AtMostOnce );
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );

	}


	[Fact]
	public async Task RemoveFromGroupAsync_ShouldSucceed()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupModel group = new( "Awesome Group" ) { Id = groupId };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		GroupMembershipRelation targetMembership = new( targetUuid ) { GroupId = groupId };
		_ = _membersRepoMock.AddKnownMembership( targetMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		_membersRepoMock.SetupDelete();

		// act
		await _sut.RemoveFromGroupAsync( groupId, targetUuid, requestorUuid );

		// assert
		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Exactly( 2 ) );

		var saved = Assert.Single( _membersRepoMock.DeleteSaved );

		Assert.Equal( targetUuid, saved.UserUuid );
		Assert.Equal( groupId, saved.GroupId );

	}

	[Theory]
	[InlineData( false )]
	[InlineData( true )]
	public async Task RemoveFromGroupAsync_ShouldThrow_WhenNotOwner( bool isTargetOwner )
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupModel group = new( "Awesome Group" ) { Id = groupId };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = false };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		GroupMembershipRelation targetMembership = new( targetUuid ) { GroupId = groupId, IsOwner = isTargetOwner };
		_ = _membersRepoMock.AddKnownMembership( targetMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();

		// act
		var act = () => _sut.RemoveFromGroupAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotAuthorizedException>( act );

		_groupsRepoMock.Verify( x => x.GetGroups(), Times.AtMostOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Once );

		Assert.Equal( isTargetOwner, targetMembership.IsOwner );

	}

	[Fact]
	public async Task RemoveFromGroupAsync_ShouldThrow_WhenTargetIsNotMember()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupModel group = new( "Awesome Group" ) { Id = groupId };
		_ = _groupsRepoMock.AddKnownGroup( group );

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		// act
		var act = () => _sut.RemoveFromGroupAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );

		_groupsRepoMock.Verify( x => x.GetGroups(), Times.AtMostOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtLeastOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

	}

	[Fact]
	public async Task RemoveFromGroupAsync_ShouldThrow_WhenGroupDoesNotExist()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();

		_groupsRepoMock.SetupGet();
		_groupsRepoMock.SetupAny<GroupModel>();

		// act
		var act = () => _sut.RemoveFromGroupAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_groupsRepoMock.Verify( x => x.GetGroups(), Times.Once );

	}


	[Fact]
	public async Task AddAsOwnerAsync_ShouldSucceed()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		GroupMembershipRelation targetMembership = new( targetUuid ) { GroupId = groupId, IsOwner = false };
		_ = _membersRepoMock.AddKnownMembership( targetMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		_ = _membersRepoMock.Setup( x => x.SaveAsync() ).Returns( Task.FromResult( 1 ) );

		// act
		await _sut.AddAsOwnerAsync( groupId, targetUuid, requestorUuid );

		// assert
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Exactly( 2 ) );

		Assert.Equal( targetUuid, targetMembership.UserUuid );
		Assert.Equal( groupId, targetMembership.GroupId );
		Assert.True( targetMembership.IsOwner );

	}

	[Theory]
	[InlineData( false )]
	[InlineData( true )]
	public async Task AddAsOwnerAsync_ShouldThrow_WhenRequestorIsNotOwner( bool isTargetOwner )
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = false };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		GroupMembershipRelation targetMembership = new( targetUuid ) { GroupId = groupId, IsOwner = isTargetOwner };
		_ = _membersRepoMock.AddKnownMembership( targetMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		// act
		var act = () => _sut.AddAsOwnerAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotAuthorizedException>( act );

		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtLeastOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

		Assert.Equal( targetUuid, targetMembership.UserUuid );
		Assert.Equal( groupId, targetMembership.GroupId );
		Assert.Equal( isTargetOwner, targetMembership.IsOwner );

	}

	[Fact]
	public async Task AddAsOwnerAsync_ShouldThrow_WhenTargetAlreadyIsOwner()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		GroupMembershipRelation targetMembership = new( targetUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( targetMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		// act
		var act = () => _sut.AddAsOwnerAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );

		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtLeastOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

		Assert.Equal( targetUuid, targetMembership.UserUuid );
		Assert.Equal( groupId, targetMembership.GroupId );
		Assert.True( targetMembership.IsOwner );

	}

	[Fact]
	public async Task AddAsOwnerAsync_ShouldThrow_WhenTargetIsNotMember()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		_ = _membersRepoMock.Setup( x => x.SaveAsync() ).Returns( Task.FromResult( 1 ) );

		// act
		var act = () => _sut.AddAsOwnerAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtLeastOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

	}


	[Fact]
	public async Task RemoveAsOwnerAsync_ShouldSucceed()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		GroupMembershipRelation targetMembership = new( targetUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( targetMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		_ = _membersRepoMock.Setup( x => x.SaveAsync() ).Returns( Task.FromResult( 1 ) );

		// act
		await _sut.RemoveAsOwnerAsync( groupId, targetUuid, requestorUuid );

		// assert
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.Exactly( 2 ) );

		Assert.Equal( targetUuid, targetMembership.UserUuid );
		Assert.Equal( groupId, targetMembership.GroupId );
		Assert.False( targetMembership.IsOwner );

	}

	[Theory]
	[InlineData( false )]
	[InlineData( true )]
	public async Task RemoveAsOwnerAsync_ShouldThrow_WhenRequestorIsNotOwner( bool isTargetOwner )
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = false };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		GroupMembershipRelation targetMembership = new( targetUuid ) { GroupId = groupId, IsOwner = isTargetOwner };
		_ = _membersRepoMock.AddKnownMembership( targetMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		// act
		var act = () => _sut.RemoveAsOwnerAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotAuthorizedException>( act );

		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtLeastOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

		Assert.Equal( targetUuid, targetMembership.UserUuid );
		Assert.Equal( groupId, targetMembership.GroupId );
		Assert.Equal( isTargetOwner, targetMembership.IsOwner );

	}

	[Fact]
	public async Task RemoveAsOwnerAsync_ShouldThrow_WhenTargetAlreadyIsNotOwner()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		GroupMembershipRelation targetMembership = new( targetUuid ) { GroupId = groupId, IsOwner = false };
		_ = _membersRepoMock.AddKnownMembership( targetMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		// act
		var act = () => _sut.RemoveAsOwnerAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );

		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtLeastOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

		Assert.Equal( targetUuid, targetMembership.UserUuid );
		Assert.Equal( groupId, targetMembership.GroupId );
		Assert.False( targetMembership.IsOwner );

	}

	[Fact]
	public async Task RemoveAsOwnerAsync_ShouldThrow_WhenTargetIsNotMember()
	{
		// arrange
		int groupId = 317;
		string targetUuid = GenerateUuid();
		string requestorUuid = GenerateUuid();


		GroupMembershipRelation requestorMembership = new( requestorUuid ) { GroupId = groupId, IsOwner = true };
		_ = _membersRepoMock.AddKnownMembership( requestorMembership );

		_membersRepoMock.SetupGet();
		_membersRepoMock.SetupAny<GroupMembershipRelation>();
		_membersRepoMock.SetupFirstOrDefault<GroupMembershipRelation>();

		_ = _membersRepoMock.Setup( x => x.SaveAsync() ).Returns( Task.FromResult( 1 ) );

		// act
		var act = () => _sut.RemoveAsOwnerAsync( groupId, targetUuid, requestorUuid );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtLeastOnce );
		_membersRepoMock.Verify( x => x.GetGroupsMembersAsync(), Times.AtMost( 2 ) );

	}


}
