using Moq;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace RubbishCam.UnitTests.Mocks.Repos;

internal class GroupsRepoMock : Mock<IGroupsRepository>
{
	public GroupsRepoMock( MockBehavior behavior )
		: base( behavior )
	{
	}

	public GroupModel[] Groups { get; private set; } = Array.Empty<GroupModel>();
	public static IEnumerable<GroupModel> GenerateGroups( int start, int count )
	{
		return Enumerable.Range( start, count )
			.Select( x => new GroupModel( Faker.Company.Name() ) );
	}

	public void InitializeGroups()
	{
		Groups = GenerateGroups( 0, 10 )
			.ToArray();
	}
	public int AddKnownGroup( GroupModel group )
	{
		int index = Groups!.Length;
		Groups = Groups.Append( group )
			.Concat( GenerateGroups( index + 1, 10 ) )
			.ToArray();

		return index;
	}


	public void SetupGet()
	{
		_ = this.Setup( x => x.GetGroups() )
			.Returns( Groups!.ToArray().AsQueryable() );
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


	public GroupModel[] AddPassed { get; private set; } = Array.Empty<GroupModel>();
	public GroupModel[] AddSaved { get; private set; } = Array.Empty<GroupModel>();
	public void SetupAdd()
	{
		_ = this.Setup( x => x.AddGroupAsync( It.IsAny<GroupModel>() ) )
			.Callback( ( GroupModel x ) => AddPassed = AddPassed.Append( x ).ToArray() )
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
