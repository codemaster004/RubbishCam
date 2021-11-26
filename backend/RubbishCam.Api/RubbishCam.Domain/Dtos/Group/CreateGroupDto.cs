using RubbishCam.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace RubbishCam.Domain.Dtos.Group;

#nullable disable warnings

public record CreateGroupDto
{
	[Required]
	public string Name { get; set; }

#nullable restore

	public GroupModel ToGroup( string ownerUuid )
	{
		return new GroupModel( Name )
		{
			TimeCreated = DateTimeOffset.UtcNow,
			MembersR = new()
			{
				new( ownerUuid ) { IsOwner = true }
			}
		};
	}

}
