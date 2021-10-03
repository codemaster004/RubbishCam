using HackathonE1.Domain.Dtos.Issue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Hubs.Notification
{
	public interface INotificationClient
	{
		Task ReciveIssue( GetIssueDto issue );
	}
}
