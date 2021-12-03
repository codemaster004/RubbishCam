using Moq;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace RubbishCam.UnitTests.Mocks.Repos;

internal class UsersRepoMock : Mock<IUserRepository>
{
	public UsersRepoMock( MockBehavior behavior )
		: base( behavior )
	{
	}

	public UserModel[] Users { get; private set; } = Array.Empty<UserModel>();
	private static IEnumerable<UserModel> GenerateUsers( int start, int count )
	{
		return Enumerable.Range( start, count )
			.Select( x => new UserModel(
				Helper.GenerateUuid(),
				Faker.Name.First(),
				Faker.Name.Last(),
				Helper.GenerateHash(),
				Faker.Internet.UserName() ) );
	}

	public void InitializeUsers()
	{
		if ( Users is null )
		{
			Users = GenerateUsers( 0, 10 )
				.ToArray();
		}
	}
	public int AddKnownUser( UserModel user )
	{
		var index = Users!.Length;
		Users = Users.Append( user )
			.Concat( GenerateUsers( index + 1, 10 ) )
			.ToArray();

		return index;
	}


	public void SetupGet()
	{
		_ = this.Setup( x => x.GetUsers() )
			.Returns( Users!.ToArray().AsQueryable() );
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
	public void SetupCount<T>()
	{
		_ = this.Setup( x => x.CountAsync( It.IsAny<IQueryable<T>>() ) ).CallBase();
	}

	public UserModel[] AddPassed { get; private set; } = Array.Empty<UserModel>();
	public UserModel[] AddSaved { get; private set; } = Array.Empty<UserModel>();
	public void SetupAdd()
	{
		_ = this.Setup( x => x.AddUserAsync( It.IsAny<UserModel>() ) )
			.Callback( ( UserModel x ) => AddPassed = AddPassed.Append( x ).ToArray() )
			.Returns( Task.CompletedTask );

		SetupSave();
	}

	public UserModel[] DeletePassed { get; private set; } = Array.Empty<UserModel>();
	public UserModel[] DeleteSaved { get; private set; } = Array.Empty<UserModel>();
	public void SetupDelete()
	{
		_ = this.Setup( x => x.RemoveUserAsync( It.IsAny<UserModel>() ) )
			.Callback( ( UserModel x ) => DeletePassed = DeletePassed.Append( x ).ToArray() )
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
