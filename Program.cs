using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;

namespace hw_11_09
{
     class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ApplicationContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                List<User> users = new List<User>()
                { 
                    new User{Email = "Alex@gmail.com", UserSettings = new UserSettings{Country = "Usa", City = "Chicago" } },
                    new User{Email = "Marry@gmail.com", UserSettings = new UserSettings{Country = "Brazil", City = "Salvador" } },
                    new User{Email = "John@gmail.com", UserSettings = new UserSettings{Country = "Canada", City = "Toronto" } }
                };
                db.Users.AddRange(users);
                db.SaveChanges();
            }
            using (var db = new ApplicationContext())
            {
                User currentUser = db.Users.Include(e => e.UserSettings).FirstOrDefault(e => e.Id == 2);
                db.Users.Remove(currentUser);
                db.SaveChanges();

                User userToDelete = db.Users.Include(e => e.UserSettings).FirstOrDefault(e => e.Id == 3);
                if (userToDelete != null)
                {
                    db.Users.Remove(userToDelete);
                    db.SaveChanges();
                }
            }
        }
        public class User
        { 
            public int Id { get; set; }
            public string Email { get; set; }
            public UserSettings UserSettings { get; set; }

        }
        public class UserSettings
        {
            public int Id { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public int UserId { get; set; }
            public User User { get; set; }
        }

       public class ApplicationContext : DbContext
        {
            public DbSet<User> Users { get; set; }
            public DbSet<UserSettings> UserSettings { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-TBASQVJ;Database=testdb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<User>()
                    .HasOne(u => u.UserSettings)
                    .WithOne(us => us.User)
                    .HasForeignKey<UserSettings>(us => us.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                base.OnModelCreating(modelBuilder);
            }
        }

    }
}
