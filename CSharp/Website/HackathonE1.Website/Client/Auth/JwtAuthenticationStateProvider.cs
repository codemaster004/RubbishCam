using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HackathonE1.Website.Client.Auth
{
	public class JwtAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly AuthenticationState anonymous = new( new ClaimsPrincipal() );
		public string Token { get; set; }


		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			if ( string.IsNullOrEmpty(Token) )
			{
				return anonymous;
			}
			await Task.CompletedTask;
			return anonymous;
		}
	}
}
