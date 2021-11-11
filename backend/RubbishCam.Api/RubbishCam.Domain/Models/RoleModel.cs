using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Domain.Models;

public class RoleModel
{
	public RoleModel( string name, List<UserModel> users )
	{
		Name = name;
		Users = users;
	}

	public RoleModel( string name )
		: this( name, new() )
	{
	}

	[Key]
	public int Id { get; set; }

	[Required]
	[StringLength( 24 )]
	public string Name { get; set; }


	public List<UserModel> Users { get; set; }
}
