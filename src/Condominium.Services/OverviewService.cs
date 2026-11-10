using Condominium.Data;
using Condominium.Models;
using Microsoft.EntityFrameworkCore;

namespace Condominium.Services;

// Stage 1 read-only queries. Billing/payment/voting commands are intentionally deferred.
public class OverviewService(ApplicationDbContext db) : IOverviewService
{
    private IQueryable<Apartment> VisibleApartments(UserAccess access)
    {
        var query = db.Apartments.AsNoTracking();
        if (access.IsAdministrator) return query;
        return query.Where(a =>
            (access.IsManager && db.BuildingManagers.Any(m => m.BuildingId == a.BuildingId && m.UserId == access.UserId))
            || a.Residents.Any(r => r.UserId == access.UserId && r.IsApproved));
    }

    public Task<List<ApartmentViewModel>> ApartmentsAsync(UserAccess access)
        => VisibleApartments(access).OrderBy(a => a.BuildingId).ThenBy(a => a.Number)
            .Select(a => new ApartmentViewModel(a.Building.Name, a.Number, a.ResidentCount, a.IdealSharePercent)).ToListAsync();

    public Task<List<FeeViewModel>> FeesAsync(UserAccess access)
    {
        var ids = VisibleApartments(access).Select(a => a.Id);
        return db.MonthlyFees.AsNoTracking().Where(f => ids.Contains(f.ApartmentId))
            .OrderBy(f => f.Period).ThenBy(f => f.ApartmentId)
            .Select(f => new FeeViewModel(f.Apartment.Building.Name, f.Apartment.Number, f.Period, f.DueDate,
                f.Amount, f.Apartment.Building.Currency)).ToListAsync();
    }

    public Task<List<PaymentViewModel>> PaymentsAsync(UserAccess access)
    {
        var ids = VisibleApartments(access).Select(a => a.Id);
        return db.Payments.AsNoTracking().Where(p => ids.Contains(p.ApartmentId)).OrderByDescending(p => p.PaidAt)
            .Select(p => new PaymentViewModel(p.Apartment.Building.Name, p.Apartment.Number, p.PaidAt,
                p.Amount, p.Apartment.Building.Currency, p.Reference)).ToListAsync();
    }

    public async Task<List<IssueViewModel>> IssuesAsync(UserAccess access)
    {
        var ids = VisibleApartments(access).Select(a => a.Id);
        var rows = await db.Issues.AsNoTracking().Where(i => ids.Contains(i.ApartmentId))
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new { Building = i.Apartment.Building.Name, Apartment = i.Apartment.Number, i.Title, i.Status }).ToListAsync();
        return rows.Select(i => new IssueViewModel(i.Building, i.Apartment, i.Title, i.Status switch
        { IssueStatus.New => "Нов", IssueStatus.InProgress => "В процес", IssueStatus.Resolved => "Решен", _ => "Неизвестен" })).ToList();
    }

    public Task<List<VotingViewModel>> VotingsAsync(UserAccess access)
    {
        var buildings = VisibleApartments(access).Select(a => a.BuildingId);
        return db.Votings.AsNoTracking().Where(v => access.IsAdministrator || buildings.Contains(v.BuildingId))
            .OrderBy(v => v.ClosesAt)
            .Select(v => new VotingViewModel(v.Building.Name, v.Title, v.ClosesAt, v.RuleDescription)).ToListAsync();
    }
}
