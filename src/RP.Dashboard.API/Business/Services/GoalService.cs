using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RP.Dashboard.API.Business.Contexts;
using RP.Dashboard.API.Models.Data.DB;

namespace RP.Dashboard.API.Business.Services
{
	public class GoalService
	{
		private readonly SqlDbContext _dbContext;

		public GoalService(SqlDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<Goal>> Get()
		{
			return await _dbContext.Goals.AsNoTracking().ToListAsync();
		}

		public async Task<Goal> Get(int id)
		{
			return await _dbContext.Goals.FirstOrDefaultAsync(s => s.Id == id);
		}

		public async Task<IEnumerable<Goal>> GetForUserId(int userId)
		{
			return await _dbContext.Goals.Where(w => w.UserId == userId)?.ToListAsync();
		}

		public async Task Create(Goal goal)
		{
			await _dbContext.Goals.AddAsync(goal);

			await _dbContext.SaveChangesAsync();
		}

		public async Task<bool> IsGoalCreated(int userId, string dailyGoalTrackID)
		{
			return await _dbContext.Goals.FirstOrDefaultAsync(Goal => Goal.UserId == userId && Goal.DailyGoalTogglID == dailyGoalTrackID) != null;
		}

		public async Task Update(Goal goalIn)
		{
			_dbContext.Entry(goalIn).State = EntityState.Modified;
			_dbContext.Goals.Update(goalIn);

			await _dbContext.SaveChangesAsync();
		}

		public async Task Remove(Goal goalIn)
		{
			_dbContext.Goals.Remove(goalIn);

			await _dbContext.SaveChangesAsync();
		}

		public async Task RemoveAsync(int id)
		{
			var goalToRemove = await Get(id);
			await Remove(goalToRemove);
		}
	}
}