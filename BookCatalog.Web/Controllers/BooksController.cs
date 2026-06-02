using BookCatalog.Services;
using BookCatalog.Services.Models;
using BookCatalog.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCatalog.Web.Controllers;

public class BooksController : Controller
{
    private readonly IBookService _books;

    // The controller depends on the INTERFACE. DI supplies the concrete
    // service. The controller has no idea EF Core exists.
    public BooksController(IBookService books)
    {
        _books = books;
    }

    // Anyone can view the list.
    public async Task<IActionResult> Index()
    {
        var model = await _books.GetAllAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book is null)
            return NotFound();
        return View(book);
    }

    // Only admins can create. Roles attribute enforces it server-side.
    [Authorize(Roles = SeedData.AdminRole)]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken] // CSRF protection
    [Authorize(Roles = SeedData.AdminRole)]
    public async Task<IActionResult> Create(BookViewModel model)
    {
        // Server-side validation: never trust the client.
        if (!ModelState.IsValid)
            return View(model);

        await _books.CreateAsync(model);
        TempData["Success"] = $"Book \"{model.Title}\" was created.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = SeedData.AdminRole)]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book is null)
            return NotFound();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SeedData.AdminRole)]
    public async Task<IActionResult> Edit(BookViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var updated = await _books.UpdateAsync(model);
        if (!updated)
            return NotFound();

        TempData["Success"] = $"Book \"{model.Title}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = SeedData.AdminRole)]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book is null)
            return NotFound();
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = SeedData.AdminRole)]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _books.DeleteAsync(id);
        TempData["Success"] = "Book was deleted.";
        return RedirectToAction(nameof(Index));
    }
}
