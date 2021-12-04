using Moq;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.UnitTests.Mocks.Repos;

internal class TokensRepoMock : Mock<ITokensRepository>
{
	public TokensRepoMock( MockBehavior behavior )
		: base( behavior )
	{
	}

	public TokenModel[] Tokens { get; private set; } = Array.Empty<TokenModel>();
	public static IEnumerable<TokenModel> GenerateTokens( int start, int count )
	{
		return Enumerable.Range( start, count )
			.Select( x => new TokenModel(
				Helper.GenerateToken(),
				Helper.GenerateToken(),
				Helper.GenerateUuid(),
				DateTimeOffset.Now.AddMinutes( x ) ) 
			);
	}

	public void InitializeTokens()
	{
		Tokens = GenerateTokens( 0, 10 )
			.ToArray();
	}
	public int AddKnownToken( TokenModel token )
	{
		int index = Tokens!.Length;
		Tokens = Tokens.Append( token )
			.Concat( GenerateTokens( index + 1, 10 ) )
			.ToArray();

		return index;
	}


	public void SetupGet()
	{
		_ = this.Setup( x => x.GetTokens() )
			.Returns( Tokens!.ToArray().AsQueryable() );
	}
	public void SetupFirstOrDefault<T>()
	{
		_ = this.Setup( x => x.FirstOrDefaultAsync( It.IsAny<IQueryable<T>>() ) ).CallBase();
	}
	public void SetupToArray<T>()
	{
		_ = this.Setup( x => x.ToArrayAsync( It.IsAny<IQueryable<T>>() ) ).CallBase();
	}
	public void SetupAny<T>()
	{
		_ = this.Setup( x => x.AnyAsync( It.IsAny<IQueryable<T>>() ) ).CallBase();
	}

	public TokenModel[] AddPassed { get; private set; } = Array.Empty<TokenModel>();
	public TokenModel[] AddSaved { get; private set; } = Array.Empty<TokenModel>();
	public void SetupAdd()
	{
		_ = this.Setup( x => x.AddTokenAsync( It.IsAny<TokenModel>() ) )
			.Callback( ( TokenModel x ) => AddPassed = AddPassed.Append( x ).ToArray() )
			.Returns( Task.CompletedTask );

		SetupSave();
	}

	private bool saveSetUp = false;
	private void SetupSave()
	{
		if ( saveSetUp )
		{
			return;
		}


		_ = this.Setup( x => x.SaveAsync() )
			.Callback( () =>
			{
				AddSaved = AddPassed.ToArray();
			} )
			.Returns( () =>
			{
				var diff = AddPassed.Length - AddSaved.Length;
				return Task.FromResult( diff );
			} );

		saveSetUp = true;
	}
}
