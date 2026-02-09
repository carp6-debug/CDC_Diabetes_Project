namespace CDC_Diabetes.Data;

using Microsoft.EntityFrameworkCore;
using CDC_Diabetes.Models; // Links to your Models project

public class HealthcareDbContext : DbContext
{
    public HealthcareDbContext(DbContextOptions<HealthcareDbContext> options)
        : base(options)
    {
    }

    // These properties represent our tables
    public DbSet<FactDiabetesSurvey> FactSurveys { get; set; } = null!;
    public DbSet<DimDemographics> Demographics { get; set; } = null!;
    public DbSet<DimLifestyle> LifestyleFactors { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tell EF Core to use the specific PostgreSQL schema we created
        modelBuilder.HasDefaultSchema("healthcare_analytics");

        // Define decimal precision for BMI to match our SQL NUMERIC(4,1)
        modelBuilder.Entity<FactDiabetesSurvey>()
            .Property(f => f.Bmi)
            .HasPrecision(4, 1);

        base.OnModelCreating(modelBuilder);
    }
}