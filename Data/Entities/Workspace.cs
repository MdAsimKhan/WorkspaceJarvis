using System.ComponentModel.DataAnnotations;

namespace WorkspaceJarvis.Data.Entities;

public class Workspace
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; } = string.Empty;

    // Navigation Property: The list of paths belonging to this workspace
    public ICollection<WorkspacePath> Paths { get; set; } = new List<WorkspacePath>();
}