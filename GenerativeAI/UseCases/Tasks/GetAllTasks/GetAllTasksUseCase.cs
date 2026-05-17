using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.UseCases.Tasks.GetAllTasks;

public sealed class GetAllTasksUseCase(ITaskRepository taskRepository)
{
    public Task<IReadOnlyList<TaskItem>> ExecuteAsync(CancellationToken cancellationToken = default) =>
        taskRepository.GetAllAsync(cancellationToken);
}
