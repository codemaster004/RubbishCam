using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Domain.Dtos.Token;

#nullable disable warnings

public class GetTokenDto
{
	public string Token { get; set; }
	public DateTimeOffset ValidUntil { get; set; }

	public static Expression<Func<TokenModel, GetTokenDto>> FromTokenExp { get; set; } = role => new GetTokenDto()
	{
		Token = role.Token,
		ValidUntil = role.ValidUntil,
	};

	private static readonly Func<TokenModel, GetTokenDto> fromTokenFunc = FromTokenExp.Compile();
	public static GetTokenDto FromToken( TokenModel role )
	{
		return fromTokenFunc( role );
	}

}
