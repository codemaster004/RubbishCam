using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Hubs
{
	public class UserIdProvider : IUserIdProvider
	{
		public string GetUserId( HubConnectionContext connection )
		{
			return connection.User?.Identity?.Name;
		}
	}
}
