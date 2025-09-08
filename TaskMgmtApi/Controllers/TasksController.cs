using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using TaskManagementApi.Data;
using TaskManagementApi.Dtos;
using TaskManagementApi.Models;
using TaskStatus = TaskManagementApi.Models.TaskStatus;

namespace TaskManagementApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public TasksController(ApplicationDbContext db)
    {
        _db = db;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskResponse>>> Get(
        [FromQuery] TaskStatus? status,
        [FromQuery] TaskPriority? priority,
        [FromQuery] DateTime? dueFrom,
        [FromQuery] DateTime? dueTo,
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string sortDir = "asc",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        var q = _db.Tasks.AsNoTracking().Where(t => t.UserId == userId);

        if (status.HasValue) q = q.Where(t => t.Status == status);
        if (priority.HasValue) q = q.Where(t => t.Priority == priority);
        if (dueFrom.HasValue) q = q.Where(t => t.DueDate >= dueFrom);
        if (dueTo.HasValue) q = q.Where(t => t.DueDate <= dueTo);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            q = q.Where(t => t.Title.ToLower().Contains(term) ||
                (t.Description != null && t.Description.ToLower().Contains(term)));
        }

        // Sorting
        bool asc = sortDir.Equals("asc", StringComparison.OrdinalIgnoreCase);
        q = sortBy?.ToLower() switch
        {
            "title" => asc ? q.OrderBy(t => t.Title) : q.OrderByDescending(t => t.Title),
            "priority" => asc ? q.OrderBy(t => t.Priority) : q.OrderByDescending(t => t.Priority),
            "status" => asc ? q.OrderBy(t => t.Status) : q.OrderByDescending(t => t.Status),
            "duedate" => asc ? q.OrderBy(t => t.DueDate) : q.OrderByDescending(t => t.DueDate),
            _ => asc ? q.OrderBy(t => t.CreatedAt) : q.OrderByDescending(t => t.CreatedAt)
        };

        var total = await q.CountAsync();
        var items = await q.Skip((page - 1) * pageSize).Take(pageSize)
            .Select(t => new TaskResponse(t.Id, t.Title, t.Description, t.Status, t.Priority, t.DueDate, t.CreatedAt))
            .ToListAsync();

        return Ok(new PagedResult<TaskResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetById(int id)
    {
        var userId = GetUserId();
        var t = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (t is null) return NotFound();

        return new TaskResponse(t.Id, t.Title, t.Description, t.Status, t.Priority, t.DueDate, t.CreatedAt);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(TaskCreateUpdateDto dto)
    {
        var userId = GetUserId();
        var t = new TodoTask
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            UserId = userId
        };
        _db.Tasks.Add(t);
        await _db.SaveChangesAsync();

        var result = new TaskResponse(t.Id, t.Title, t.Description, t.Status, t.Priority, t.DueDate, t.CreatedAt);
        return CreatedAtAction(nameof(GetById), new { id = t.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskResponse>> Update(int id, TaskCreateUpdateDto dto)
    {
        var userId = GetUserId();
        var t = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (t is null) return NotFound();

        t.Title = dto.Title;
        t.Description = dto.Description;
        t.Status = dto.Status;
        t.Priority = dto.Priority;
        t.DueDate = dto.DueDate;

        await _db.SaveChangesAsync();

        return new TaskResponse(t.Id, t.Title, t.Description, t.Status, t.Priority, t.DueDate, t.CreatedAt);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();
        var t = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
        if (t is null) return NotFound();

        _db.Tasks.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
