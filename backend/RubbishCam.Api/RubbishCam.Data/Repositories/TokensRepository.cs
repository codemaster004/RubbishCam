using Microsoft.EntityFrameworkCore;
using RubbishCam.Domain.Models;

namespace RubbishCam.Data.Repositories;

public interface ITokensRepository : IRepository
{
	IQueryable<TokenModel> GetTokens();
	Task AddTokenAsync( TokenModel token );
	Task<int> SaveAsync();

	IQueryable<TokenModel> WithUsers( IQueryable<TokenModel> source );
	IQueryable<TokenModel> WithUsersWithRoles( IQueryable<TokenModel> source );
}

public class TokensRepository : ITokensRepository
{
	private readonly AppDbContext _dbContext;

	public TokensRepository( AppDbContext dbContext )
	{
		_dbContext = dbContext;
	}

	public IQueryable<TokenModel> GetTokens()
	{
		return _dbContext.Tokens;
	}
	public async Task AddTokenAsync( TokenModel token )
	{
		_ = await _dbContext.Tokens.AddAsync( token );
	}
	public Task<int> SaveAsync()
	{
		return _dbContext.SaveChangesAsync();
	}

	public IQueryable<TokenModel> WithUsers( IQueryable<TokenModel> source )
	{
		return source.Include( t => t.User! );
	}
	public IQueryable<TokenModel> WithUsersWithRoles( IQueryable<TokenModel> source )
	{
		return source.Include( t => t.User! )
			.ThenInclude( u => u.Roles! );
	}


}

public static class TokensRepositoryExtensions
{
	public static IQueryable<TokenModel> FilterByAccessToken( this IQueryable<TokenModel> source, string token )
	{
		return source.Where( t => t.Token == token );
	}
	public static IQueryable<TokenModel> FilterByRefreshToken( this IQueryable<TokenModel> source, string token )
	{
		return source.Where( t => t.RefreshToken == token );
	}
	public static IQueryable<TokenModel> WithUsers( this IQueryable<TokenModel> source, ITokensRepository repository )
	{
		return repository.WithUsers( source );
	}
	public static IQueryable<TokenModel> WithUsersWithRoles( this IQueryable<TokenModel> source, ITokensRepository repository )
	{
		return repository.WithUsersWithRoles( source );
	}


}
