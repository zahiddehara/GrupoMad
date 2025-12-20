using GrupoMad.Data;
using GrupoMad.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();
// Agregar DbContext con SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<PriceListService>();
builder.Services.AddScoped<QuotationService>();
builder.Services.AddScoped<QuotationPdfService>();

// Configurar autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(48); // Sesión de 48 horas
        options.SlidingExpiration = true; // Renovar automáticamente
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Permitir HTTP (ideal: configurar HTTPS en producción)
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

var app = builder.Build();

// Crear usuario administrador inicial si no existe ningún usuario
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Verificar si existen usuarios
    if (!context.Users.Any())
    {
        var adminUser = new GrupoMad.Models.User
        {
            FirstName = "Administrador",
            LastName = "Sistema",
            Email = "admin@grupomad.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
            PhoneNumber = null,
            Role = GrupoMad.Models.UserRole.Administrator,
            StoreId = null,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(adminUser);
        context.SaveChanges();

        Console.WriteLine("✓ Usuario administrador creado:");
        Console.WriteLine("  Email: admin@grupomad.com");
        Console.WriteLine("  Contraseña: Admin123");
        Console.WriteLine("  ¡IMPORTANTE! Cambia la contraseña después del primer login.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
