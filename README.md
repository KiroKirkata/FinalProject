# Book Catalog — ASP.NET Core Learning Slice

A deliberately small ASP.NET Core 8.0 app built to **demonstrate the architecture**
behind a typical "ASP.NET MVC" course project. It is a vertical slice, not a full
exam submission — it shows each concept working once, with comments explaining why.

## What it demonstrates

| Concept | Where to look |
|---|---|
| Multi-project separation | `BookCatalog.Data` / `.Services` / `.Web` / `.Tests` |
| EF Core + migrations + seed data | `ApplicationDbContext.cs`, `HasData(...)` |
| Two related entities (1-to-many) | `Author.cs`, `Book.cs` |
| Business logic in services | `BookService.cs` (controllers stay thin) |
| Dependency injection | `Program.cs` `AddScoped<IBookService, BookService>()` |
| CRUD via MVC controller | `BooksController.cs` + `Views/Books/*` |
| Web API controller | `BooksApiController.cs` (`/api/books`) |
| AJAX consuming the API | `wwwroot/js/books.js`, Books `Index.cshtml` |
| Identity + custom user property | `ApplicationUser.DisplayName` |
| Admin role + role-gated actions | `SeedData.cs`, `[Authorize(Roles=...)]` |
| Role-aware navigation | `_Layout.cshtml` |
| Client + server validation | data annotations + `_ValidationScriptsPartial` |
| CSRF / XSS protection | `[ValidateAntiForgeryToken]`, `textContent` in JS |
| TempData messages | `_Layout.cshtml` success alert |
| Custom error pages | `Home/Error`, `Home/StatusCode` |
| NUnit unit tests | `BookCatalog.Tests/BookServiceTests.cs` |

## How to run

> Requires the .NET 8 SDK, Visual Studio 2022 (or `dotnet` CLI), and SQL Server
> LocalDB. This code was written as a reference and has **not** been compiled in
> the environment it was generated in — expect to fix a small thing or two as you
> read through it. That is part of the point: type through it, run it, fix it.

```bash
# 1. Create the initial migration (run from the solution folder)
dotnet ef migrations add InitialCreate \
  --project BookCatalog.Data \
  --startup-project BookCatalog.Web

# 2. Run — migrations + admin seeding happen automatically at startup
dotnet run --project BookCatalog.Web

# 3. Run the tests
dotnet test
```

Default admin login: `admin@books.local` / `Admin123!`

## Scope honesty

This slice intentionally has fewer entities/controllers/pages than a full course
rubric (which typically wants 4+ entities, 4+ controllers, 10+ views, 70%+
coverage). It is sized for *understanding*, not for submission. To grow it toward
a full project: add more entities the same way `Book` was added, give each its own
service + controller + views, and extend the test project to match.
