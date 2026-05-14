using Microsoft.EntityFrameworkCore;
using WorkspaceJarvis.Data.Entities;

namespace WorkspaceJarvis.Data;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
        // This ensures the database is created and the tables exist
        // every time the app starts up.
        Database.EnsureCreated();
    }

    // These DbSets represent our physical tables in SQLite
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<WorkspacePath> WorkspacePaths { get; set; }

    // This method tells EF Core where the database file lives
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Gets the path to: C:\Users\<User>\AppData\Local\WorkspaceJarvis
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "jarvis.db");
        options.UseSqlite($"Data Source={dbPath}");
    }
}