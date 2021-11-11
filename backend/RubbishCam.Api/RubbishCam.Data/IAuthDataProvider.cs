using Microsoft.EntityFrameworkCore;
using RubbishCam.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RubbishCam.Data;

public interface IAuthDataProvider
{
	DbSet<UserModel> Users { get; }
	DbSet<TokenModel> Tokens { get; }
	DbSet<RoleModel> Roles { get; }
}
