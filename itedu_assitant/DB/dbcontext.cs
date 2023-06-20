using itedu_assitant.Model.Base;
using Microsoft.EntityFrameworkCore;

namespace itedu_assitant.DB
{
    public class dbcontext : DbContext
    {
        public dbcontext(DbContextOptions<dbcontext> options) : base(options) {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Active>()
                .Property(b => b.changeAmount)
                .UseIdentityAlwaysColumn();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var connectVal = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("dbsettings.json")
                .Build()        
                .GetConnectionString("HostConnection");

            builder.UseNpgsql(connectVal);

        }

        public DbSet<Instance> cntuserinstance { get; set;} // not important
        public DbSet<Active> contcurrentactive { get; set; } // not very important
        public DbSet<ManagerNumbers> contmanagernumbers { get; set; } // important
        public DbSet<Access_token> access_tokens { get; set; } // vey important
        public DbSet<Groups> contusergroups { get; set; } // not so much important
        public DbSet<Users> contusers { get; set;  } // important
    }
}   
