using HackathonE1.Domain.Dtos.Issue;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Hubs.Notification
{
	public static class NotificationHubExtensions
	{
		public static async Task NotifyUsers( this IHubContext<NotificationsHub, INotificationClient> context, IEnumerable<string> ids, GetIssueDto issue )
		{
			await context.Clients.Users( ids ).ReciveIssue( issue );
		}


	}
}
