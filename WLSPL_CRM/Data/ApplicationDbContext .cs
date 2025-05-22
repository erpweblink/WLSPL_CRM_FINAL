using Microsoft.EntityFrameworkCore;

namespace WLSPL_CRM_2.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor for injecting DbContext options
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        // DbSet for Employee model (make sure Employee class exists in your project)
        //public DbSet<Employee> Employees { get; set; }
    }
}
