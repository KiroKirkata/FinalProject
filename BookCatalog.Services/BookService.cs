using BookCatalog.Data;
using BookCatalog.Data.Models;
using BookCatalog.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Services;

// The actual business logic. ALL data access and mapping lives here,
// not in controllers. Controllers just call these methods. This is the
// "business logic in services" requirement.
public class BookService : IBookService
{
    private readonly ApplicationDbContext _context;

    // Constructor injection: DI hands us the DbContext.
    public BookService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BookViewModel>> GetAllAsync()
    {
        // Project entities into DTOs. .AsNoTracking() because this is
        // read-only — a small but real performance/best-practice touch.
        return await _context.Books
            .AsNoTracking()
            .Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Price = b.Price,
                Year = b.Year,
                AuthorId = b.AuthorId,
                AuthorName = b.Author!.Name
            })
            .ToListAsync();
    }

    public async Task<BookViewModel?> GetByIdAsync(int id)
    {
        var book = await _context.Books
            .AsNoTracking()
            .Include(b => b.Author)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book is null)
            return null;

        return new BookViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Price = book.Price,
            Year = book.Year,
            AuthorId = book.AuthorId,
            AuthorName = book.Author?.Name
        };
    }

    public async Task<int> CreateAsync(BookViewModel model)
    {
        var book = new Book
        {
            Title = model.Title,
            Price = model.Price,
            Year = model.Year,
            AuthorId = model.AuthorId
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return book.Id;
    }

    public async Task<bool> UpdateAsync(BookViewModel model)
    {
        var book = await _context.Books.FindAsync(model.Id);
        if (book is null)
            return false;

        book.Title = model.Title;
        book.Price = model.Price;
        book.Year = model.Year;
        book.AuthorId = model.AuthorId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null)
            return false;

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
        return true;
    }
}
