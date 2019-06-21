using Microsoft.EntityFrameworkCore;
using RP.Dashboard.API.Models.Data.DB;

namespace RP.Dashboard.API.Business.Contexts
{
	public class SqlDbContext : DbContext
	{
		public DbSet<Goal> Goals { get; set; }
		public DbSet<User> Users { get; set; }

		public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Goal>().ToTable("Goals");
			modelBuilder.Entity<User>().ToTable("Users");
		}
	}
}

// Create/Add Migration
// dotnet ef migrations add InitialCreate

// Run it
// dotnet ef database update