using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.UseCases.Tasks.CreateTask;

public sealed record CreateTaskInput(
    string Title,
    string Description,
    TaskItemStatus Status,
    DateTime DueDate,
    Guid UserId);
