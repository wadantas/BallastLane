using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Application.UseCases.Tasks.DeleteTask;

public sealed class DeleteTaskUseCase(ITaskRepository taskRepository)
{
    public async Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!await taskRepository.ExistsAsync(id, cancellationToken))
            throw new NotFoundException($"Task with id '{id}' was not found.");

        await taskRepository.DeleteAsync(id, cancellationToken);
    }
}
