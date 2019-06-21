using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RP.Dashboard.API.Business.Contexts;
using RP.Dashboard.API.Models.Data.DB;

namespace RP.Dashboard.API.Business.Services
{
	public class UserService
	{
		private readonly SqlDbContext _dbContext;

		public UserService(SqlDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<User>> Get()
		{
			return await _dbContext.Users.AsNoTracking().ToListAsync();
		}

		public async Task<User> Get(int id)
		{
			return await _dbContext.Users.FirstOrDefaultAsync(s => s.Id == id);
		}

		public async Task<User> GetByNameIdentifier(string nameIdentifier)
		{
			return await _dbContext.Users.FirstOrDefaultAsync(User => User.NameIdentifier == nameIdentifier);
		}

		public async Task<User> GetBySecret(string secret)
		{
			return await _dbContext.Users.FirstOrDefaultAsync(s => s.UserSecret == secret);
		}

		public async Task<bool> IsEmailRegistered(string email)
		{
			return await _dbContext.Users.FirstOrDefaultAsync(User => User.Email == email) != null;
		}

		public async Task Create(User User)
		{
			await _dbContext.Users.AddAsync(User);

			await _dbContext.SaveChangesAsync();
		}

		public async Task Update(User userIn)
		{
			_dbContext.Entry(userIn).State = EntityState.Modified;
			_dbContext.Users.Update(userIn);

			await _dbContext.SaveChangesAsync();
		}

		public async Task Remove(User userIn)
		{
			_dbContext.Users.Remove(userIn);

			await _dbContext.SaveChangesAsync();
		}

		public async Task Remove(int id)
		{
			var userToRemove = await Get(id);
			await Remove(userToRemove);
		}
	}
}