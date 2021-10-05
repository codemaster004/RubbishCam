using HackathonE1.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.RelationModels
{
	public class RoleUserRelation
	{
		public int UserId { get; set; }
		public UserModel User { get; set; }
		public int RoleId { get; set; }
		public RoleModel Role { get; set; }
	}
}
