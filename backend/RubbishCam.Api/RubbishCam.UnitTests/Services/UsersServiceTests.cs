using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Moq;
using RubbishCam.Api.Exceptions;
using RubbishCam.Api.Repositories;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.User;
using RubbishCam.Domain.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RubbishCam.UnitTests.Services;

public class UsersServiceTests
{
	private readonly UsersService _sut;
	private readonly Mock<IUserRepository> _userRepoMock = new();
	private readonly Mock<ILogger<UsersService>> _loggerMock = new();

	public UsersServiceTests()
	{
		_sut = new( _userRepoMock.Object, _loggerMock.Object );
	}

	[Fact]
	public async Task GetUsersAsync_ShouldReturnAllUsers()
	{
		// arrange 
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.ToArrayAsync( It.IsAny<IQueryable<GetUserDto>>() ) )
			.Returns( ( IQueryable<GetUserDto> x ) => Task.FromResult( x.ToArray() ) );

		// act
		var actual = await _sut.GetUsersAsync();


		// assert
		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );

		Assert.NotNull( actual );
		Assert.Equal( users.Length, actual.Length );
		foreach ( var (userItem, actualItem) in users.Zip(actual) )
		{
			var expectedItem = GetUserDto.FromUser( userItem );
			Assert.Equal( expectedItem, actualItem );
		}

	}

	[Fact]
	public async Task GetUserAsync_ShouldReturnUser_WhenUserExists()
	{
		// arrange 
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		string uuid = GenerateUuid();
		UserModel user = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		users[users.Length / 2] = user;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<GetUserDetailsDto>>() ) )
			.Returns( ( IQueryable<GetUserDetailsDto> x ) => Task.FromResult( x.FirstOrDefault() ) );

		// act
		var actual = await _sut.GetUserAsync( uuid );


		// assert
		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );

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
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );
		_ = _userRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<GetUserDetailsDto>>() ) )
			.Returns( ( IQueryable<GetUserDetailsDto> x ) => Task.FromResult( x.FirstOrDefault() ) );

		// act
		var actual = await _sut.GetUserAsync( GenerateUuid() );


		// assert
		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );

		Assert.Null( actual );
	}

	[Fact]
	public async Task CreateUserAsync_ShouldReturnUser()
	{
		// arrange 
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		UserModel? passed = null;
		int called = 0;
		UserModel? saved = null;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.AnyAsync( It.IsAny<IQueryable<GetUserDetailsDto>>() ) )
			.Returns( ( IQueryable<GetUserDetailsDto> x ) => Task.FromResult( x.Any() ) );

		_ = _userRepoMock.Setup( x => x.AddUserAsync( It.IsAny<UserModel>() ) )
			.Callback( ( UserModel x ) => { passed = x; called++; } )
			.Returns( Task.CompletedTask );

		_ = _userRepoMock.Setup( x => x.SaveAsync() )
			.Callback( () => saved = passed )
			.Returns( Task.FromResult( called ) );

		string password = "password";
		CreateUserDto user = new() { FirstName = "John", LastName = "Smith", Password = password, UserName = "johnsmith" };

		// act
		var returned = await _sut.CreateUserAsync( user );


		// assert
		Assert.NotNull( returned );
		Assert.Equal( user.FirstName, returned.FirstName );
		Assert.Equal( user.LastName, returned.LastName );
		Assert.Equal( user.UserName, returned.UserName );

		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_userRepoMock.Verify( x => x.AddUserAsync( It.IsAny<UserModel>() ), Times.Once );
		_userRepoMock.Verify( x => x.SaveAsync(), Times.AtLeastOnce );

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

		Assert.DoesNotContain( saved.Uuid, users.Select( u => u.Uuid ) );

	}

	[Fact]
	public async Task DeleteUserAsync_ShouldSucceed_WhenUserExists()
	{
		// arrange 
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		string uuid = GenerateUuid();
		UserModel user = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		users[users.Length / 2] = user;

		UserModel? passed = null;
		int called = 0;
		UserModel? saved = null;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.RemoveUserAsync( It.IsAny<UserModel>() ) )
			.Callback( ( UserModel x ) => { passed = x; called++; } )
			.Returns( Task.CompletedTask );

		_ = _userRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<UserModel>>() ) )
			.Returns( ( IQueryable<UserModel> x ) => Task.FromResult( x.FirstOrDefault() ) );

		_ = _userRepoMock.Setup( x => x.SaveAsync() )
			.Callback( () => saved = passed )
			.Returns( Task.FromResult( called ) );

		// act
		await _sut.DeleteUserAsync( uuid );


		// assert
		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_userRepoMock.Verify( x => x.RemoveUserAsync( It.IsAny<UserModel>() ), Times.Once );
		_userRepoMock.Verify( x => x.SaveAsync(), Times.AtLeastOnce );

		Assert.NotNull( saved );
		Assert.Equal( user, saved );

	}

	[Fact]
	public async Task DeleteUserAsync_ShouldFail_WhenUserDoesNotExist()
	{
		// arrange 
		var users = Enumerable.Range( 0, 10 )
			.Select( x => new UserModel(
				GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				GenerateHash(),
				Faker.Internet.UserName() ) )
			.ToArray();

		string uuid = GenerateUuid();
		UserModel user = new( uuid, "John", "Smith", GenerateHash(), "john1234" );
		users[users.Length / 2] = user;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<UserModel>>() ) )
			.Returns( ( IQueryable<UserModel> x ) => Task.FromResult( x.FirstOrDefault() ) );

		// act
		var act = () => _sut.DeleteUserAsync( GenerateUuid() );


		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_userRepoMock.Verify( x => x.RemoveUserAsync( It.IsAny<UserModel>() ), Times.Never );
		_userRepoMock.Verify( x => x.SaveAsync(), Times.Never );

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
