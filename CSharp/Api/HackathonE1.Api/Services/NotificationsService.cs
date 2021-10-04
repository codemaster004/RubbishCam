using HackathonE1.Api.Data;
using HackathonE1.Api.Hubs.Notification;
using HackathonE1.Domain.Dtos.Issue;
using HackathonE1.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HackathonE1.Api.Services
{
	public interface INotificationsService
	{
		Task NotifyAsync( IssueModel issue );
	}

	public class NotificationsService : INotificationsService
	{
		private readonly IHubContext<NotificationsHub, INotificationClient> _hubContext;
		private readonly AppDbContext _dbContext;

		public NotificationsService( IHubContext<NotificationsHub, INotificationClient> hubContext, AppDbContext dbContext )
		{
			_hubContext = hubContext;
			_dbContext = dbContext;
		}

		public async Task NotifyAsync( IssueModel issue )
		{
			var users = await GetRelatedUserIdentifiersAsync( issue );
			await _hubContext.NotifyUsers( users, (GetIssueDto)issue );
		}

		private async Task<string[]> GetRelatedUserIdentifiersAsync( IssueModel issue )
		{
			var lat = issue.Latitude;
			var @long = issue.Longitude;

			var users = from UserModel user in _dbContext.Users
						join ObservedAreaModel area in _dbContext.ObservedAreas
						on user.Id equals area.Id
						let x = Math.Abs( area.Longitude - @long )
						let y = Math.Abs( area.Latitude - lat )
						where Math.Sqrt( x * x + y * y ) < area.Radius
						select user.Identifier;

			return await users.ToArrayAsync();
		}

	}
}
