using HackathonE1.Domain.RelationModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Models
{
	public class RoleModel
	{
		public int Id { get; set; }
		[Required]
		[MaxLength( 16)]
		public string Name { get; set; }

		public List<UserModel> Owners { get; set; }
		public List<RoleUserRelation> RoleUsers { get; set; }
	}
}
