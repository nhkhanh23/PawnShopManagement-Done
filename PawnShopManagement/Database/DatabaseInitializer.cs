using Microsoft.EntityFrameworkCore;
using PawnShopManagement.Data;
namespace PawnShopManagement.Database;
public static class DatabaseInitializer {
    public static async Task InitializeAsync(ApplicationDbContext db, string contentRoot) {
        if (!db.Database.IsSqlite()) throw new InvalidOperationException("This portable edition requires SQLite.");
        await db.Database.EnsureCreatedAsync();
        await db.Database.ExecuteSqlRawAsync("PRAGMA journal_mode=WAL;");
        await db.AppUsers.AsNoTracking().OrderBy(x=>x.Id).Select(x=>x.Id).Take(1).ToListAsync();
        await db.Customers.AsNoTracking().OrderBy(x=>x.CustomerID).Select(x=>x.CustomerID).Take(1).ToListAsync();
        await db.PawnContracts.AsNoTracking().OrderBy(x=>x.ContractID).Select(x=>x.ContractID).Take(1).ToListAsync();
        Console.WriteLine("[SQLITE-PORTABLE] File database ready. No SQL Server required.");
    }
}
