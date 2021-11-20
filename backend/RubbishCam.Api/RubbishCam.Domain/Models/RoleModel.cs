using System.ComponentModel.DataAnnotations;

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
