using Microsoft.Extensions.Logging;
using Moq;
using RubbishCam.Api.Exceptions;
using RubbishCam.Api.Exceptions.Auth;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Models;
using RubbishCam.UnitTests.Mocks.Repos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static RubbishCam.UnitTests.Helper;

namespace RubbishCam.UnitTests.Services;

public class AuthServiceTests
{
	private readonly AuthService _sut;
	private readonly UsersRepoMock _usersRepoMock = new( MockBehavior.Strict );
	private readonly TokensRepoMock _tokensRepoMock = new( MockBehavior.Strict );
	private readonly Mock<ILogger<AuthService>> _loggerMock = new();

	public AuthServiceTests()
	{
		_usersRepoMock.InitializeUsers();
		_tokensRepoMock.InitializeTokens();

		_sut = new( _tokensRepoMock.Object, _usersRepoMock.Object, _loggerMock.Object );
	}

	[Fact]
	public async Task Login_ShouldSucceed_WhenCorrect()
	{
		// arrange
		string username = "john1234";
		string password = "pass#1234ó";

		UserModel loggingUser = new( GenerateUuid(), "John", "Smith", Hash( password ), username );
		_ = _usersRepoMock.AddKnownUser( loggingUser );

		_usersRepoMock.SetupGet();
		_usersRepoMock.SetupFirstOrDefault<UserModel>();


		_tokensRepoMock.SetupGet();
		_tokensRepoMock.SetupAny<TokenModel>();
		_tokensRepoMock.SetupAdd();


		// act
		var returned = await _sut.Login( username, password );

		// assert
		Assert.NotNull( returned );
		Assert.NotNull( returned.Token );
		Assert.NotNull( returned.RefreshToken );
		Assert.True( returned.ValidUntil > DateTimeOffset.Now );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_tokensRepoMock.Verify( x => x.AddTokenAsync( It.IsAny<TokenModel>() ), Times.Once );
		_tokensRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		var actual = Assert.Single( _tokensRepoMock.AddSaved );

		Assert.Equal( loggingUser.Uuid, actual.UserUuid );
		Assert.Equal( returned.ValidUntil, actual.ValidUntil );
		Assert.False( actual.Revoked );
		Assert.DoesNotContain( actual.Token, _tokensRepoMock.Tokens.Select( t => t.Token ) );
		Assert.DoesNotContain( actual.RefreshToken, _tokensRepoMock.Tokens.Select( t => t.RefreshToken ) );

	}

	[Fact]
	public async Task Login_ShouldFail_WhenUsernameIncorrect()
	{
		// arrange
		string username = "john1234";
		string password = "pass#1234ó";

		_usersRepoMock.SetupGet();

		_usersRepoMock.SetupFirstOrDefault<UserModel>();

		// act
		var act = () => _sut.Login( username, password );

		// assert
		_ = await Assert.ThrowsAsync<NotFoundException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_tokensRepoMock.Verify( x => x.AddTokenAsync( It.IsAny<TokenModel>() ), Times.Never );
		_tokensRepoMock.Verify( x => x.SaveAsync(), Times.Never );

	}

