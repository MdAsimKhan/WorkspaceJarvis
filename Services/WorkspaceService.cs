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
}