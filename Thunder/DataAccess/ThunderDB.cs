using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Thunder.Models;

namespace Thunder.DataAccess
{
    public class ThunderDB : DbContext
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public ThunderDB(IConfiguration _configuration, ILoggerFactory _logger, IHttpContextAccessor _httpContextAccessor)
        {
            configuration = _configuration;
            logger = _logger;
            httpContextAccessor = _httpContextAccessor;
        }

        public DbSet<User> User { set; get; }
        public DbSet<Role> Role { set; get; }
        public DbSet<University> University { set; get; }
        public DbSet<City> City { set; get; }   
        public DbSet<Province> Province { set; get; }
        public DbSet<Facility> Facility { set; get; }
        public DbSet<UniversityFacility> UniversityFacility { set; get; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            dbContextOptionsBuilder.UseLoggerFactory(logger);
            dbContextOptionsBuilder.EnableSensitiveDataLogging();
            dbContextOptionsBuilder.UseMySQL(configuration.GetConnectionString("ThunderDB"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UniversityFacility>(entity =>
            {
                entity.HasOne(column => column.University)
                .WithMany(column => column.UniversityFacilities)
                .HasForeignKey(column => column.UniversityId);
            });

        }

    }
}
