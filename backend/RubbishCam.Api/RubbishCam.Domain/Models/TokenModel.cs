using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Domain.Models;

#nullable disable warnings

public class TokenModel
{
	[Key]
	public int Id { get; set; }

	public string Token { get; set; }

	public string UserUuid { get; set; }
	public UserModel User { get; set; }

	public DateTimeOffset ValidUntil { get; set; }
}
