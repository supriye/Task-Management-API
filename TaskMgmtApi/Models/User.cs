using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(64)]
    public string Username { get; set; } = default!;

    [Required, EmailAddress, MaxLength(256)]
    public string Email { get; set; } = default!;

    [Required]
    public byte[] PasswordHash { get; set; } = default!;

    [Required]
    public byte[] PasswordSalt { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<TodoTask> Tasks { get; set; } = new();
}
