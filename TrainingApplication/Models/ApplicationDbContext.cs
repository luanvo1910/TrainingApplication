using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace TrainingApplication.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Staff> Staffs { get; set; }

        public DbSet<Trainer> Trainers { get; set; }

        public DbSet<Trainee> Trainees { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CoursesTrainer> CoursesTrainer { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}