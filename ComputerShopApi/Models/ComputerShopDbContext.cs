using Microsoft.EntityFrameworkCore;

namespace ComputerShopApi.Models
{
    public class ComputerShopDbContext : DbContext
    {
        public ComputerShopDbContext(DbContextOptions options) : base(options)
        {
        }

        public ComputerShopDbContext()
        {
        }

        public DbSet<Osystem> Osystems { get; set; }
        public DbSet<Computers> Computers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<ComputerComponent> ComputerComponents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // A kapcsolótábla összetett elsődleges kulcsának (Composite Primary Key) megadása
            modelBuilder.Entity<ComputerComponent>()
                .HasKey(cc => new { cc.ComputerId, cc.ComponentId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var conn = "Server=db74563.databaseasp.net; Database=db74563; Uid=db74563; Pwd=M+j9wS2-Z!p5;";

            optionsBuilder.UseMySql(conn, new MySqlServerVersion(new Version(8, 0, 30)));
        }
    }
}
