using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HWT.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        // Use your actual connection string here or read from config
        optionsBuilder.UseNpgsql(
            "Host=ep-misty-lab-aefnvhul.c-2.us-east-2.aws.neon.tech;Database=packTracker;Username=neondb_owner;Password=npg_a3nKCGfdk4UR;Port=5432;SSL Mode=Require");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}