	[Fact]
	public async Task Login_ShouldFail_WhenPasswordIncorrect()
	{
		// arrange
		string username = "john1234";
		string password = "pass#1234ó";

		UserModel loggingUser = new( GenerateUuid(), "John", "Smith", Hash( password ), username );
		_ = _usersRepoMock.AddKnownUser( loggingUser );

		_usersRepoMock.SetupGet();

		_usersRepoMock.SetupFirstOrDefault<UserModel>();

		// act
		var act = () => _sut.Login( username, password + "a" );

		// assert
		_ = await Assert.ThrowsAsync<NotAuthorizedException>( act );

		_usersRepoMock.Verify( x => x.GetUsers(), Times.Once );
		_tokensRepoMock.Verify( x => x.AddTokenAsync( It.IsAny<TokenModel>() ), Times.Never );
		_tokensRepoMock.Verify( x => x.SaveAsync(), Times.Never );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldSucceed_WhenValidAccessToken()
	{
		// arrange
		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		_ = _tokensRepoMock.AddKnownToken( testedToken );

		_tokensRepoMock.SetupGet();

		_tokensRepoMock.SetupFirstOrDefault<TokenModel>();

		_ = _tokensRepoMock.Setup( x => x.SaveAsync() )
			.Returns( Task.FromResult( 1 ) );

		// act
		await _sut.RevokeTokenAsync( testedToken.Token );

		// assert
		_tokensRepoMock.Verify( x => x.GetTokens(), Times.Once );
		_tokensRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.True( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldSucceed_WhenAlreadyRevoked()
	{
		// arrange
		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) ) { Revoked = true };
		_ = _tokensRepoMock.AddKnownToken( testedToken );

		_tokensRepoMock.SetupGet();

		_tokensRepoMock.SetupFirstOrDefault<TokenModel>();

		// act
		await _sut.RevokeTokenAsync( testedToken.Token );

		// assert
		_tokensRepoMock.Verify( x => x.GetTokens(), Times.Once );

		Assert.True( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldThrow_WhenInvalidAccessToken()
	{
		// arrange
		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		_ = _tokensRepoMock.AddKnownToken( testedToken );

		_tokensRepoMock.SetupGet();

		_tokensRepoMock.SetupFirstOrDefault<TokenModel>();

		_ = _tokensRepoMock.Setup( x => x.SaveAsync() )
			.Returns( Task.FromResult( 1 ) );

		// act
		var act = () => _sut.RevokeTokenAsync( testedToken.RefreshToken + "a" );

		// assert
		_ = await Assert.ThrowsAsync<TokenInvalidException>( act );

		_tokensRepoMock.Verify( x => x.GetTokens(), Times.Once );
		_tokensRepoMock.Verify( x => x.SaveAsync(), Times.Never );

		Assert.False( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldThrow_WhenRefreshToken()
	{
		// arrange
		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		_ = _tokensRepoMock.AddKnownToken( testedToken );

		_tokensRepoMock.SetupGet();

		_tokensRepoMock.SetupFirstOrDefault<TokenModel>();

		_ = _tokensRepoMock.Setup( x => x.SaveAsync() )
			.Returns( Task.FromResult( 1 ) );

		// act
		var act = () => _sut.RevokeTokenAsync( testedToken.RefreshToken );

		// assert
		_ = await Assert.ThrowsAsync<TokenInvalidException>( act );

		_tokensRepoMock.Verify( x => x.GetTokens(), Times.Once );
		_tokensRepoMock.Verify( x => x.SaveAsync(), Times.Never );

		Assert.False( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldSucceed_WhenExists()
	{
		// arrange
		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		_ = _tokensRepoMock.AddKnownToken( testedToken );

		_ = _tokensRepoMock.Setup( x => x.SaveAsync() )
			.Returns( Task.FromResult( 1 ) );

		// act
		await _sut.RevokeTokenAsync( testedToken );

		// assert
		_tokensRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.True( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldSucceed_WhenAreadyRevoked()
	{
		// arrange
		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) ) { Revoked = true };
		_ = _tokensRepoMock.AddKnownToken( testedToken );

		// act
		await _sut.RevokeTokenAsync( testedToken );

		// assert
		Assert.True( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

	}

	[Fact]
	public async Task RevokeTokenAsync_ShouldThrow_WhenDoesNotExist()
	{
		// arrange
		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );

		_ = _tokensRepoMock.Setup( x => x.SaveAsync() )
			.Returns( Task.FromResult( 0 ) );

		// act
		var act = () => _sut.RevokeTokenAsync( testedToken );

		// assert
		_ = await Assert.ThrowsAsync<TokenInvalidException>( act );
		_tokensRepoMock.Verify( x => x.SaveAsync(), Times.AtMostOnce );

	}

	[Fact]
	public async Task RefreshTokenAsync_ShouldSucceed_WhenValidRefreshToken()
	{
		// arrange
		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		_ = _tokensRepoMock.AddKnownToken( testedToken );

		_tokensRepoMock.SetupGet();
		_tokensRepoMock.SetupFirstOrDefault<TokenModel>();
		_tokensRepoMock.SetupAny<TokenModel>();
		_tokensRepoMock.SetupAdd();

		// act
		var returned = await _sut.RefreshTokenAsync( testedToken.RefreshToken );

		// assert
		_tokensRepoMock.Verify( x => x.GetTokens(), Times.AtLeastOnce );
		_tokensRepoMock.Verify( x => x.GetTokens(), Times.AtMost( 2 ) );
		_tokensRepoMock.Verify( x => x.AddTokenAsync( It.IsAny<TokenModel>() ), Times.Once );
		_tokensRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.True( testedToken.Revoked );
		Assert.Equal( userUuid, testedToken.UserUuid );
		Assert.Equal( accessToken, testedToken.Token );
		Assert.Equal( refreshToken, testedToken.RefreshToken );

		var saved = Assert.Single( _tokensRepoMock.AddSaved );

		Assert.Equal( testedToken.UserUuid, saved.UserUuid );
		Assert.NotEqual( testedToken.Token, saved.Token );
		Assert.NotEqual( testedToken.RefreshToken, saved.RefreshToken );

		Assert.True( saved.ValidUntil > DateTimeOffset.Now );

		Assert.NotNull( returned );
		Assert.Equal( saved.Token, returned.Token );
		Assert.Equal( saved.RefreshToken, returned.RefreshToken );
		Assert.Equal( saved.ValidUntil, returned.ValidUntil );


	}

	[Fact]
	public async Task GetTokenAsync_ShouldReturnUser_WhenTokenExists()
	{
		// arrange
		string accessToken = GenerateToken();
		string refreshToken = GenerateToken();
		string userUuid = GenerateUuid();
		TokenModel testedToken = new( accessToken, refreshToken, userUuid, DateTimeOffset.Now.AddYears( 1 ) );
		_ = _tokensRepoMock.AddKnownToken( testedToken );

		_tokensRepoMock.SetupGet();

		_ = _tokensRepoMock.Setup( x => x.WithUsersWithRoles( It.IsAny<IQueryable<TokenModel>>() ) )
			.Returns( ( IQueryable<TokenModel> x ) => x );

		_tokensRepoMock.SetupFirstOrDefault<TokenModel>();

		// act
		var returned = await _sut.GetTokenAsync( accessToken );

		// assert
		_tokensRepoMock.Verify( x => x.GetTokens(), Times.Once );
		_tokensRepoMock.Verify( x => x.WithUsersWithRoles( It.IsAny<IQueryable<TokenModel>>() ), Times.Once );

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
		_tokensRepoMock.SetupGet();

		_ = _tokensRepoMock.Setup( x => x.WithUsersWithRoles( It.IsAny<IQueryable<TokenModel>>() ) )
			.Returns( ( IQueryable<TokenModel> x ) => x );

		_tokensRepoMock.SetupFirstOrDefault<TokenModel>();

		// act
		var actual = await _sut.GetTokenAsync( GenerateUuid() );

		// assert
		_tokensRepoMock.Verify( x => x.GetTokens(), Times.Once );

		Assert.Null( actual );
	}

	[Fact]
	public async Task GetTokenAsync_ShouldReturnNull_WhenTokenRefreshToken()
	{
		// arrange
		string refreshToken = GenerateToken();
		TokenModel testedToken = new( GenerateToken(), refreshToken, GenerateUuid(), DateTimeOffset.Now.AddYears( 1 ) );
		_ = _tokensRepoMock.AddKnownToken( testedToken );

		_tokensRepoMock.SetupGet();

		_ = _tokensRepoMock.Setup( x => x.WithUsersWithRoles( It.IsAny<IQueryable<TokenModel>>() ) )
			.Returns( ( IQueryable<TokenModel> x ) => x );

		_tokensRepoMock.SetupFirstOrDefault<TokenModel>();

		// act
		var actual = await _sut.GetTokenAsync( refreshToken );

		// assert
		_tokensRepoMock.Verify( x => x.GetTokens(), Times.Once );

		Assert.Null( actual );
	}


}
