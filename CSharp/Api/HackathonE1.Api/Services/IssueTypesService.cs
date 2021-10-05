using HackathonE1.Api.Data;
using HackathonE1.Domain.Dtos.IssueType;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Services
{
	public interface IIssueTypesService
	{
		Task<GetIssueTypeDto> CreateIssueTypeAsync( CreateIssueTypeDto issueTypeDto );
		Task<bool> DeleteIssueTypeAsync( int id );
		Task<GetIssueTypeDto> GetIssueTypeAsync( int id );
		Task<GetIssueTypeDto[]> GetIssueTypesAsync();
	}

	public class IssueTypesService : IIssueTypesService
	{
		private readonly AppDbContext _dbContext;

		public IssueTypesService( AppDbContext dbContext )
		{
			_dbContext = dbContext;
		}
		public async Task<GetIssueTypeDto[]> GetIssueTypesAsync()
		{
			return await _dbContext.IssueTypes
				.Select( GetIssueTypeDto.FromIssueTypeModel )
				.ToArrayAsync();
		}

		public async Task<GetIssueTypeDto> GetIssueTypeAsync( int id )
		{
			return await _dbContext.IssueTypes
				.Where( it => it.Id == id )
				.Select( GetIssueTypeDto.FromIssueTypeModel )
				.FirstOrDefaultAsync();
		}


		public async Task<GetIssueTypeDto> CreateIssueTypeAsync( CreateIssueTypeDto issueTypeDto )
		{
			var exists = await _dbContext.IssueTypes
				.Where( it => it.Name == issueTypeDto.Name )
				.Select( GetIssueTypeDto.FromIssueTypeModel )
				.AnyAsync();

			if ( exists )
			{
				return null;
			}

			var issueType = issueTypeDto.ToIssueTypeModel();

			_ = await _dbContext.IssueTypes.AddAsync( issueType );
			_ = await _dbContext.SaveChangesAsync();

			return (GetIssueTypeDto)issueType;
		}


		public async Task<bool> DeleteIssueTypeAsync( int id )
		{
			var issueType = await _dbContext.IssueTypes
				.Where( it => it.Id == id )
				.FirstOrDefaultAsync();

			if ( issueType is null )
			{
				return false;
			}

			_ = _dbContext.IssueTypes.Remove( issueType );
			_ = await _dbContext.SaveChangesAsync();

			return true;
		}
	}
}
