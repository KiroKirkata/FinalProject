using Condominium.Models;

namespace Condominium.Services;

public interface IOverviewService
{
    Task<List<ApartmentViewModel>> ApartmentsAsync(UserAccess access);
    Task<List<FeeViewModel>> FeesAsync(UserAccess access);
    Task<List<PaymentViewModel>> PaymentsAsync(UserAccess access);
    Task<List<IssueViewModel>> IssuesAsync(UserAccess access);
    Task<List<VotingViewModel>> VotingsAsync(UserAccess access);
}
