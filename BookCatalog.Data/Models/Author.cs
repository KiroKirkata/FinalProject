using System.ComponentModel.DataAnnotations;

namespace BookCatalog.Data.Models;

public class Author
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = null!;

    // Navigation property: one author has many books.
    // EF Core reads this to build the foreign-key relationship.
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
