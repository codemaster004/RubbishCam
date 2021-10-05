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
		private readonly IEmailService _emailService;

		public NotificationsService( IHubContext<NotificationsHub, INotificationClient> hubContext, AppDbContext dbContext, IEmailService emailService )
		{
			_hubContext = hubContext;
			_dbContext = dbContext;
			_emailService = emailService;
		}

		public async Task NotifyAsync( IssueModel issue )
		{
			await Task.WhenAll( NotifySignalrAsync( issue ), NotifyEmailAsync( issue ) );
		}

		private async Task NotifySignalrAsync( IssueModel issue )
		{
			var identifiers = await GetRelatedUserIdentifiersAsync( issue );

			await _hubContext.NotifyUsers( identifiers, (GetIssueDto)issue );
		}
		private async Task NotifyEmailAsync( IssueModel issue )
		{
			var emails = await GetRelatedUsersEmailsAsync( issue );
			List<Task> tasks = new( emails.Length );

			foreach ( var email in emails )
			{
				var task = _emailService.SendEmailAsync( email, "New issue in your observed area", "New issue in your area" );
				tasks.Add( task );
			}

			await Task.WhenAll( tasks );
		}


		private async Task<string[]> GetRelatedUserIdentifiersAsync( IssueModel issue )
		{
			var lat = issue.Latitude;
			var @long = issue.Longitude;

			var identifiers = from UserModel user in _dbContext.Users
							  join ObservedAreaModel area in _dbContext.ObservedAreas
							  on user.Identifier equals area.UserIdentifier
							  let x = Math.Abs( area.Longitude - @long )
							  let y = Math.Abs( area.Latitude - lat )
							  where Math.Sqrt( x * x + y * y ) < area.Radius
							  select user.Identifier;

			return await identifiers.ToArrayAsync();
		}

		private async Task<string[]> GetRelatedUsersEmailsAsync( IssueModel issue )
		{
			var lat = issue.Latitude;
			var @long = issue.Longitude;

			var emails = from UserModel user in _dbContext.Users
						 where user.ReciveEmails
						 join ObservedAreaModel area in _dbContext.ObservedAreas
						 on user.Identifier equals area.UserIdentifier
						 let x = Math.Abs( area.Longitude - @long )
						 let y = Math.Abs( area.Latitude - lat )
						 where Math.Sqrt( x * x + y * y ) < area.Radius
						 select user.Email;

			return await emails.ToArrayAsync();
		}

	}
}
