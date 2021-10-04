using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Api.Helpers
{
	public class JwtHelper
	{
		private static readonly JwtSecurityTokenHandler _jwtHandler = new();
		private readonly IConfiguration _configuration;

		public JwtHelper( IConfiguration configuration )
		{
			_configuration = configuration;
		}


		static SymmetricSecurityKey GetKey( IConfiguration configuration )
		{
			var secret = configuration["JwtConfig:Secret"] ?? Environment.GetEnvironmentVariable( "JWT_SECRET" );

			var bytes = Encoding.UTF8.GetBytes( secret );
			return new SymmetricSecurityKey( bytes );
		}
		static string GetIssuer( IConfiguration configuration )
		{
			return configuration["JwtConfig:Issuer"] ?? Environment.GetEnvironmentVariable( "JWT_ISSUER" );
		}


		public SecurityToken CreateToken( IEnumerable<Claim> claims )
		{
			return CreateToken( claims, null );
		}
		public SecurityToken CreateToken( IEnumerable<Claim> claims, string audience )
		{
			var subject = new ClaimsIdentity( claims );
			var now = DateTime.UtcNow;
			var expires = now.AddMinutes( 15 );

			var issuer = GetIssuer( _configuration );

			var key = GetKey( _configuration );
			var credentials = new SigningCredentials( key, SecurityAlgorithms.HmacSha256Signature );

			return _jwtHandler.CreateJwtSecurityToken(
				issuer: issuer,
				audience: audience,
				subject: subject,
				expires: expires,
				issuedAt: now,
				signingCredentials: credentials );
		}



		public static void ConfigureAuthOptions( AuthenticationOptions options )
		{
			options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}

		public static void ConfigureJwtOptions( JwtBearerOptions options, IConfiguration configuration )
		{
			options.RequireHttpsMetadata = true;
			options.TokenValidationParameters = new()
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = GetKey( configuration ),

				ValidateIssuer = true,
				ValidIssuer = GetIssuer( configuration ),

				ValidateLifetime = true,

				ValidateAudience = false,

				ClockSkew = TimeSpan.FromSeconds( 5 ),
			};

		}
	}
}
