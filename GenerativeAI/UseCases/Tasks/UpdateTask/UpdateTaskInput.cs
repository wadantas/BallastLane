using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.UseCases.Tasks.UpdateTask;

public sealed record UpdateTaskInput(
    Guid Id,
    string Title,
    string Description,
    TaskItemStatus Status,
    DateTime DueDate,
    Guid UserId);
