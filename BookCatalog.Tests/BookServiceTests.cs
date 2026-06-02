using BookCatalog.Data;
using BookCatalog.Data.Models;
using BookCatalog.Services;
using BookCatalog.Services.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BookCatalog.Tests;

[TestFixture]
public class BookServiceTests
{
    private ApplicationDbContext _context = null!;
    private BookService _service = null!;

    [SetUp]
    public void SetUp()
    {
        // Each test gets a fresh in-memory database (unique name), so
        // tests don't bleed into each other and we don't need SQL Server.
        // This is how we isolate the unit under test from a real DB.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        _context.Authors.Add(new Author { Id = 1, Name = "Test Author" });
        _context.Books.Add(new Book { Id = 1, Title = "Existing Book", Price = 5m, Year = 2000, AuthorId = 1 });
        _context.SaveChanges();

        _service = new BookService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllBooks()
    {
        var result = await _service.GetAllAsync();
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetByIdAsync_ExistingId_ReturnsBook()
    {
        var result = await _service.GetByIdAsync(1);
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Title, Is.EqualTo("Existing Book"));
    }

    [Test]
    public async Task GetByIdAsync_MissingId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(999);
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateAsync_AddsBook()
    {
        var model = new BookViewModel { Title = "New Book", Price = 10m, Year = 2020, AuthorId = 1 };
        var newId = await _service.CreateAsync(model);

        Assert.That(newId, Is.GreaterThan(0));
        Assert.That(await _context.Books.CountAsync(), Is.EqualTo(2));
    }

    [Test]
    public async Task UpdateAsync_ExistingBook_ChangesValues()
    {
        var model = new BookViewModel { Id = 1, Title = "Updated", Price = 7m, Year = 2001, AuthorId = 1 };
        var ok = await _service.UpdateAsync(model);

        Assert.That(ok, Is.True);
        var updated = await _context.Books.FindAsync(1);
        Assert.That(updated!.Title, Is.EqualTo("Updated"));
    }

    [Test]
    public async Task UpdateAsync_MissingBook_ReturnsFalse()
    {
        var model = new BookViewModel { Id = 999, Title = "Nope", Price = 1m, Year = 2001, AuthorId = 1 };
        var ok = await _service.UpdateAsync(model);
        Assert.That(ok, Is.False);
    }

    [Test]
    public async Task DeleteAsync_ExistingBook_RemovesIt()
    {
        var ok = await _service.DeleteAsync(1);
        Assert.That(ok, Is.True);
        Assert.That(await _context.Books.CountAsync(), Is.EqualTo(0));
    }

    [Test]
    public async Task DeleteAsync_MissingBook_ReturnsFalse()
    {
        var ok = await _service.DeleteAsync(999);
        Assert.That(ok, Is.False);
    }
}
