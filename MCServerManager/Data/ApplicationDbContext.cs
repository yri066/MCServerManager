using MCServerManager.Library.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MCService = MCServerManager.Library.Data.Models.Service;

namespace MCServerManager.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		public DbSet<Server> Servers { get; set; }
        public DbSet<MCService> Services { get; set; }
    }
}