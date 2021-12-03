using Moq;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace RubbishCam.UnitTests.Mocks.Repos;

internal class FriendshipsRepoMock : Mock<IFriendshipsRepository>
{
	public FriendshipsRepoMock( MockBehavior behavior )
		: base( behavior )
	{
	}

	public FriendshipModel[] Friendships { get; private set; } = Array.Empty<FriendshipModel>();
	private static IEnumerable<FriendshipModel> GenerateFriendships( int start, int count )
	{
		return Enumerable.Range( start, count )
			.Select( x => new FriendshipModel( Helper.GenerateUuid(), Helper.GenerateUuid() ) { Accepted = ( x & 1 ) > 0 } );
	}

	public void InitializeFriendships()
	{
		Friendships = GenerateFriendships( 0, 10 )
			.ToArray();
	}
	public int AddKnownFriendship( FriendshipModel friendship )
	{
		var index = Friendships!.Length;
		Friendships = Friendships.Append( friendship )
			.Concat( GenerateFriendships( index + 1, 10 ) )
			.ToArray();

		return index;
	}
	public void AddManyKnownFriendships( IEnumerable<FriendshipModel> friendships )
	{
		foreach ( var friendship in friendships )
		{
			var index = Friendships!.Length;
			Friendships = Friendships.Append( friendship )
						.Concat( GenerateFriendships( index + 1, 2 ) )
						.ToArray();
		}
	}


	public void SetupGet()
	{
		_ = this.Setup( x => x.GetFriendships() )
			.Returns( Friendships!.ToArray().AsQueryable() );
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

	public FriendshipModel[] AddPassed { get; private set; } = Array.Empty<FriendshipModel>();
	public FriendshipModel[] AddSaved { get; private set; } = Array.Empty<FriendshipModel>();
	public void SetupAdd()
	{
		_ = this.Setup( x => x.AddFriendshipsAsync( It.IsAny<FriendshipModel>() ) )
			.Callback( ( FriendshipModel x ) => AddPassed = AddPassed.Append( x ).ToArray() )
			.Returns( Task.CompletedTask );

		SetupSave();
	}

	public FriendshipModel[] DeletePassed { get; private set; } = Array.Empty<FriendshipModel>();
	public FriendshipModel[] DeleteSaved { get; private set; } = Array.Empty<FriendshipModel>();
	public void SetupDelete()
	{
		_ = this.Setup( x => x.RemoveFriendshipsAsync( It.IsAny<FriendshipModel>() ) )
			.Callback( ( FriendshipModel x ) => DeletePassed = DeletePassed.Append( x ).ToArray() )
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
				DeleteSaved = DeletePassed.ToArray();
			} )
			.Returns( () =>
			{
				var diff = DeletePassed.Length - DeleteSaved.Length + AddPassed.Length - AddSaved.Length;
				return Task.FromResult( diff );
			} );

		saveSetUp = true;
	}
}
