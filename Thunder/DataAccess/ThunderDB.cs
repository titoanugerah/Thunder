using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Thunder.Models;

namespace Thunder.DataAccess
{
    public class ThunderDB : DbContext
    {
        private readonly IConfiguration configuration;
        private readonly ILoggerFactory logger;
        //private readonly IHttpContextAccessor httpContextAccessor;

        public ThunderDB(IConfiguration _configuration, ILoggerFactory _logger)
        {
            configuration = _configuration;
            logger = _logger;
            //httpContextAccessor = _httpContextAccessor;
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
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 27));            
            dbContextOptionsBuilder
                .UseMySql(configuration.GetConnectionString("ThunderDB"), serverVersion)
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(logger);
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
