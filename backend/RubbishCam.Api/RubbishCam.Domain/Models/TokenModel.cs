using System.ComponentModel.DataAnnotations;

namespace RubbishCam.Domain.Models;

public class TokenModel
{
	public TokenModel( string token, string refreshToken, string userUuid, DateTimeOffset validUntil )
	{
		Token = token;
		RefreshToken = refreshToken;
		UserUuid = userUuid;
		ValidUntil = validUntil;
	}

	[Key]
	public int Id { get; set; }

	public string Token { get; set; }
	public string RefreshToken { get; set; }

	public string UserUuid { get; set; }
	public DateTimeOffset ValidUntil { get; set; }
	
	public bool Revoked { get; set; }

	public UserModel? User { get; set; }
}
