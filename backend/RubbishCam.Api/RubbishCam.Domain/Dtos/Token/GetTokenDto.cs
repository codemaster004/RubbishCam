using RubbishCam.Domain.Models;
using System.Linq.Expressions;

namespace RubbishCam.Domain.Dtos.Token;

#nullable disable warnings

public record GetTokenDto
{
	public string Token { get; init; }
	public string RefreshToken { get; init; }
	public DateTimeOffset ValidUntil { get; init; }

#nullable restore

	public static Expression<Func<TokenModel, GetTokenDto>> FromTokenExp { get; } = token => new GetTokenDto()
	{
		Token = token.Token,
		ValidUntil = token.ValidUntil,
		RefreshToken = token.RefreshToken,
	};

	private static readonly Func<TokenModel, GetTokenDto> fromTokenFunc = FromTokenExp.Compile();
	public static GetTokenDto FromToken( TokenModel role )
	{
		return fromTokenFunc( role );
	}

}
