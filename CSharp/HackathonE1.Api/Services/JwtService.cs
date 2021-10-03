using HackathonE1.Api.Data;
using HackathonE1.Api.Helpers;
using HackathonE1.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HackathonE1.Api.Services
{
	public interface IJwtService
	{
		Task<string> AuthenticateAsync( string username, string password );
		Task<string> RefreshAsync( ClaimsPrincipal principal );
		Task<string> RefreshAsync( string old );
	}

	public class JwtService : IJwtService
	{
		private static readonly JwtSecurityTokenHandler _tokenHandler = new();

		private readonly AppDbContext _dbContext;
		private readonly JwtHelper _jwtHelper;

		public JwtService( AppDbContext context, JwtHelper jwtSource )
		{
			this._dbContext = context;
			this._jwtHelper = jwtSource;
		}

		public async Task<string> AuthenticateAsync( string email, string password )
		{
			var user = await _dbContext.Users
				.Where( u => u.Email == email && u.PasswordHash == password )
				.FirstOrDefaultAsync();

			if ( user is null )
			{
				return null;
			}

			Claim[] claims = GetClaims( user ).ToArray();

			var token = _jwtHelper.CreateToken( claims, user.Email );

			return _tokenHandler.WriteToken( token );
		}

		public static IEnumerable<Claim> GetClaims( UserModel user )
		{
			yield return new Claim( type: ClaimTypes.Name, user.Identifier );

			//foreach ( var role in user.Roles )
			//{
			//	yield return new Claim( type: ClaimTypes.Role, role.Name );
			//}
		}

		public async Task<string> RefreshAsync( ClaimsPrincipal principal )
		{
			await Task.CompletedTask;

			var @new = _jwtHelper.CreateToken( principal.Claims );

			return _tokenHandler.WriteToken( @new );
		}
		public async Task<string> RefreshAsync( string old )
		{
			var principal = await PrincipalFromToken( old );

			return await RefreshAsync( principal );
		}


		public async Task<ClaimsPrincipal> PrincipalFromToken( string tokenString )
		{
			await Task.CompletedTask;

			JwtSecurityToken token = _tokenHandler.ReadJwtToken( tokenString );

			ClaimsIdentity identity = new( token.Claims );

			ClaimsPrincipal principal = new( identity );

			return principal;
		}


	}
}
