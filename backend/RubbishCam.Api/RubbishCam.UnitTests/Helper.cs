using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.UnitTests;

internal static class Helper
{
	public static string GenerateUuid()
	{
		return Base64UrlTextEncoder.Encode( Guid.NewGuid().ToByteArray() );
	}
	public static string GenerateToken()
	{
		return Base64UrlTextEncoder.Encode( Guid.NewGuid().ToByteArray() );
	}
	private static readonly SHA512 sha = SHA512.Create();
	public static string GenerateHash()
	{
		return Hash( Faker.Lorem.Paragraph() );
	}
	public static string Hash( string source )
	{
		byte[] buffer = Encoding.UTF8.GetBytes( source );
		return Convert.ToBase64String( sha.ComputeHash( buffer ) );
	}
}
