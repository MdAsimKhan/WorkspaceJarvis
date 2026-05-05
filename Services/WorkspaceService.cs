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

    public (bool Success, string Message) DeleteWorkspace(string workspaceName)
    {
        try
        {
            var workspaces = GetWorkspaces();

            // Find the item to remove
            var itemToRemove = workspaces.FirstOrDefault(w => w.Name == workspaceName);

            if (itemToRemove == null)
                return (false, "Workspace not found.");

            workspaces.Remove(itemToRemove);

            // Save the updated list
            var config = new WorkspaceConfig { Workspaces = workspaces };
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);

            return (true, "Workspace deleted successfully!");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public (bool Success, string Message) UpdateWorkspace(string oldName, Workspace updatedWs)
    {
        try
        {
            var workspaces = GetWorkspaces();
            var index = workspaces.FindIndex(w => w.Name == oldName);

            if (index == -1) return (false, "Workspace not found.");

            workspaces[index] = updatedWs;

            var config = new WorkspaceConfig { Workspaces = workspaces };
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);

            return (true, "Workspace updated!");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}