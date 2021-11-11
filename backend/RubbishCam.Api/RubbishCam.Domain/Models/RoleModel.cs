using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Domain.Models;

#nullable disable warnings

public class RoleModel
{
	[Key]
	public int Id { get; set; }

	[Required]
	[StringLength( 24 )]
	public string Name { get; set; }


	public List<UserModel>? Users { get; set; }
}
