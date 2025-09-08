using System.ComponentModel.DataAnnotations;
using TaskManagementApi.Models;
using TaskStatus = TaskManagementApi.Models.TaskStatus;

namespace TaskManagementApi.Dtos;

public record TaskCreateUpdateDto(
    [property: Required, MaxLength(200)] string Title,
    string? Description,
    TaskStatus Status,         // ? fixed: now uses your model enum
    TaskPriority Priority,     // ? still correct
    DateTime? DueDate
);

public record TaskResponse(
    int Id,
    string Title,
    string? Description,
    TaskStatus Status,         // ? fixed: now uses your model enum
    TaskPriority Priority,     // ? still correct
    DateTime? DueDate,
    DateTime CreatedAt
);
