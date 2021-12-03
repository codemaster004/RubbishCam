using Microsoft.Extensions.Logging;
using Moq;
using RubbishCam.Api.Exceptions;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.User;
using RubbishCam.Domain.Models;
using RubbishCam.UnitTests.Mocks.Repos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static RubbishCam.UnitTests.Helper;

namespace RubbishCam.UnitTests.Services;

public class UsersServiceTests
{
	private readonly UsersService _sut;
	private readonly UsersRepoMock _usersRepoMock = new( MockBehavior.Strict );
	private readonly Mock<ILogger<UsersService>> _loggerMock = new( MockBehavior.Strict );

	public UsersServiceTests()
	{
		_usersRepoMock.InitializeUsers();

		_sut = new( _usersRepoMock.Object, _loggerMock.Object );
	}

	[Fact]
	public async Task GetUsersAsync_ShouldReturnAllUsers()
	{
		// arrange
		_usersRepoMock.SetupGet();

		_usersRepoMock.SetupToArray<GetUserDto>();

		// act
		var actual = await _sut.GetUsersAsync();


		// assert
		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );

		Assert.NotNull( actual );
		Assert.Equal( _usersRepoMock.Users.Length, actual.Length );

		Assert.True( actual.SequenceEqual( _usersRepoMock.Users.Select( x => GetUserDto.FromUser( x ) ) ) );

	}

	[Fact]
	public async Task GetUserAsync_ShouldReturnUser_WhenUserExists()
	{
		// arrange
		string uuid = GenerateUuid();
		UserModel user = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( user );

		_usersRepoMock.SetupGet();

		_usersRepoMock.SetupFirstOrDefault<GetUserDetailsDto>();

		// act
		var actual = await _sut.GetUserAsync( uuid );


		// assert
		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );

		Assert.NotNull( actual );
		Assert.Equal( user.Uuid, actual!.Uuid );
		Assert.Equal( user.FirstName, actual!.FirstName );
		Assert.Equal( user.LastName, actual!.LastName );
		Assert.Equal( user.UserName, actual!.UserName );
		Assert.Empty( actual.Roles );
	}

	[Fact]
	public async Task GetUserAsync_ShouldReturnNull_WhenUserDoesNotExist()
	{
		// arrange
		_usersRepoMock.SetupGet();

		_usersRepoMock.SetupFirstOrDefault<GetUserDetailsDto>();

		// act
		var actual = await _sut.GetUserAsync( GenerateUuid() );


		// assert
		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );

		Assert.Null( actual );
	}

	[Fact]
	public async Task CreateUserAsync_ShouldReturnUser_WhenUsernameAvailable()
	{
		// arrange
		_usersRepoMock.SetupGet();

		_usersRepoMock.SetupAny<UserModel>();

		_usersRepoMock.SetupAdd();

		string password = "password";
		CreateUserDto user = new() { FirstName = "John", LastName = "Smith", Password = password, UserName = "johnsmith" };

		// act
		var returned = await _sut.CreateUserAsync( user );


		// assert
		Assert.NotNull( returned );
		Assert.Equal( user.FirstName, returned.FirstName );
		Assert.Equal( user.LastName, returned.LastName );
		Assert.Equal( user.UserName, returned.UserName );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.AtLeastOnce );
		_usersRepoMock.Verify( x => x.GetUsers(), Times.AtMost( 2 ) );
		_usersRepoMock.Verify( x => x.AddUserAsync( It.IsAny<UserModel>() ), Times.Once );
		_usersRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		var saved = Assert.Single( _usersRepoMock.AddSaved );

		Assert.NotNull( saved );
		Assert.Equal( saved!.Uuid, returned.Uuid );
		Assert.Equal( saved.FirstName, returned.FirstName );
		Assert.Equal( saved.LastName, returned.LastName );
		Assert.Equal( saved.UserName, returned.UserName );
		Assert.Equal( saved.PasswordHash, Hash( password ) );

		Assert.Empty( saved.Roles );
		Assert.Empty( saved.Tokens );
		Assert.Empty( saved.Friendships );
		Assert.Empty( saved.Friends );

		Assert.DoesNotContain( saved.Uuid, _usersRepoMock.Users.Select( u => u.Uuid ) );

	}

	[Fact]
	public async Task CreateUserAsync_ShouldThrow_WhenUsernameTaken()
	{
		// arrange
		UserModel existing = new( GenerateUuid(), "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( existing );

		_usersRepoMock.SetupGet();

		_usersRepoMock.SetupAny<UserModel>();

		CreateUserDto @new = new() { FirstName = Faker.Name.First(), LastName = Faker.Name.Last(), Password = "pass#1234ó", UserName = existing.UserName };

		// act
		var act = () => _sut.CreateUserAsync( @new );


		// assert
		_ = await Assert.ThrowsAsync<ConflictException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_usersRepoMock.Verify( x => x.AddUserAsync( It.IsAny<UserModel>() ), Times.Never );
		_usersRepoMock.Verify( x => x.SaveAsync(), Times.Never );

	}

	[Fact]
	public async Task DeleteUserAsync_ShouldSucceed_WhenUserExists()
	{
		// arrange
		string uuid = GenerateUuid();
		UserModel user = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( user );


		_usersRepoMock.SetupGet();

		_usersRepoMock.SetupFirstOrDefault<UserModel>();

		_usersRepoMock.SetupDelete();

		// act
		await _sut.DeleteUserAsync( uuid );


		// assert
		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_usersRepoMock.Verify( x => x.RemoveUserAsync( It.IsAny<UserModel>() ), Times.Once );
		_usersRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		var actual = Assert.Single( _usersRepoMock.DeleteSaved );
		Assert.Equal( user, actual );

	}

	[Fact]
	public async Task DeleteUserAsync_ShouldThrow_WhenUserDoesNotExist()
	{
		// arrange
		string uuid = GenerateUuid();
		UserModel user = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		_ = _usersRepoMock.AddKnownUser( user );

		_usersRepoMock.SetupGet();

		_usersRepoMock.SetupFirstOrDefault<UserModel>();

		// act
		var act = () => _sut.DeleteUserAsync( GenerateUuid() );


		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_usersRepoMock.Verify( x => x.RemoveUserAsync( It.IsAny<UserModel>() ), Times.Never );
		_usersRepoMock.Verify( x => x.SaveAsync(), Times.Never );

	}


}
