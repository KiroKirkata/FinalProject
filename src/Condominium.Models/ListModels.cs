namespace Condominium.Models;

public record ApartmentViewModel(string Building, string Number, int ResidentCount, decimal IdealSharePercent);
public record FeeViewModel(string Building, string Apartment, DateOnly Period, DateOnly DueDate, decimal Amount, string Currency);
public record PaymentViewModel(string Building, string Apartment, DateTimeOffset PaidAt, decimal Amount, string Currency, string Reference);
public record IssueViewModel(string Building, string Apartment, string Title, string Status);
public record VotingViewModel(string Building, string Title, DateTimeOffset ClosesAt, string RuleDescription);

// Constructed exclusively from authenticated server-side claims by the Web layer.
public record UserAccess(string UserId, bool IsAdministrator, bool IsManager);
