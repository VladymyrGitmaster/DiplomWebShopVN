using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiplomWebShopVN.Models.Domain
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; } = null!;    
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<BasketElement> BasketElements { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<RatingProduct> RatingProducts { get; set; } = null!;
        public DbSet<ProductComent> ProductComents { get; set; } = null!;
        public DbSet<Photo> Photos { get; set; } = null!;

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            //Database.EnsureCreated();
        }
    }
}
