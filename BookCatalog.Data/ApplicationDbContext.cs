using BookCatalog.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookCatalog.Data;

// We inherit from IdentityDbContext<ApplicationUser> rather than plain
// DbContext. That single choice wires up ALL the Identity tables
// (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.) automatically.
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Each DbSet becomes a table. These are the entities we query.
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Book> Books { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // MUST call base first so Identity configures its own tables.
        base.OnModelCreating(builder);

        // Seeded data: EF Core writes these rows into the migration,
        // so a fresh database starts populated. Satisfies the
        // "seeded data" requirement. (User/role seeding happens at
        // startup in the Web project, because password hashing needs
        // runtime services.)
        builder.Entity<Author>().HasData(
            new Author { Id = 1, Name = "George Orwell" },
            new Author { Id = 2, Name = "Jane Austen" }
        );

        builder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "1984", Price = 12.99m, Year = 1949, AuthorId = 1 },
            new Book { Id = 2, Title = "Animal Farm", Price = 9.99m, Year = 1945, AuthorId = 1 },
            new Book { Id = 3, Title = "Pride and Prejudice", Price = 8.50m, Year = 1813, AuthorId = 2 }
        );
    }
}
