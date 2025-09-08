using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models;

public class TodoTask
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Todo;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationship
    public int UserId { get; set; }
    public User? User { get; set; }
}
