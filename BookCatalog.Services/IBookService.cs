using BookCatalog.Services.Models;

namespace BookCatalog.Services;

// Controllers depend on THIS, never on the concrete BookService.
// That indirection is what lets us mock the service in unit tests
// and what keeps the controller loosely coupled from EF Core.
public interface IBookService
{
    Task<IEnumerable<BookViewModel>> GetAllAsync();
    Task<BookViewModel?> GetByIdAsync(int id);
    Task<int> CreateAsync(BookViewModel model);
    Task<bool> UpdateAsync(BookViewModel model);
    Task<bool> DeleteAsync(int id);
}
