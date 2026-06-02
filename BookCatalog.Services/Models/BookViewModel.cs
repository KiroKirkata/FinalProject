using System.ComponentModel.DataAnnotations;

namespace BookCatalog.Services.Models;

// A DTO (data transfer object). The service returns these instead of raw
// EF entities, so the web layer never touches the database model directly.
// This is loose coupling: the UI depends on a stable shape, not on EF.
public class BookViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Title { get; set; } = null!;

    [Range(0, 10000)]
    public decimal Price { get; set; }

    [Range(1000, 2100)]
    public int Year { get; set; }

    [Required]
    [Display(Name = "Author")]
    public int AuthorId { get; set; }

    // Read-only, populated when listing. Not bound on create/edit.
    public string? AuthorName { get; set; }
}
