using Condominium.Data;
using Condominium.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Задайте ConnectionStrings:DefaultConnection чрез User Secrets или средата. Вижте README.md.");
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(connection));
builder.Services.AddDefaultIdentity<ApplicationUser>(o =>
{
    o.SignIn.RequireConfirmedAccount = false; // Local skeleton; no email provider at stage 1.
    o.User.RequireUniqueEmail = true;
    o.Password.RequiredLength = 12;
    o.Lockout.MaxFailedAccessAttempts = 5;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(o =>
{
    o.Cookie.HttpOnly = true;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    o.Cookie.SameSite = SameSiteMode.Lax;
});
builder.Services.AddControllersWithViews(o => o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddRazorPages();
builder.Services.AddScoped<IOverviewService, OverviewService>();
var app = builder.Build();

if (args.Contains("--initialize-db"))
{
    if (!app.Environment.IsDevelopment())
        throw new InvalidOperationException("Началните демо данни са разрешени само в Development.");
    var password = builder.Configuration["Seed:Password"]
        ?? throw new InvalidOperationException("Задайте Seed:Password в User Secrets преди инициализацията.");
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
    await DemoSeeder.SeedAsync(db, scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(), password);
    Console.WriteLine("Базата данни и демонстрационните профили са инициализирани.");
    return;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
