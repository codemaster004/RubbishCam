using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Moq;
using RubbishCam.Api.Repositories;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.User;
using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
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
		_sut = new( _loggerMock.Object, _userRepoMock.Object );
	}

	[Fact]
	public async Task GetUserAsync_ShouldReturnUser_WhenUserExistsAsync()
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
		Assert.NotNull( actual );
		Assert.Equal( user.Uuid, actual!.Uuid );
		Assert.Equal( user.FirstName, actual!.FirstName );
		Assert.Equal( user.LastName, actual!.LastName );
		Assert.Equal( user.UserName, actual!.UserName );
		Assert.Empty( actual.Roles );
	}

	private static string GenerateUuid()
	{
		return Base64UrlTextEncoder.Encode( Guid.NewGuid().ToByteArray() );
	}
	private static readonly SHA512 sha = SHA512.Create();
	private static string GenerateHash()
	{
		byte[] buffer = Encoding.UTF8.GetBytes( Faker.Lorem.Paragraph() );
		return Convert.ToBase64String( sha.ComputeHash( buffer ) );
	}


}
