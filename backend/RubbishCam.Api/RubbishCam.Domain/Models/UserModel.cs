using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Domain.Models;


#nullable disable warnings

[Index( nameof( Uuid ), IsUnique = true )]
[Index( nameof( UserName ), IsUnique = true )]
public class UserModel
{
	[Key]
	public int Id { get; set; }
	[Required]
	[StringLength( 24 )]
	public string Uuid { get; set; }

	[Required]
	[StringLength( 50 )]
	public string FirstName { get; set; }

	[Required]
	[StringLength( 50 )]
	public string LastName { get; set; }

	[Required]
	public string PasswordHash { get; set; }

	[Required]
	[StringLength( 32 )]
	public string UserName { get; set; }


	public List<TokenModel> Tokens { get; set; }

	public List<RoleModel> Roles { get; set; }

}
