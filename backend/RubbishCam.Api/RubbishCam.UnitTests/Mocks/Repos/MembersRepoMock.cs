using Moq;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Relations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace RubbishCam.UnitTests.Mocks.Repos;

internal class MembersRepoMock : Mock<IGroupsMembersRepository>
{
	public MembersRepoMock( MockBehavior behavior )
		: base( behavior )
	{
	}

	public GroupMembershipRelation[] Memberships { get; private set; } = Array.Empty<GroupMembershipRelation>();
	private static IEnumerable<GroupMembershipRelation> GenerateMemberships( int start, int count )
	{
		return Enumerable.Range( start, count )
			.Select( x => new GroupMembershipRelation( Helper.GenerateUuid() ) );
	}

	public void InitializeMemberships()
	{
		Memberships = GenerateMemberships( 0, 10 )
			.ToArray();
	}
	public int AddKnownMembership( GroupMembershipRelation membership )
	{
		var index = Memberships!.Length;
		Memberships = Memberships.Append( membership )
			.Concat( GenerateMemberships( index + 1, 10 ) )
			.ToArray();

		return index;
	}
	public void AddManyKnownMemberships( IEnumerable<GroupMembershipRelation> memberships )
	{
		foreach ( var membership in memberships )
		{
			var index = Memberships!.Length;
			Memberships = Memberships.Append( membership )
						.Concat( GenerateMemberships( index + 1, 2 ) )
						.ToArray();
		}
	}


	public void SetupGet()
	{
		_ = this.Setup( x => x.GetGroupsMembersAsync() )
			.Returns( Memberships!.ToArray().AsQueryable() );
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


	public GroupMembershipRelation[] AddPassed { get; private set; } = Array.Empty<GroupMembershipRelation>();
	public GroupMembershipRelation[] AddSaved { get; private set; } = Array.Empty<GroupMembershipRelation>();
	public void SetupAdd()
	{
		_ = this.Setup( x => x.AddGroupMemberAsync( It.IsAny<GroupMembershipRelation>() ) )
			.Callback( ( GroupMembershipRelation x ) => AddPassed = AddPassed.Append( x ).ToArray() )
			.Returns( Task.CompletedTask );

		SetupSave();
	}

	public GroupMembershipRelation[] DeletePassed { get; private set; } = Array.Empty<GroupMembershipRelation>();
	public GroupMembershipRelation[] DeleteSaved { get; private set; } = Array.Empty<GroupMembershipRelation>();
	public void SetupDelete()
	{
		_ = this.Setup( x => x.RemoveGroupMemberAsync( It.IsAny<GroupMembershipRelation>() ) )
			.Callback( ( GroupMembershipRelation x ) => DeletePassed = DeletePassed.Append( x ).ToArray() )
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
