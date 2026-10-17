using Microsoft.AspNetCore.DataProtection;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PawnShopManagement.Data;
using PawnShopManagement.Models;
var desktop = args.Contains("--desktop");
if (desktop) {
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
    Directory.SetCurrentDirectory(AppContext.BaseDirectory);
}
var builder = WebApplication.CreateBuilder(new WebApplicationOptions {
    Args = args,
    ContentRootPath = desktop ? AppContext.BaseDirectory : null
});
if (desktop) builder.WebHost.UseUrls("http://127.0.0.1:0");
builder.Services.AddLocalization();
builder.Services.AddControllersWithViews(o => { o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); o.ModelBinderProviders.Insert(0,new InvariantNumberBinderProvider()); }).AddDataAnnotationsLocalization(o => o.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(PawnShopManagement.Resources.UiResources)));
var connectionString = DatabaseConfiguration.ResolveConnectionString(
    builder.Configuration.GetConnectionString("DefaultConnection"), builder.Environment.ContentRootPath);
var sqlitePath = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder(connectionString).DataSource;
if (sqlitePath != ":memory:") Directory.CreateDirectory(Path.GetDirectoryName(sqlitePath)!);
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connectionString));
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => {
    o.LoginPath = "/Account/Login"; o.AccessDeniedPath = "/Account/Denied";
    o.Cookie.HttpOnly = true; o.Cookie.SameSite = SameSiteMode.Lax;
    o.ExpireTimeSpan = TimeSpan.FromHours(8);
    o.Events.OnRedirectToLogin = c => { if(c.Request.Path.StartsWithSegments("/api")) c.Response.StatusCode=401; else c.Response.Redirect(c.RedirectUri); return Task.CompletedTask; };
    o.Events.OnRedirectToAccessDenied = c => { if(c.Request.Path.StartsWithSegments("/api")) c.Response.StatusCode=403; else c.Response.Redirect(c.RedirectUri); return Task.CompletedTask; };
});
builder.Services.AddAuthorization();
var protection = builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "keys")));
if (OperatingSystem.IsWindows()) protection.ProtectKeysWithDpapi();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o => { o.IdleTimeout = TimeSpan.FromMinutes(30); o.Cookie.HttpOnly = true; o.Cookie.IsEssential = true; });
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<AppUser>, Microsoft.AspNetCore.Identity.PasswordHasher<AppUser>>();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.Use(async (context, next) => {
    context.Response.Headers["X-Content-Type-Options"]="nosniff";
    context.Response.Headers["X-Frame-Options"]="DENY";
    context.Response.Headers["Referrer-Policy"]="same-origin";
    await next();
});
if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
var cultures = new[] { new CultureInfo("vi-VN"), new CultureInfo("en-US") };
app.UseRequestLocalization(new RequestLocalizationOptions { DefaultRequestCulture = new("vi-VN"), SupportedCultures = cultures, SupportedUICultures = cultures });
app.UseSession(); app.UseAuthentication(); app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Contracts}/{action=Index}/{id?}");
app.MapControllers();
if (app.Environment.IsDevelopment())
    app.MapGet("/_launcher/ready", () => Results.Text(app.Configuration["PAWN_LAUNCH_ID"] ?? "ready"));
var initialize = true; // The file database is prepared automatically on every launch.
if (initialize) {
    try {
        using var scope = app.Services.CreateScope();
        await PawnShopManagement.Database.DatabaseInitializer.InitializeAsync(scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(), app.Environment.ContentRootPath);
        Console.WriteLine("Database ready. First use: /Account/Setup");
    } catch (Exception ex) when (ex is Microsoft.Data.Sqlite.SqliteException or InvalidOperationException or IOException or UnauthorizedAccessException) {
        Console.Error.WriteLine("Cannot prepare the SQLite file. Extract the full ZIP into a writable folder such as Documents; do not run from inside the ZIP.");
        Console.Error.WriteLine(ex.Message);
        Environment.ExitCode = 1;
        return;
    }
}
if (args.Contains("--init-db")) return;
if (desktop) {
    app.Lifetime.ApplicationStarted.Register(() => {
        var address = app.Urls.First(x=>x.StartsWith("http://",StringComparison.OrdinalIgnoreCase));
        var url = address.TrimEnd('/') + "/Account/Start";
        Console.WriteLine("\n[PAWNFLOW-PORTABLE] Website ready: " + url);
        Console.WriteLine("Keep this window open. Press Ctrl+C to stop. Data: App_Data/pawnflow.db");
        if (!args.Contains("--no-browser")) {
            try { System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute=true }); }
            catch { Console.WriteLine("Please open the address above in your browser."); }
        }
    });
}
app.Run();
public partial class Program { }
