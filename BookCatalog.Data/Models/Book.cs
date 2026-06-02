using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCatalog.Data.Models;

public class Book
{
    public int Id { get; set; }

    // [Required] and [StringLength] are data-annotation validation.
    // They drive BOTH server-side validation (ModelState) AND the
    // client-side validation attributes Razor emits into the HTML.
    // That single source covers the "client-side and server-side
    // validation" requirement.
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Title { get; set; } = null!;

    [Range(0, 10000)]
    public decimal Price { get; set; }

    [Range(1000, 2100)]
    public int Year { get; set; }

    // Foreign key + navigation property = the "many" side of the
    // one-to-many relationship with Author.
    public int AuthorId { get; set; }

    [ForeignKey(nameof(AuthorId))]
    public Author? Author { get; set; }
}
