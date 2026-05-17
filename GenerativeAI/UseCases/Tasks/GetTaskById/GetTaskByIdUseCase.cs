using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Application.UseCases.Tasks.GetTaskById;

public sealed class GetTaskByIdUseCase(ITaskRepository taskRepository)
{
    public async Task<TaskItem> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(id, cancellationToken);

        if (task is null)
            throw new NotFoundException($"Task with id '{id}' was not found.");

        return task;
    }
}
