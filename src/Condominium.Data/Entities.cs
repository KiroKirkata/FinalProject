using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Condominium.Data;

public static class Roles
{
    public const string Resident = "Resident";
    public const string Manager = "Manager";
    public const string Administrator = "Administrator";
}

public enum OccupancyKind { Owner = 1, User = 2, Occupant = 3 }
public enum TariffMethod { PerApartment = 1, PerResident = 2, PerIdealShare = 3 }
public enum IssueStatus { New = 1, InProgress = 2, Resolved = 3 }
public enum VoteChoice { For = 1, Against = 2, Abstain = 3 }

public class ApplicationUser : IdentityUser
{
    [MaxLength(150)] public string DisplayName { get; set; } = "";
}

public class Building
{
    public int Id { get; set; }
    [MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(250)] public string Address { get; set; } = "";
    [MaxLength(3)] public string Currency { get; set; } = "EUR";
}

public class Apartment
{
    public int Id { get; set; }
    public int BuildingId { get; set; }
    public Building Building { get; set; } = null!;
    [MaxLength(20)] public string Number { get; set; } = "";
    public int Floor { get; set; }
    public int ResidentCount { get; set; }
    public decimal IdealSharePercent { get; set; }
    public ICollection<Resident> Residents { get; set; } = new List<Resident>();
}

// Approved many-to-many membership, distinct from the user's Identity role.
public class Resident
{
    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; } = null!;
    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
    public OccupancyKind Kind { get; set; }
    public bool IsApproved { get; set; }
}

public class BuildingManager
{
    public int BuildingId { get; set; }
    public Building Building { get; set; } = null!;
    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
}

public class Fund
{
    public int Id { get; set; }
    public int BuildingId { get; set; }
    public Building Building { get; set; } = null!;
    [MaxLength(100)] public string Name { get; set; } = "";
}

public class Tariff
{
    public int Id { get; set; }
    public int BuildingId { get; set; }
    public int FundId { get; set; }
    public Fund Fund { get; set; } = null!;
    [MaxLength(100)] public string Name { get; set; } = "";
    public TariffMethod Method { get; set; }
    public decimal Rate { get; set; }
    public DateOnly EffectiveFrom { get; set; }
    public DateOnly? EffectiveTo { get; set; }
}

public class MonthlyFee
{
    public int Id { get; set; }
    public int BuildingId { get; set; }
    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; } = null!;
    public int TariffId { get; set; }
    public Tariff Tariff { get; set; } = null!;
    // Always first day of month; one charge per apartment and month in this initial model.
    public DateOnly Period { get; set; }
    public DateOnly DueDate { get; set; }
    public decimal Amount { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTimeOffset PaidAt { get; set; }
    [MaxLength(100)] public string Reference { get; set; } = "";
}

public class PaymentAllocation
{
    public int PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;
    public int MonthlyFeeId { get; set; }
    public MonthlyFee MonthlyFee { get; set; } = null!;
    public int ApartmentId { get; set; }
    public decimal Amount { get; set; }
}

public class Expense
{
    public int Id { get; set; }
    public int FundId { get; set; }
    public Fund Fund { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    [MaxLength(500)] public string Description { get; set; } = "";
}

public class Issue
{
    public int Id { get; set; }
    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; } = null!;
    public string SubmittedById { get; set; } = "";
    public ApplicationUser SubmittedBy { get; set; } = null!;
    [MaxLength(150)] public string Title { get; set; } = "";
    [MaxLength(2000)] public string Description { get; set; } = "";
    [MaxLength(500)] public string? PhotoPath { get; set; }
    public IssueStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class Voting
{
    public int Id { get; set; }
    public int BuildingId { get; set; }
    public Building Building { get; set; } = null!;
    [MaxLength(200)] public string Title { get; set; } = "";
    [MaxLength(1000)] public string RuleDescription { get; set; } = "";
    public DateTimeOffset OpensAt { get; set; }
    public DateTimeOffset ClosesAt { get; set; }
    public bool IsClosed { get; set; }
    public decimal QuorumPercent { get; set; }
    public decimal MajorityPercent { get; set; }
}

public class Vote
{
    public int VotingId { get; set; }
    public Voting Voting { get; set; } = null!;
    public int ApartmentId { get; set; }
    public Apartment Apartment { get; set; } = null!;
    public int BuildingId { get; set; }
    public string CastById { get; set; } = "";
    public ApplicationUser CastBy { get; set; } = null!;
    public VoteChoice Choice { get; set; }
    public decimal WeightPercent { get; set; }
    public DateTimeOffset CastAt { get; set; }
}

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public ApplicationUser User { get; set; } = null!;
    [MaxLength(500)] public string Message { get; set; } = "";
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
