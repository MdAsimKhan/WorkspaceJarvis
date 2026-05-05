using WorkspaceJarvis.UI.Models;
using System.Text.Json;
using System.Diagnostics;

namespace WorkspaceJarvis.UI.Services;

public class WorkspaceService
{
    private readonly string _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

    public List<Workspace> GetWorkspaces()
    {
        if (!File.Exists(_configPath))
        {
            return new List<Workspace>();
        }
        var json = File.ReadAllText(_configPath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var config = JsonSerializer.Deserialize<WorkspaceConfig>(json, options);
        return config?.Workspaces ?? new();
    }

    public void Launch(Workspace workspace)
    {
        foreach (var path in workspace.Paths)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to launch {path}: {ex.Message}");
            }
        }
    }

    public(bool Success, string Message) SaveWorkspace(Workspace newWorkspace)
    {
        try
        {
            var workspaces = GetWorkspaces();
            workspaces.Add(newWorkspace);

            var config = new WorkspaceConfig { Workspaces = workspaces };
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(_configPath, json);
            return (true, "Workspace added successfully!");
        }
        catch (Exception ex)
        {
            // We pass the actual error message back to the UI
            return (false, ex.Message);
        }
    }

    public (bool Success, string Message) DeleteWorkspace(Guid id)
    {
        try
        {
            var workspaces = GetWorkspaces();
            var itemToRemove = workspaces.FirstOrDefault(w => w.Id == id);

            if (itemToRemove == null)
                return (false, "Workspace not found.");

            workspaces.Remove(itemToRemove);
            SaveAll(workspaces);

            return (true, "Workspace deleted successfully!");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public (bool Success, string Message) UpdateWorkspace(Workspace updatedWs)
    {
        try
        {
            var workspaces = GetWorkspaces();
            var index = workspaces.FindIndex(w => w.Id == updatedWs.Id);

            if (index == -1) return (false, "Workspace not found.");

            workspaces[index] = updatedWs;
            SaveAll(workspaces);

            return (true, "Workspace updated!");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }


    private void SaveAll(List<Workspace> workspaces)
    {
        var json = JsonSerializer.Serialize(new { Workspaces = workspaces },
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_configPath, json);
    }
}