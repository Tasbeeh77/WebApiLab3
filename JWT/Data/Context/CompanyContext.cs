using JWT.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWT.Data.Context
{
    public class CompanyContext : IdentityDbContext<Users>
    {
        public CompanyContext(DbContextOptions<CompanyContext> options):base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Users>().ToTable("Users");
        }
    }
}
