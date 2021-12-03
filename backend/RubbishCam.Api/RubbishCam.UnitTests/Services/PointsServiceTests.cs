using Microsoft.Extensions.Logging;
using Moq;
using RubbishCam.Api.Services;
using RubbishCam.Domain.Dtos.Point;
using RubbishCam.Domain.Models;
using RubbishCam.UnitTests.Mocks.Repos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using static RubbishCam.UnitTests.Helper;

namespace RubbishCam.UnitTests.Services;

public class PointsServiceTests
{
	private readonly PointsService _sut;
	private readonly PointsRepoMock _pointsRepoMock = new( MockBehavior.Strict );
	private readonly Mock<ILogger<PointsService>> _loggerMock = new( MockBehavior.Strict );

	public PointsServiceTests()
	{
		_pointsRepoMock.InitializePoints();

		_sut = new( _pointsRepoMock.Object, _loggerMock.Object );
	}

	[Fact]
	public async Task GetPointsAsync_ShouldReturnAllPoints()
	{
		// arrange
		var uuid = GenerateUuid();
		UserModel? user = new( uuid,
			Faker.Name.First(),
			Faker.Name.Last(),
			GenerateHash(),
			Faker.Internet.UserName() );

		var owned = Enumerable.Range( 2, 10 )
			.Select( ( x, i ) =>
			new PointModel(
				( x & 1 ) + 131,
				uuid,
				x, i,
				DateTimeOffset.Now.AddHours( -x * i ) )
			).ToArray();

		_pointsRepoMock.AddManyKnownPoints( owned );

		_pointsRepoMock.SetupGet();
		_ = _pointsRepoMock.Setup( x => x.WithTypes( It.IsAny<IQueryable<PointModel>>() ) ).Returns( ( IQueryable<PointModel> x ) => x );
		_pointsRepoMock.SetupToArray<GetPointDetailsDto>();

		// act
		var actual = await _sut.GetPointsAsync( uuid );

		// assert
		_pointsRepoMock.Verify( x => x.GetPoints(), Times.Once );
		_pointsRepoMock.Verify( x => x.WithTypes( It.IsAny<IQueryable<PointModel>>() ), Times.Once );

		Assert.NotNull( actual );
		Assert.True( actual.SequenceEqual( owned.Select( x => GetPointDetailsDto.FromPoint( x ) ) ) );
	}

	[Fact]
	public async Task GetPointsAsync_ShouldReturnEmpty_WhenNone()
	{
		// arrange
		var uuid = GenerateUuid();
		UserModel? user = new( uuid,
			Faker.Name.First(),
			Faker.Name.Last(),
			GenerateHash(),
			Faker.Internet.UserName() );

		_pointsRepoMock.SetupGet();
		_ = _pointsRepoMock.Setup( x => x.WithTypes( It.IsAny<IQueryable<PointModel>>() ) ).Returns( ( IQueryable<PointModel> x ) => x );
		_pointsRepoMock.SetupToArray<GetPointDetailsDto>();

		// act
		var actual = await _sut.GetPointsAsync( uuid );

		// assert
		_pointsRepoMock.Verify( x => x.GetPoints(), Times.Once );
		_pointsRepoMock.Verify( x => x.WithTypes( It.IsAny<IQueryable<PointModel>>() ), Times.Once );

		Assert.NotNull( actual );
		Assert.Empty( actual );
	}

	[Fact]
	public async Task SumPointsAsync_ShouldReturnSum()
	{
		// arrange
		var uuid = GenerateUuid();
		UserModel? user = new( uuid,
			Faker.Name.First(),
			Faker.Name.Last(),
			GenerateHash(),
			Faker.Internet.UserName() );
		var type = new GarbageTypeModel[] {
			new( "evil trash" ) { Id = 132, PointsPerItem = 2 },
			new( "yes" ) { Id = 131, PointsPerItem = 1 }
		};

		var owned = Enumerable.Range( 2, 10 )
			.Select( ( x, i ) =>
			new PointModel(
				( x & 1 ) + 131,
				uuid,
				x, i,
				DateTimeOffset.Now.AddHours( -x * i ) )
			{ GarbageType = type[( x & 1 )] }
			).ToArray();

		_pointsRepoMock.AddManyKnownPoints( owned );

		_pointsRepoMock.SetupGet();
		_pointsRepoMock.SetupSum<PointModel>();

		// act
		var actual = await _sut.SumPointsAsync( uuid );

		// assert
		_pointsRepoMock.Verify( x => x.GetPoints(), Times.Once );

		Assert.Equal( owned.Select( x => x.GarbageType!.PointsPerItem ).Sum(), actual );
	}

	[Fact]
	public async Task SumPointsAsync_ShouldReturnZero_WhenNone()
	{
		// arrange
		var uuid = GenerateUuid();
		UserModel? user = new( uuid,
			Faker.Name.First(),
			Faker.Name.Last(),
			GenerateHash(),
			Faker.Internet.UserName() );


		_pointsRepoMock.SetupGet();
		_pointsRepoMock.SetupSum<PointModel>();

		// act
		var actual = await _sut.SumPointsAsync( uuid );

		// assert
		_pointsRepoMock.Verify( x => x.GetPoints(), Times.Once );

		Assert.Equal( 0, actual );
	}

	[Fact]
	public async Task CreatePointsAsync_ShouldReturnPoint()
	{
		// arrange
		var uuid = GenerateUuid();
		UserModel? user = new( uuid,
			Faker.Name.First(),
			Faker.Name.Last(),
			GenerateHash(),
			Faker.Internet.UserName() );

		_pointsRepoMock.SetupAdd();

		int typeId = 1;
		CreatePointDto point = new()
		{
			GarbageTypeId = typeId,
			Latitude = 10,
			Longitude = 10,
			DateScored = DateTimeOffset.Now
		};

		// act
		var actual = await _sut.CreatePoint( point, uuid );

		// assert
		_pointsRepoMock.Verify( x => x.AddPointAsync( It.IsAny<PointModel>() ), Times.Once );
		_pointsRepoMock.Verify( x => x.SaveAsync(), Times.Once );

		Assert.NotNull( actual );
		Assert.Equal( uuid, actual.UserUuid );
		Assert.Equal( point.Latitude, actual.Latitude );
		Assert.Equal( point.Longitude, actual.Longitude );
		Assert.Equal( point.DateScored, actual.DateScored );
		Assert.Equal( point.GarbageTypeId, typeId );
	}


}
