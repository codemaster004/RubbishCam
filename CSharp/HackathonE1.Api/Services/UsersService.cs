using HackathonE1.Api.Data;
using HackathonE1.Domain.Dtos.User;
using HackathonE1.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HackathonE1.Api.Services
{
	public interface IUsersService
	{
		Task<GetUserDto> AddUserAsync( CreateUserDto Dto );
		Task<bool> DeleteUser( string identifier );
		Task<GetUserDto> GetUser( string identifier );
		Task<GetUserDto[]> GetUsers();
	}

	public class UsersService : IUsersService
	{
		private readonly AppDbContext _dbContext;

		public UsersService( AppDbContext dbContext )
		{
			_dbContext = dbContext;
		}
		public async Task<GetUserDto[]> GetUsers()
		{
			return await _dbContext.Users
				.Select( GetUserDto.FromUserModel )
				.ToArrayAsync();
		}

		public async Task<GetUserDto> GetUser( string identifier )
		{
			return await _dbContext.Users
				.Where( u => u.Identifier == identifier )
				.Select( GetUserDto.FromUserModel )
				.FirstOrDefaultAsync();
		}


		public async Task<GetUserDto> AddUserAsync( CreateUserDto userDto )
		{
			var exists = await _dbContext.Users
				.Where( u => u.Email == userDto.Email )
				.Select( GetUserDto.FromUserModel )
				.AnyAsync();

			if ( exists )
			{
				return null;
			}

			var user = userDto.ToUserModel();

			user.Identifier = await GenerateIdentifierAsync();

			_ = await _dbContext.AddAsync( userDto );
			_ = await _dbContext.SaveChangesAsync();

			return (GetUserDto)user;
		}
		private async Task<string> GenerateIdentifierAsync()
		{
			string identifier;
			bool used;

			do
			{
				var guid = Guid.NewGuid();
				identifier = Convert.ToBase64String( guid.ToByteArray() );

				used = await _dbContext.Users.Where( u => u.Identifier == identifier ).AnyAsync();
			} while ( used );

			return identifier;
		}


		public async Task<bool> DeleteUser( string identifier )
		{
			var user = await _dbContext.Users
				.Where( u => u.Identifier == identifier )
				.FirstOrDefaultAsync();

			if ( user is null )
			{
				return false;
			}

			_ = _dbContext.Users.Remove( user );
			_ = _dbContext.SaveChangesAsync();

			return true;
		}

	}
}
