using System.ComponentModel.DataAnnotations;

namespace WorkspaceJarvis.Data.Entities;

public class WorkspacePath
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string PathValue { get; set; } = string.Empty;

    // Foreign Key: Links to the Workspace table
    public Guid WorkspaceId { get; set; }

    // Navigation Property: Back-reference to the parent
    public Workspace? Workspace { get; set; }
}