namespace WorkspaceJarvis.UI.Models;

public class Workspace
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public List<string> Paths { get; set; } = new();
}
public class WorkspaceConfig
{
    public List<Workspace> Workspaces { get; set; } = [];
}

public class WorkspaceViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public List<AppDisplayInfo> Paths { get; set; } = new();
}

public record AppDisplayInfo(string FriendlyName, string FullPath);