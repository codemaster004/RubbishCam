using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HackathonE1.Domain.Models
{
	[Index( nameof( Identifier ), IsUnique = true )]
	[Index( nameof( Email ), IsUnique = true )]
	public class UserModel
	{
		[Key]
		public int Id { get; set; }

		[Required( AllowEmptyStrings = false )]
		[MaxLength( 24 )]
		public string Identifier { get; set; }

		[Required]
		[MaxLength( 32 )]
		public string FirstName { get; set; }

		[Required]
		[MaxLength( 32 )]
		public string LastName { get; set; }

		[Required]
		[MaxLength( 32 )]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[MaxLength( 88 )]
		public string PasswordHash { get; set; }

		public List<ObservedAreaModel> ObservedAreas { get; set; }


		public static string HashPassword( string password )
		{
			// definitely not algorythm for production version, but i have litle time
			// todo: change to proper algorythm

			using var sha = SHA512.Create();
			var passwordHash = sha.ComputeHash( Encoding.UTF8.GetBytes( password ) );
			return Convert.ToBase64String( passwordHash );
		}
	}
}
