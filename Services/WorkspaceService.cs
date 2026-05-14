using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WorkspaceJarvis.Data;
using WorkspaceJarvis.Models;

namespace WorkspaceJarvis.Services;

public class WorkspaceService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<WorkspaceJarvis.Models.Workspace>> GetWorkspacesAsync()
    {
        // 1. Pull the data from the SQLite tables
        var entities = await _context.Workspaces
            .Include(w => w.Paths)
            .ToListAsync();

        // 2. Convert each 'Entity' into a 'Model' the UI can use
        return entities.Select(e => new WorkspaceJarvis.Models.Workspace
        {
            Id = e.Id,
            Name = e.Name,
            // We take the PathValue string from each path entity
            Paths = e.Paths.Select(p => p.PathValue).ToList()
        }).ToList();
    }

    public async Task LaunchWorkspaceAsync(Guid id, int delayMilliseconds = 1500)
    {
        var workspace = await _context.Workspaces
            .Include(w => w.Paths)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (workspace == null) return;

        foreach (var pathEntity in workspace.Paths)
        {
            var path = pathEntity.PathValue;
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                };

                Process.Start(psi);

                // Wait before launching the next one
                await Task.Delay(delayMilliseconds);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to launch {path}: {ex.Message}");
            }
        }
    }

    public async Task<(bool Success, string Message)> SaveWorkspaceAsync(Workspace model)
    {
        try
        {
            var entity = new WorkspaceJarvis.Data.Entities.Workspace
            {
                Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id,
                Name = model.Name,
                Paths = model.Paths.Select(p => new WorkspaceJarvis.Data.Entities.WorkspacePath
                {
                    PathValue = p
                }).ToList()
            };

            _context.Workspaces.Add(entity);
            await _context.SaveChangesAsync();
            return (true, "Workspace added successfully!");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> DeleteWorkspaceAsync(Guid id)
    {
        try
        {
            var workspace = await _context.Workspaces.FindAsync(id);
            if (workspace == null) return (false, "Workspace not found.");

            _context.Workspaces.Remove(workspace);
            await _context.SaveChangesAsync();
            return (true, "Workspace deleted successfully!");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string Message)> UpdateWorkspaceAsync(Workspace model)
    {
        try
        {
            var entity = await _context.Workspaces
                .Include(w => w.Paths)
                .FirstOrDefaultAsync(w => w.Id == model.Id);

            if (entity == null) return (false, "Workspace not found.");

            entity.Name = model.Name;

            // Simple way to update paths: remove old ones and add new ones
            _context.WorkspacePaths.RemoveRange(entity.Paths);
            entity.Paths = model.Paths.Select(p => new WorkspaceJarvis.Data.Entities.WorkspacePath
            {
                PathValue = p,
                WorkspaceId = entity.Id
            }).ToList();

            await _context.SaveChangesAsync();
            return (true, "Workspace updated!");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}