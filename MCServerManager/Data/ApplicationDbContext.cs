using MCServerManager.Library.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MCServerManager.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Server> Servers { get; set; }
        public DbSet<Library.Data.Models.Service> Services { get; set; }
    }
}