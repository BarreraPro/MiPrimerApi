using Microsoft.EntityFrameworkCore;
using MiPrimerAPI_.Models;

namespace MiPrimerAPI_.Data
{
    public class AppDbContext :  DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
