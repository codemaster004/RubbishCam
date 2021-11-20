using RubbishCam.Domain.Models;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Token;

#nullable disable warnings

public class GetTokenDto
{
	public string Token { get; set; }
	public string RefreshToken { get; set; }
	public DateTimeOffset ValidUntil { get; set; }

#nullable restore

	public static Expression<Func<TokenModel, GetTokenDto>> FromTokenExp { get; set; } = role => new GetTokenDto()
	{
		Token = role.Token,
		ValidUntil = role.ValidUntil,
		RefreshToken = role.RefreshToken,
	};

	private static readonly Func<TokenModel, GetTokenDto> fromTokenFunc = FromTokenExp.Compile();
	public static GetTokenDto FromToken( TokenModel role )
	{
		return fromTokenFunc( role );
	}

}
