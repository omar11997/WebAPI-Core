using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chrestin_Project.Model
{
	public class Context : IdentityDbContext<ApplicationUser>
	{
		public DbSet<Department> Departments { get; set; }
		public Context() { }
		public Context(DbContextOptions options ) : base(options)
		{

		}
	}
}
