using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HackathonE1.Website.Client.Auth
{
	public class JwtAuthenticationStateProvider : AuthenticationStateProvider, ILoginManager
	{
		private static readonly JwtSecurityTokenHandler _tokenHandler = new();
		private readonly IHttpClientFactory _httpFactory;

		private readonly AuthenticationState anonymous = new( new ClaimsPrincipal() );
		private HttpClient HttpClient => _httpFactory.CreateClient( "api" );

		public JwtAuthenticationStateProvider( IHttpClientFactory httpFactory )
		{
			this._httpFactory = httpFactory;
		}


		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await IsTokenValidAsync();
			if ( token is null )
			{
				return anonymous;
			}

			var principal = PrincipalFromToken( token );
			return new AuthenticationState( principal );
		}

		class LoginModel
		{
			public string Username { get; set; }
			public string Password { get; set; }
		}
		public async Task<bool> Login( string username, string password )
		{
			LoginModel model = new()
			{
				Username = username,
				Password = password
			};

			using HttpRequestMessage msg = new( HttpMethod.Post, new Uri( "auth/token", UriKind.Relative ) );
			msg.Content = JsonContent.Create( model );
			_ = msg.SetBrowserRequestCredentials( BrowserRequestCredentials.Include );

			var resp = await HttpClient.SendAsync( msg );

			if ( !resp.IsSuccessStatusCode )
			{
				return false;
			}

			var token = await resp.Content.ReadAsStringAsync();

			NotifyAuthenticationStateChanged( GetAuthenticationStateAsync() );

			return true;
		}

		public async Task Logout()
		{
			using HttpRequestMessage msg = new( HttpMethod.Post, new Uri( "auth/token/logout", UriKind.Relative ) );
			_ = msg.SetBrowserRequestCredentials( BrowserRequestCredentials.Include );
			var resp = await HttpClient.SendAsync( msg );


			NotifyAuthenticationStateChanged( Task.FromResult( anonymous ) );
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

		private async Task<string> IsTokenValidAsync()
		{
			using HttpRequestMessage msg = new( HttpMethod.Get, new Uri( "/auth/token/check", UriKind.Relative ) );
			_ = msg.SetBrowserRequestCredentials( BrowserRequestCredentials.Include );

			var resp = await HttpClient.SendAsync( msg );

			if ( !resp.IsSuccessStatusCode )
			{
				return null;
			}

			return await resp.Content.ReadAsStringAsync();
		}

	}
}
