using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Condominium.Data;

public static class DemoSeeder
{
    // Development-only, explicit invocation. Password comes from User Secrets or environment.
    public static async Task SeedAsync(ApplicationDbContext db, UserManager<ApplicationUser> users,
        RoleManager<IdentityRole> roles, string password)
    {
        foreach (var role in new[] { Roles.Resident, Roles.Manager, Roles.Administrator })
            if (!await roles.RoleExistsAsync(role)) Ensure(await roles.CreateAsync(new IdentityRole(role)));

        var resident = await User("resident@demo.invalid", "Демо живущ", Roles.Resident);
        var manager = await User("manager@demo.invalid", "Демо домоуправител", Roles.Manager);
        await User("admin@demo.invalid", "Демо администратор", Roles.Administrator);

        // Users/roles are individually repeatable. Domain data is one atomic transaction.
        if (await db.Buildings.AnyAsync(x => x.Name == "Демо вход А")) return;
        await using var tx = await db.Database.BeginTransactionAsync();
        var building = new Building { Name = "Демо вход А", Address = "Примерен адрес 1", Currency = "EUR" };
        db.Buildings.Add(building);
        await db.SaveChangesAsync();
        var a1 = new Apartment { BuildingId = building.Id, Number = "1", Floor = 1, ResidentCount = 2, IdealSharePercent = 40m };
        var a2 = new Apartment { BuildingId = building.Id, Number = "2", Floor = 1, ResidentCount = 3, IdealSharePercent = 35m };
        var a3 = new Apartment { BuildingId = building.Id, Number = "3", Floor = 2, ResidentCount = 1, IdealSharePercent = 25m };
        db.Apartments.AddRange(a1, a2, a3);
        var maintenance = new Fund { BuildingId = building.Id, Name = "Поддръжка" };
        var repair = new Fund { BuildingId = building.Id, Name = "Ремонт и обновяване" };
        db.Funds.AddRange(maintenance, repair);
        await db.SaveChangesAsync();
        db.Residents.Add(new Resident { ApartmentId = a1.Id, UserId = resident.Id, Kind = OccupancyKind.Owner, IsApproved = true });
        db.BuildingManagers.Add(new BuildingManager { BuildingId = building.Id, UserId = manager.Id });
        var tariff = new Tariff { BuildingId = building.Id, FundId = maintenance.Id, Name = "Поддръжка на апартамент",
            Method = TariffMethod.PerApartment, Rate = 20m, EffectiveFrom = new DateOnly(2026, 9, 1) };
        db.Tariffs.AddRange(tariff,
            new Tariff { BuildingId = building.Id, FundId = maintenance.Id, Name = "Пример на лице — неактивен",
                Method = TariffMethod.PerResident, Rate = 5m, EffectiveFrom = new DateOnly(2027, 1, 1) },
            new Tariff { BuildingId = building.Id, FundId = repair.Id, Name = "Ремонт на процент — неактивен",
                Method = TariffMethod.PerIdealShare, Rate = 1m, EffectiveFrom = new DateOnly(2027, 1, 1) });
        await db.SaveChangesAsync();
        // Fixed sample facts, NOT an implementation of monthly billing.
        foreach (var apartment in new[] { a1, a2, a3 })
            foreach (var month in new[] { 9, 10, 11 })
                db.MonthlyFees.Add(new MonthlyFee { BuildingId = building.Id, ApartmentId = apartment.Id,
                    TariffId = tariff.Id, Period = new DateOnly(2026, month, 1), DueDate = new DateOnly(2026, month, 20), Amount = 20m });
        var payment = new Payment { ApartmentId = a1.Id, Amount = 15m,
            PaidAt = new DateTimeOffset(2026, 9, 10, 9, 0, 0, TimeSpan.Zero), Reference = "DEMO-001" };
        db.Payments.Add(payment);
        db.Issues.Add(new Issue { ApartmentId = a1.Id, SubmittedById = resident.Id,
            Title = "Осветление на входа", Description = "Примерен сигнал за неработеща лампа.", Status = IssueStatus.New,
            CreatedAt = new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero) });
        db.Votings.Add(new Voting { BuildingId = building.Id, Title = "Учебна анкета за озеленяване",
            RuleDescription = "Демонстрационна анкета. Параметрите не са правно правило за общо събрание.",
            OpensAt = new DateTimeOffset(2026, 11, 1, 0, 0, 0, TimeSpan.Zero),
            ClosesAt = new DateTimeOffset(2026, 11, 16, 18, 0, 0, TimeSpan.Zero), QuorumPercent = 50m, MajorityPercent = 50m });
        db.Notifications.Add(new Notification { UserId = resident.Id, Message = "Демонстрационните данни са заредени.",
            CreatedAt = new DateTimeOffset(2026, 11, 1, 9, 0, 0, TimeSpan.Zero) });
        await db.SaveChangesAsync();
        var firstFee = await db.MonthlyFees.SingleAsync(x => x.ApartmentId == a1.Id && x.Period == new DateOnly(2026, 9, 1));
        db.PaymentAllocations.Add(new PaymentAllocation { ApartmentId = a1.Id, PaymentId = payment.Id, MonthlyFeeId = firstFee.Id, Amount = 15m });
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        async Task<ApplicationUser> User(string email, string name, string role)
        {
            var user = await users.FindByEmailAsync(email);
            if (user is null)
            {
                user = new ApplicationUser { UserName = email, Email = email, DisplayName = name, EmailConfirmed = true };
                Ensure(await users.CreateAsync(user, password));
            }
            if (!await users.IsInRoleAsync(user, role)) Ensure(await users.AddToRoleAsync(user, role));
            return user;
        }
    }

    private static void Ensure(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(x => x.Description)));
    }
}
