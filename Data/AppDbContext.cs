using Microsoft.EntityFrameworkCore;
using WorkspaceJarvis.Data.Entities;

namespace WorkspaceJarvis.Data;

public class AppDbContext : DbContext
{
    // These DbSets represent our physical tables in SQLite
    public DbSet<Workspace> Workspaces { get; set; }
    public DbSet<WorkspacePath> WorkspacePaths { get; set; }

    // This method tells EF Core where the database file lives
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // We'll use a local file named 'jarvis.db'
        options.UseSqlite("Data Source=jarvis.db");
    }
}