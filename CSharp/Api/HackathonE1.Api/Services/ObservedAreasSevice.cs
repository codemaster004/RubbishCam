using HackathonE1.Api.Data;
using HackathonE1.Domain.Dtos.ObservedArea;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Services
{
	public interface IObservedAreasService
	{
		Task<bool> DeleteAreaAsync( int id );
		Task<GetObservedAreaDto> GetAreaAsync( int id );
		Task<GetObservedAreaDto> GetAreaAsync( int id, string userIdentifier );
		Task<GetObservedAreaDto[]> GetAreasAsync();
		Task<GetObservedAreaDto[]> GetUserAreasAsync( string userIdentifier );
		Task<GetObservedAreaDto> ObserveAreaAsync( CreateObservedAreaDto areaDto, string userIdentifier );
		Task<bool> DeleteAreaAsync( int id, string userIdentifier );
	}

	public class ObservedAreasService : IObservedAreasService
	{
		private readonly AppDbContext _dbContext;

		public ObservedAreasService( AppDbContext dbContext )
		{
			_dbContext = dbContext;
		}

		public async Task<GetObservedAreaDto[]> GetAreasAsync()
		{
			return await _dbContext.ObservedAreas
				.Select( GetObservedAreaDto.FromObservedAreaModel )
				.ToArrayAsync();
		}

		public async Task<GetObservedAreaDto[]> GetUserAreasAsync( string userIdentifier )
		{
			return await _dbContext.ObservedAreas
				.Where( oa => oa.UserIdentifier == userIdentifier )
				.Select( GetObservedAreaDto.FromObservedAreaModel )
				.ToArrayAsync();
		}

		public async Task<GetObservedAreaDto> GetAreaAsync( int id )
		{
			return await _dbContext.ObservedAreas
				.Where( oa => oa.Id == id )
				.Select( GetObservedAreaDto.FromObservedAreaModel )
				.FirstOrDefaultAsync();
		}

		public async Task<GetObservedAreaDto> GetAreaAsync( int id, string userIdentifier )
		{
			return await _dbContext.ObservedAreas
				.Where( oa => oa.Id == id
					&& oa.UserIdentifier == userIdentifier )
				.Select( GetObservedAreaDto.FromObservedAreaModel )
				.FirstOrDefaultAsync();
		}

		public async Task<GetObservedAreaDto> ObserveAreaAsync( CreateObservedAreaDto areaDto, string userIdentifier )
		{
			var exists = await _dbContext.ObservedAreas
				.Where( oa =>
					oa.Latitude == areaDto.Latitude
					&& oa.Longitude == areaDto.Longitude
					&& oa.Radius == areaDto.Radius
					&& oa.UserIdentifier == userIdentifier
				 )
				.Select( GetObservedAreaDto.FromObservedAreaModel )
				.AnyAsync();

			if ( exists )
			{
				return null;
			}

			var area = areaDto.ToObservedAreaModel();

			_ = await _dbContext.ObservedAreas.AddAsync( area );
			_ = await _dbContext.SaveChangesAsync();

			return (GetObservedAreaDto)area;
		}


		public async Task<bool> DeleteAreaAsync( int id )
		{
			var area = await _dbContext.ObservedAreas
				.Where( oa => oa.Id == id )
				.FirstOrDefaultAsync();

			if ( area is null )
			{
				return false;
			}

			_ = _dbContext.ObservedAreas.Remove( area );
			_ = _dbContext.SaveChangesAsync();

			return true;
		}

		public async Task<bool> DeleteAreaAsync( int id, string userIdentifier )
		{
			var area = await _dbContext.ObservedAreas
				.Where( oa => oa.Id == id
					&& oa.UserIdentifier == userIdentifier )
				.FirstOrDefaultAsync();

			if ( area is null )
			{
				return false;
			}

			_ = _dbContext.ObservedAreas.Remove( area );
			_ = _dbContext.SaveChangesAsync();

			return true;
		}
	}
}
