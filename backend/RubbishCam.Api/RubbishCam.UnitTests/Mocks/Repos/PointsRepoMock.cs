using Moq;
using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RubbishCam.UnitTests.Mocks.Repos;

internal class PointsRepoMock : Mock<IPointsRepository>
{
	public PointsRepoMock( MockBehavior behavior )
		: base( behavior )
	{
	}

	public PointModel[] Points { get; private set; } = Array.Empty<PointModel>();
	private readonly Random rng = new( 10 );
	private IEnumerable<PointModel> GeneratePoints( int start, int count )
	{
		return Enumerable.Range( start, count )
			.Select( x => new PointModel(
				x & 3,
				Helper.GenerateUuid(),
				rng.NextDouble() * 90,
				rng.NextDouble() * 90,
				DateTimeOffset.Now.AddHours( -rng.Next( 8000 ) ) )
			);
	}

	public void InitializePoints()
	{
		if ( Points is null )
		{
			Points = GeneratePoints( 0, 10 )
				.ToArray();
		}
	}
	public int AddKnownPoint( PointModel point )
	{
		var index = Points!.Length;
		Points = Points.Append( point )
			.Concat( GeneratePoints( index + 1, 10 ) )
			.ToArray();

		return index;
	}
	public void AddManyKnownPoints( IEnumerable<PointModel> points )
	{
		foreach ( var point in points )
		{
			var index = Points.Length;
			Points = Points.Append( point )
						.Concat( GeneratePoints( index + 1, 2 ) )
						.ToArray();
		}
	}


	public void SetupGet()
	{
		_ = this.Setup( x => x.GetPoints() )
			.Returns( Points!.ToArray().AsQueryable() );
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
	public void SetupSum<T>()
	{
		_ = this.Setup( x => x.SumAsync( It.IsAny<IQueryable<T>>(), It.IsAny<Expression<Func<T, int>>>() ) ).CallBase();
	}

	public PointModel[] AddPassed { get; private set; } = Array.Empty<PointModel>();
	public PointModel[] AddSaved { get; private set; } = Array.Empty<PointModel>();
	public void SetupAdd()
	{
		_ = this.Setup( x => x.AddPointAsync( It.IsAny<PointModel>() ) )
			.Callback( ( PointModel x ) => AddPassed = AddPassed.Append( x ).ToArray() )
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
