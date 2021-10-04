using HackathonE1.Api.Data;
using HackathonE1.Domain.Dtos.Issue;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Services
{
	public interface IIssuesService
	{
		Task<GetIssueDto> CreateIssueAsync( CreateIssueDto issueDto );
		Task<bool> DeleteIssueAsync( int id );
		Task<GetIssueDto> GetIssueAsync( int id );
		Task<GetIssueDto[]> GetIssuesAsync();
		Task<GetIssueDto[]> GetIssuesAsync( string userIdentifier );
	}

	public class IssuesService : IIssuesService
	{
		private readonly AppDbContext _dbContext;

		public IssuesService( AppDbContext dbContext )
		{
			_dbContext = dbContext;
		}
		public async Task<GetIssueDto[]> GetIssuesAsync()
		{
			return await _dbContext.Issues
				.Select( GetIssueDto.FromIssueModel )
				.ToArrayAsync();
		}

		public async Task<GetIssueDto[]> GetIssuesAsync( string userIdentifier )
		{
			var issues = from  area in _dbContext.ObservedAreas
						 where area.UserIdentifier == userIdentifier
						 let lat = area.Latitude
						 let @long = area.Longitude

						 from issue in _dbContext.Issues
						 let x = Math.Abs( issue.Longitude - @long )
						 let y = Math.Abs( issue.Latitude - lat )
						 where Math.Sqrt( x * x + y * y ) < area.Radius

						 select issue;

			return await issues
				.Select(GetIssueDto.FromIssueModel)
				.ToArrayAsync();
		}

		public async Task<GetIssueDto> GetIssueAsync( int id )
		{
			return await _dbContext.Issues
				.Where( i => i.Id == id )
				.Select( GetIssueDto.FromIssueModel )
				.FirstOrDefaultAsync();
		}


		public async Task<GetIssueDto> CreateIssueAsync( CreateIssueDto issueDto )
		{
			var exists = await _dbContext.Issues
				.Where( i =>
					i.Latitude == issueDto.Latitude
					&& i.Longitude == issueDto.Longitude
					&& i.TypeId == issueDto.TypeId
				 )
				.Select( GetIssueDto.FromIssueModel )
				.AnyAsync();

			if ( exists )
			{
				return null;
			}

			var issue = issueDto.ToIssueModel();

			_ = await _dbContext.Issues.AddAsync( issue );
			_ = await _dbContext.SaveChangesAsync();

			return (GetIssueDto)issue;
		}


		public async Task<bool> DeleteIssueAsync( int id )
		{
			var issue = await _dbContext.Issues
				.Where( i => i.Id == id )
				.FirstOrDefaultAsync();

			if ( issue is null )
			{
				return false;
			}

			_ = _dbContext.Issues.Remove( issue );
			_ = _dbContext.SaveChangesAsync();

			return true;
		}
	}
}
