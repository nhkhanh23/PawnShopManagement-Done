using Microsoft.Data.Sqlite;
namespace PawnShopManagement.Data;
public static class DatabaseConfiguration {
    public static string ResolveConnectionString(string? configured, string contentRoot) {
        var settings = new SqliteConnectionStringBuilder(string.IsNullOrWhiteSpace(configured)
            ? "Data Source=App_Data/pawnflow.db;Foreign Keys=True;Default Timeout=30" : configured);
        if (string.IsNullOrWhiteSpace(settings.DataSource))
            throw new InvalidOperationException("SQLite Data Source must specify a file path.");
        if (settings.DataSource != ":memory:")
            settings.DataSource = Path.GetFullPath(settings.DataSource, contentRoot);
        settings.ForeignKeys = true;
        return settings.ConnectionString;
    }
}
