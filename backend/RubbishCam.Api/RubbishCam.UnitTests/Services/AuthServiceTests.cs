using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Moq;
using RubbishCam.Api.Exceptions;
using RubbishCam.Api.Exceptions.Auth;
using RubbishCam.Api.Services;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RubbishCam.UnitTests.Services;

public class AuthServiceTests
{
	private readonly AuthService _sut;
	private readonly Mock<ITokenRepository> _tokenRepoMock = new( MockBehavior.Strict );
	private readonly Mock<IUserRepository> _userRepoMock = new( MockBehavior.Strict );
	private readonly Mock<ILogger<AuthService>> _loggerMock = new();

	public AuthServiceTests()
	{
		_sut = new( _tokenRepoMock.Object, _userRepoMock.Object, _loggerMock.Object );
	}

	[Fact]
	public async Task Login_ShouldSucceed_WhenCorrect()
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

		string username = "john1234";
		string password = "pass#1234ó";

		UserModel loggingUser = new( GenerateUuid(), "John", "Smith", Hash( password ), username );
		users[users.Length / 2] = loggingUser;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( () => users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<UserModel>>() ) ).CallBase();

		#endregion

		#region tokens
		var tokens = Enumerable.Range( 1, 11 )
			.Select( x => new TokenModel(
				 GenerateToken(),
				 GenerateToken(),
				GenerateUuid(),
				DateTimeOffset.Now.AddMinutes( x ) ) )
			.ToArray();

		TokenModel? passed = null;
		int called = 0;
		TokenModel? saved = null;

		_ = _tokenRepoMock.Setup( x => x.GetTokens() )
			.Returns( () => tokens.ToArray().AsQueryable() );

		_ = _tokenRepoMock.Setup( x => x.AnyAsync( It.IsAny<IQueryable<TokenModel>>() ) ).CallBase();

		_ = _tokenRepoMock.Setup( x => x.AddTokenAsync( It.IsAny<TokenModel>() ) )
			.Callback( ( TokenModel t ) => { passed = t; called++; } )
			.Returns( Task.CompletedTask );

		_ = _tokenRepoMock.Setup( x => x.SaveAsync() )
			.Callback( () => saved = passed )
			.Returns( Task.FromResult( called ) );

		#endregion

		// act
		var returned = await _sut.Login( username, password );

		// assert
		Assert.NotNull( returned );
		Assert.NotNull( returned.Token );
		Assert.NotNull( returned.RefreshToken );
		Assert.True( returned.ValidUntil > DateTimeOffset.Now );

		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_tokenRepoMock.Verify( x => x.AddTokenAsync( It.IsAny<TokenModel>() ), Times.Once );
		_tokenRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.NotNull( saved );
		Assert.NotNull( saved!.Token );
		Assert.NotNull( saved.RefreshToken );

		Assert.Equal( loggingUser.Uuid, saved.UserUuid );
		Assert.Equal( returned.ValidUntil, saved.ValidUntil );
		Assert.False( saved.Revoked );
		Assert.DoesNotContain( saved.Token, tokens.Select( t => t.Token ) );
		Assert.DoesNotContain( saved.RefreshToken, tokens.Select( t => t.RefreshToken ) );

	}

	[Fact]
	public async Task Login_ShouldFail_WhenUsernameIncorrect()
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

		string username = "john1234";
		string password = "pass#1234ó";

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( () => users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<UserModel>>() ) ).CallBase();

		#endregion

		// act
		var act = () => _sut.Login( username, password );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_tokenRepoMock.Verify( x => x.AddTokenAsync( It.IsAny<TokenModel>() ), Times.Never );
		_tokenRepoMock.Verify( x => x.SaveAsync(), Times.Never );

	}

	[Fact]
	public async Task Login_ShouldFail_WhenPasswordIncorrect()
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

		string username = "john1234";
		string password = "pass#1234ó";

		UserModel loggingUser = new( GenerateUuid(), "John", "Smith", Hash( password ), username );
		users[users.Length / 2] = loggingUser;

		_ = _userRepoMock.Setup( x => x.GetUsers() )
			.Returns( () => users.ToArray().AsQueryable() );

		_ = _userRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<UserModel>>() ) ).CallBase();

		#endregion

		// act
		var act = () => _sut.Login( username, password + "a" );

		// assert
		_ = await Assert.ThrowsAsync<NotAuthorizedException>( act );

		_userRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_tokenRepoMock.Verify( x => x.AddTokenAsync( It.IsAny<TokenModel>() ), Times.Never );
		_tokenRepoMock.Verify( x => x.SaveAsync(), Times.Never );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldSucceed_WhenValidAccessToken()
	{
		// arrange
		#region tokens
		var tokens = Enumerable.Range( 1, 11 )
			.Select( x => new TokenModel(
				 GenerateToken(),
				 GenerateToken(),
				GenerateUuid(),
				DateTimeOffset.Now.AddMinutes( x ) ) )
			.ToArray();


		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		tokens[tokens.Length / 2] = testedToken;

		_ = _tokenRepoMock.Setup( x => x.GetTokens() )
			.Returns( () => tokens.ToArray().AsQueryable() );

		_ = _tokenRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<TokenModel>>() ) ).CallBase();

		_ = _tokenRepoMock.Setup( x => x.SaveAsync() )
			.Returns( Task.FromResult( 1 ) );

		#endregion

		// act
		await _sut.RevokeTokenAsync( testedToken.Token );

		// assert
		_tokenRepoMock.Verify( x => x.GetTokens(), Times.Once );
		_tokenRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.True( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldThrow_WhenInvalidAccessToken()
	{
		// arrange
		#region tokens
		var tokens = Enumerable.Range( 1, 11 )
			.Select( x => new TokenModel(
				 GenerateToken(),
				 GenerateToken(),
				GenerateUuid(),
				DateTimeOffset.Now.AddMinutes( x ) ) )
			.ToArray();


		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		tokens[tokens.Length / 2] = testedToken;

		_ = _tokenRepoMock.Setup( x => x.GetTokens() )
			.Returns( () => tokens.ToArray().AsQueryable() );

		_ = _tokenRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<TokenModel>>() ) ).CallBase();

		_ = _tokenRepoMock.Setup( x => x.SaveAsync() )
			.Returns( Task.FromResult( 1 ) );

		#endregion

		// act
		var act = () => _sut.RevokeTokenAsync( testedToken.RefreshToken + "a" );

		// assert
		_ = await Assert.ThrowsAsync<TokenInvalidException>( act );

		_tokenRepoMock.Verify( x => x.GetTokens(), Times.Once );
		_tokenRepoMock.Verify( x => x.SaveAsync(), Times.Never );

		Assert.False( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldThrow_WhenRefreshToken()
	{
		// arrange
		#region tokens
		var tokens = Enumerable.Range( 1, 11 )
			.Select( x => new TokenModel(
				 GenerateToken(),
				 GenerateToken(),
				GenerateUuid(),
				DateTimeOffset.Now.AddMinutes( x ) ) )
			.ToArray();


		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		tokens[tokens.Length / 2] = testedToken;

		_ = _tokenRepoMock.Setup( x => x.GetTokens() )
			.Returns( () => tokens.ToArray().AsQueryable() );

		_ = _tokenRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<TokenModel>>() ) ).CallBase();

		_ = _tokenRepoMock.Setup( x => x.SaveAsync() )
			.Returns( Task.FromResult( 1 ) );

		#endregion

		// act
		var act = () => _sut.RevokeTokenAsync( testedToken.RefreshToken );

		// assert
		_ = await Assert.ThrowsAsync<TokenInvalidException>( act );

		_tokenRepoMock.Verify( x => x.GetTokens(), Times.Once );
		_tokenRepoMock.Verify( x => x.SaveAsync(), Times.Never );

		Assert.False( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

	}

	[Fact]
	public async Task RefreshTokenAsync_ShouldSucceed_WhenValidRefreshToken()
	{
		// arrange
		#region tokens
		var tokens = Enumerable.Range( 1, 11 )
			.Select( x => new TokenModel(
				 GenerateToken(),
				 GenerateToken(),
				GenerateUuid(),
				DateTimeOffset.Now.AddMinutes( x ) ) )
			.ToArray();


		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		tokens[tokens.Length / 2] = testedToken;



		TokenModel? passed = null;
		int called = 0;
		TokenModel? saved = null;

		_ = _tokenRepoMock.Setup( x => x.GetTokens() )
			.Returns( () => tokens.ToArray().AsQueryable() );

		_ = _tokenRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<TokenModel>>() ) ).CallBase();

		_ = _tokenRepoMock.Setup( x => x.AnyAsync( It.IsAny<IQueryable<TokenModel>>() ) ).CallBase();

		_ = _tokenRepoMock.Setup( x => x.AddTokenAsync( It.IsAny<TokenModel>() ) )
			.Callback( ( TokenModel t ) => { passed = t; called++; } )
			.Returns( Task.CompletedTask );

		_ = _tokenRepoMock.Setup( x => x.SaveAsync() )
			.Callback( () => saved = passed )
			.Returns( Task.FromResult( called ) );


		#endregion

		// act
		var returned = await _sut.RefreshTokenAsync( testedToken.RefreshToken );

		// assert
		_tokenRepoMock.Verify( x => x.GetTokens(), Times.AtLeastOnce );
		_tokenRepoMock.Verify( x => x.GetTokens(), Times.AtMost( 2 ) );
		_tokenRepoMock.Verify( x => x.AddTokenAsync( It.IsAny<TokenModel>() ), Times.Once );
		_tokenRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.True( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

		Assert.NotNull( saved );
		Assert.Equal( testedToken.UserUuid, saved!.UserUuid );
		Assert.True( saved.ValidUntil > DateTimeOffset.Now );

		Assert.NotNull( returned );
		Assert.Equal( saved.Token, returned.Token );
		Assert.Equal( saved.RefreshToken, returned.RefreshToken );
		Assert.Equal( saved.ValidUntil, returned.ValidUntil );

		Assert.NotEqual( testedToken.Token, saved.Token );
		Assert.NotEqual( testedToken.RefreshToken, saved.RefreshToken );

	}

	[Fact]
	public async Task GetTokenAsync_ShouldReturnUser_WhenTokenExists()
	{
		// arrange 
		var tokens = Enumerable.Range( 1, 11 )
			.Select( x => new TokenModel(
				 GenerateToken(),
				 GenerateToken(),
				GenerateUuid(),
				DateTimeOffset.Now.AddMinutes( x ) ) )
			.ToArray();

		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		tokens[tokens.Length / 2] = testedToken;

		_ = _tokenRepoMock.Setup( x => x.GetTokens() )
			.Returns( tokens.ToArray().AsQueryable() );

		_ = _tokenRepoMock.Setup( x => x.WithUsersWithRoles( It.IsAny<IQueryable<TokenModel>>() ) )
			.Returns( ( IQueryable<TokenModel> x ) => x );


		_ = _tokenRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<TokenModel>>() ) ).CallBase();

		// act
		var returned = await _sut.GetTokenAsync( accessToken );


		// assert
		_tokenRepoMock.Verify( x => x.GetTokens(), Times.Once );

		Assert.NotNull( returned );
		Assert.Equal( testedToken.Token, returned!.Token );
		Assert.Equal( testedToken.UserUuid, returned.UserUuid );
		Assert.Equal( testedToken.RefreshToken, returned.RefreshToken );
		Assert.Equal( testedToken.ValidUntil, returned.ValidUntil );
		Assert.Equal( testedToken.Revoked, returned.Revoked );
	}

	[Fact]
	public async Task GetTokenAsync_ShouldReturnNull_WhenTokenDoesNotExist()
	{
		// arrange 
		var tokens = Enumerable.Range( 1, 11 )
			.Select( x => new TokenModel(
				 GenerateToken(),
				 GenerateToken(),
				GenerateUuid(),
				DateTimeOffset.Now.AddMinutes( x ) ) )
			.ToArray();

		_ = _tokenRepoMock.Setup( x => x.GetTokens() )
			.Returns( tokens.ToArray().AsQueryable() );

		_ = _tokenRepoMock.Setup( x => x.WithUsersWithRoles( It.IsAny<IQueryable<TokenModel>>() ) )
			.Returns( ( IQueryable<TokenModel> x ) => x );

		_ = _tokenRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<TokenModel>>() ) ).CallBase();

		// act
		var actual = await _sut.GetTokenAsync( GenerateUuid() );


		// assert
		_tokenRepoMock.Verify( x => x.GetTokens(), Times.Once );

		Assert.Null( actual );
	}

	[Fact]
	public async Task GetTokenAsync_ShouldReturnNull_WhenTokenRefreshToken()
	{
		// arrange 
		var tokens = Enumerable.Range( 1, 11 )
			.Select( x => new TokenModel(
				 GenerateToken(),
				 GenerateToken(),
				GenerateUuid(),
				DateTimeOffset.Now.AddMinutes( x ) ) )
			.ToArray();

		string refreshToken = GenerateToken();
		TokenModel testedToken = new( GenerateToken(), refreshToken, GenerateUuid(), DateTimeOffset.Now.AddYears( 1 ) );
		tokens[tokens.Length / 2] = testedToken;

		_ = _tokenRepoMock.Setup( x => x.GetTokens() )
			.Returns( tokens.ToArray().AsQueryable() );

		_ = _tokenRepoMock.Setup( x => x.WithUsersWithRoles( It.IsAny<IQueryable<TokenModel>>() ) )
			.Returns( ( IQueryable<TokenModel> x ) => x );

		_ = _tokenRepoMock.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<TokenModel>>() ) ).CallBase();

		// act
		var actual = await _sut.GetTokenAsync( refreshToken );


		// assert
		_tokenRepoMock.Verify( x => x.GetTokens(), Times.Once );

		Assert.Null( actual );
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
	private static string GenerateToken()
	{
		byte[] buffer = Encoding.UTF8.GetBytes( Faker.Lorem.Paragraph() );
		return Convert.ToBase64String( sha.ComputeHash( buffer ) );
	}
	private static string Hash( string source )
	{
		byte[] buffer = Encoding.UTF8.GetBytes( source );
		return Convert.ToBase64String( sha.ComputeHash( buffer ) );
	}


}
