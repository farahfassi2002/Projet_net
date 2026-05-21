using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortefeuilleInvestissement.Components;
using PortefeuilleInvestissement.Data;
using PortefeuilleInvestissement.Models;
using PortefeuilleInvestissement.Services;
using Radzen;
using System.Globalization;  // ← À AJOUTER

var builder = WebApplication.CreateBuilder(args);

// ======== Services Blazor ========
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ======== Services métier ========
builder.Services.AddScoped<IActifService, ActifService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IInvestisseurService, InvestisseurService>();
builder.Services.AddRadzenComponents();

// ======== Base de données ========
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// ======== Authentification ========
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration["BaseUrl"] ?? "http://localhost:5191/")
});

var app = builder.Build();

// ========== CHANGER LA MONNAIE PARTOUT (DT) ==========
var culture = new CultureInfo("fr-FR");
culture.NumberFormat.CurrencySymbol = "DT";
culture.NumberFormat.CurrencyDecimalDigits = 3;
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
// =====================================================

// ======== Seed minimal ========
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    context.Database.EnsureCreated();

    if (!await roleManager.RoleExistsAsync("Administrateur"))
        await roleManager.CreateAsync(new IdentityRole("Administrateur"));

    if (!await roleManager.RoleExistsAsync("Investisseur"))
        await roleManager.CreateAsync(new IdentityRole("Investisseur"));

    if (await userManager.FindByEmailAsync("admin@portefeuille.com") == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com",
            Nom = "Admin",
            Prenom = "Système",
            Role = "Administrateur"
        };
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Administrateur");
            if (!context.Administrateurs.Any(a => a.UserId == adminUser.Id))
            {
                context.Administrateurs.Add(new Administrateur { UserId = adminUser.Id });
                await context.SaveChangesAsync();
            }
        }
    }
}

// ======== Pipeline HTTP ========
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Endpoints d'authentification
app.MapPost("/api/auth/login", async (
    [FromServices] SignInManager<ApplicationUser> signInManager,
    [FromForm] string email,
    [FromForm] string password) =>
{
    var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
    if (result.Succeeded) return Results.Redirect("/dashboard");
    return Results.Redirect("/login?error=Email%20ou%20mot%20de%20passe%20incorrect");
}).DisableAntiforgery();

app.MapPost("/api/auth/logout", async ([FromServices] SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/");
}).DisableAntiforgery();

app.MapPost("/api/auth/register", async (
    [FromServices] UserManager<ApplicationUser> userManager,
    [FromServices] AppDbContext db,
    [FromForm] string email,
    [FromForm] string password,
    [FromForm] string confirmPassword,
    [FromForm] string nom,
    [FromForm] string prenom,
    [FromForm] float budgetInitial) =>
{
    if (password != confirmPassword)
        return Results.Redirect("/signup?error=Les%20mots%20de%20passe%20ne%20correspondent%20pas");

    if (await userManager.FindByEmailAsync(email) != null)
        return Results.Redirect("/signup?error=Cet%20email%20est%20d%C3%A9j%C3%A0%20utilis%C3%A9");

    var newUser = new ApplicationUser
    {
        UserName = email,
        Email = email,
        Nom = nom,
        Prenom = prenom,
        Role = "Investisseur"
    };
    var result = await userManager.CreateAsync(newUser, password);
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(newUser, "Investisseur");
        var inv = new Investisseur
        {
            UserId = newUser.Id,
            BudgetTotal = budgetInitial,
            BudgetDisponible = budgetInitial,
            PointsFidelite = 0
        };
        db.Investisseurs.Add(inv);
        await db.SaveChangesAsync();
        return Results.Redirect("/login?success=Compte%20cr%C3%A9%C3%A9%20avec%20succ%C3%A8s%20!%20Connectez-vous");
    }

    var errors = Uri.EscapeDataString(string.Join(", ", result.Errors.Select(e => e.Description)));
    return Results.Redirect($"/signup?error={errors}");
}).DisableAntiforgery();

app.Run();