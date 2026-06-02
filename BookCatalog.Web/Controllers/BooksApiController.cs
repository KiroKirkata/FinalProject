using BookCatalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.Web.Controllers;

// This is the API controller (the requirement asks for at least one).
// [ApiController] + route attribute = returns JSON, not views.
// The AJAX call on the Books index page fetches from /api/books.
[ApiController]
[Route("api/books")]
public class BooksApiController : ControllerBase
{
    private readonly IBookService _books;

    public BooksApiController(IBookService books)
    {
        _books = books;
    }

    // GET /api/books  -> JSON array of all books.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var books = await _books.GetAllAsync();
        return Ok(books);
    }

    // GET /api/books/5 -> single book as JSON, or 404.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book is null)
            return NotFound();
        return Ok(book);
    }
}
