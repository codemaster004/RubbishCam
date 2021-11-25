using RubbishCam.Data.Repositories;
using RubbishCam.Domain.Dtos.Point;
using RubbishCam.Domain.Models;

namespace RubbishCam.Api.Services;

public interface IPointsService
{
	Task<GetPointDetailsDto> CreatePoint( CreatePointDto dto, string uuid );
	Task<GetPointDetailsDto[]> GetPointsAsync( string uuid );
	Task<int> SumPointsAsync( string uuid );
}

public class PointsService : IPointsService
{
	private readonly IPointsRepository _pointRepo;
	private readonly ILogger<PointsService> _logger;

	public PointsService( IPointsRepository pointRepo, ILogger<PointsService> logger )
	{
		_pointRepo = pointRepo ?? throw new ArgumentNullException( nameof( pointRepo ) );
		_logger = logger ?? throw new ArgumentNullException( nameof( logger ) );
	}

	public Task<GetPointDetailsDto[]> GetPointsAsync( string uuid )
	{
		return _pointRepo.GetPoints()
			.FilterByUserUuid( uuid )
			.Select( GetPointDetailsDto.FromPointExp )
			.ToArrayAsync( _pointRepo );
	}

	public Task<int> SumPointsAsync( string uuid )
	{
		return _pointRepo.GetPoints()
			.FilterByUserUuid( uuid )
			.SumAsync( _pointRepo, p => p.Value );
	}

	public async Task<GetPointDetailsDto> CreatePoint( CreatePointDto dto, string uuid )
	{
		PointModel point = dto.ToPoint( uuid );

		await _pointRepo.AddPointAsync( point );
		_ = await _pointRepo.SaveAsync();

		return GetPointDetailsDto.FromPoint( point );
	}

}
