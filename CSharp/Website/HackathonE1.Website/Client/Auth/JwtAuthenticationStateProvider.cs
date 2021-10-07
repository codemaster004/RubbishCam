using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HackathonE1.Website.Client.Auth
{
	public class JwtAuthenticationStateProvider : AuthenticationStateProvider
	{
		private static readonly JwtSecurityTokenHandler _tokenHandler = new();
		private readonly IHttpClientFactory _httpFactory;

		private readonly AuthenticationState anonymous = new( new ClaimsPrincipal() );

		public JwtAuthenticationStateProvider( IHttpClientFactory httpFactory )
		{
			this._httpFactory = httpFactory;
		}

		private static string token;
		public string Token
		{
			get => token;
			set
			{
				token = value;
				NotifyAuthenticationStateChanged( GetAuthenticationStateAsync() );
			}
		}
		public static string TokenValue => token;


		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			if ( string.IsNullOrEmpty( Token ) )
			{
				return anonymous;
			}

			if ( !await IsTokenValidAsync() )
			{
				Token = null;
				return anonymous;
			}

			var principal = PrincipalFromToken( token );
			return new AuthenticationState( principal );
		}

		private static ClaimsPrincipal PrincipalFromToken( string tokenString )
		{
			JwtSecurityToken token = _tokenHandler.ReadJwtToken( tokenString );

			var claims = token.Claims.Select( RenameClaim ).ToArray();

			ClaimsIdentity identity = new( claims: claims, authenticationType: "Bearer", nameType: ClaimTypes.GivenName, roleType: ClaimTypes.Role );

			ClaimsPrincipal principal = new( identity );

			return principal;
		}

		private static Claim RenameClaim( Claim claim )
		{
			return claim.Type switch
			{
				"unique_name" => new Claim( ClaimTypes.Name, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer, claim.Subject ),
				"given_name" => new Claim( ClaimTypes.GivenName, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer, claim.Subject ),
				"family_name" => new Claim( ClaimTypes.Surname, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer, claim.Subject ),
				"email" => new Claim( ClaimTypes.Email, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer, claim.Subject ),
				"role" => new Claim( ClaimTypes.Role, claim.Value, claim.ValueType, claim.Issuer, claim.OriginalIssuer, claim.Subject ),
				_ => claim
			};
		}

		private async Task<bool> IsTokenValidAsync()
		{
			var resp = await _httpFactory.CreateClient( "api" ).GetAsync( "/auth/token/check" );
			return resp.IsSuccessStatusCode;
		}

	}
}
