using ClinicaMedEduardoMorenoMVCWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registrar el contexto de base de datos
builder.Services.AddDbContext<ClinicaMedEduardoMorenoMVCWeb.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

// Configuración del servicio de correo con MailKit
builder.Services.Configure<ClinicaMedEduardoMorenoMVCWeb.Models.EmailSettings>(
builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<ClinicaMedEduardoMorenoMVCWeb.Services.IEmailService, ClinicaMedEduardoMorenoMVCWeb.Services.EmailService>();
builder.Services.AddDataProtection();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

var app = builder.Build();

// Ejecuta las migraciones automáticamente al iniciar el proyecto
ApplyMigrations(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Configuración de rutas predeterminadas
app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

// Iniciar la aplicación
app.Run();

// Función local: aplica migraciones de EF Core al iniciar
static void ApplyMigrations(WebApplication application)
{
    using var scope = application.Services.CreateScope();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ClinicaMedEduardoMorenoMVCWeb.Data.AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
        logger?.LogError(ex, "Ocurrió un error al aplicar las migraciones de EF Core.");
    }
}