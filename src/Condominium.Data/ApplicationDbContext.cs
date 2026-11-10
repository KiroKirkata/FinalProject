using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Condominium.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Resident> Residents => Set<Resident>();
    public DbSet<BuildingManager> BuildingManagers => Set<BuildingManager>();
    public DbSet<Fund> Funds => Set<Fund>();
    public DbSet<Tariff> Tariffs => Set<Tariff>();
    public DbSet<MonthlyFee> MonthlyFees => Set<MonthlyFee>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<PaymentAllocation> PaymentAllocations => Set<PaymentAllocation>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<Voting> Votings => Set<Voting>();
    public DbSet<Vote> Votes => Set<Vote>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void ConfigureConventions(ModelConfigurationBuilder b)
        => b.Properties<decimal>().HavePrecision(18, 2);

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Apartment>(e =>
        {
            e.HasIndex(x => new { x.BuildingId, x.Number }).IsUnique();
            e.HasAlternateKey(x => new { x.BuildingId, x.Id });
            e.Property(x => x.IdealSharePercent).HasPrecision(9, 6);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Apartment_Share", "[IdealSharePercent] > 0 AND [IdealSharePercent] <= 100");
                t.HasCheckConstraint("CK_Apartment_Count", "[ResidentCount] >= 0");
            });
        });
        b.Entity<Resident>(e =>
        {
            e.HasKey(x => new { x.ApartmentId, x.UserId });
            e.HasOne(x => x.Apartment).WithMany(x => x.Residents).HasForeignKey(x => x.ApartmentId);
            e.ToTable(t => t.HasCheckConstraint("CK_Resident_Kind", "[Kind] IN (1,2,3)"));
        });
        b.Entity<BuildingManager>().HasKey(x => new { x.BuildingId, x.UserId });
        b.Entity<Fund>().HasAlternateKey(x => new { x.BuildingId, x.Id });
        b.Entity<Fund>().HasIndex(x => new { x.BuildingId, x.Name }).IsUnique();
        b.Entity<Tariff>(e =>
        {
            e.HasAlternateKey(x => new { x.BuildingId, x.Id });
            e.HasOne(x => x.Fund).WithMany().HasForeignKey(x => new { x.BuildingId, x.FundId })
                .HasPrincipalKey(x => new { x.BuildingId, x.Id });
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Tariff_Rate", "[Rate] > 0");
                t.HasCheckConstraint("CK_Tariff_Method", "[Method] IN (1,2,3)");
                t.HasCheckConstraint("CK_Tariff_Dates", "[EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom]");
            });
        });
        b.Entity<MonthlyFee>(e =>
        {
            e.HasAlternateKey(x => new { x.ApartmentId, x.Id });
            e.HasOne(x => x.Apartment).WithMany().HasForeignKey(x => new { x.BuildingId, x.ApartmentId })
                .HasPrincipalKey(x => new { x.BuildingId, x.Id });
            e.HasOne(x => x.Tariff).WithMany().HasForeignKey(x => new { x.BuildingId, x.TariffId })
                .HasPrincipalKey(x => new { x.BuildingId, x.Id });
            e.HasIndex(x => new { x.ApartmentId, x.Period }).IsUnique();
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_MonthlyFee_Amount", "[Amount] > 0");
                t.HasCheckConstraint("CK_MonthlyFee_Period", "DAY([Period]) = 1");
                t.HasCheckConstraint("CK_MonthlyFee_Due", "[DueDate] >= [Period]");
            });
        });
        b.Entity<Payment>(e =>
        {
            e.HasAlternateKey(x => new { x.ApartmentId, x.Id });
            e.ToTable(t => t.HasCheckConstraint("CK_Payment_Amount", "[Amount] > 0"));
        });
        b.Entity<PaymentAllocation>(e =>
        {
            e.HasKey(x => new { x.PaymentId, x.MonthlyFeeId });
            e.HasOne(x => x.Payment).WithMany().HasForeignKey(x => new { x.ApartmentId, x.PaymentId })
                .HasPrincipalKey(x => new { x.ApartmentId, x.Id });
            e.HasOne(x => x.MonthlyFee).WithMany().HasForeignKey(x => new { x.ApartmentId, x.MonthlyFeeId })
                .HasPrincipalKey(x => new { x.ApartmentId, x.Id });
            e.ToTable(t => t.HasCheckConstraint("CK_Allocation_Amount", "[Amount] > 0"));
        });
        b.Entity<Expense>().ToTable(t => t.HasCheckConstraint("CK_Expense_Amount", "[Amount] > 0"));
        b.Entity<Issue>().ToTable(t => t.HasCheckConstraint("CK_Issue_Status", "[Status] IN (1,2,3)"));
        b.Entity<Voting>(e =>
        {
            e.HasAlternateKey(x => new { x.BuildingId, x.Id });
            e.Property(x => x.QuorumPercent).HasPrecision(9, 6);
            e.Property(x => x.MajorityPercent).HasPrecision(9, 6);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Voting_Dates", "[ClosesAt] > [OpensAt]");
                t.HasCheckConstraint("CK_Voting_Thresholds", "[QuorumPercent] BETWEEN 0 AND 100 AND [MajorityPercent] BETWEEN 0 AND 100");
            });
        });
        b.Entity<Vote>(e =>
        {
            e.HasKey(x => new { x.VotingId, x.ApartmentId });
            e.Property(x => x.WeightPercent).HasPrecision(9, 6);
            e.HasOne(x => x.Voting).WithMany().HasForeignKey(x => new { x.BuildingId, x.VotingId })
                .HasPrincipalKey(x => new { x.BuildingId, x.Id });
            e.HasOne(x => x.Apartment).WithMany().HasForeignKey(x => new { x.BuildingId, x.ApartmentId })
                .HasPrincipalKey(x => new { x.BuildingId, x.Id });
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Vote_Weight", "[WeightPercent] > 0 AND [WeightPercent] <= 100");
                t.HasCheckConstraint("CK_Vote_Choice", "[Choice] IN (1,2,3)");
            });
        });

        // Preserve financial history, and avoid SQL Server multiple cascade paths.
        foreach (var fk in b.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
}
