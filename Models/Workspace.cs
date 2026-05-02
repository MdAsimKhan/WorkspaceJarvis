namespace WorkspaceJarvis.UI.Models;

public class Workspace
{
    public string Name { get; set; } = string.Empty;
    public List<string> Paths { get; set; } = [];
}

public class WorkspaceConfig
{
    public List<Workspace> Workspaces { get; set; } = [];
}
