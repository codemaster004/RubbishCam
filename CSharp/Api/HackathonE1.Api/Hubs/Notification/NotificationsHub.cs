using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Hubs.Notification
{
	[Authorize]
	public class NotificationsHub : Hub<INotificationClient>
	{
	}
}
