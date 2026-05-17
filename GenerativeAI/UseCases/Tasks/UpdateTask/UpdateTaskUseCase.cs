using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Application.UseCases.Tasks.UpdateTask;

public sealed class UpdateTaskUseCase(
    ITaskRepository taskRepository,
    IUserRepository userRepository)
{
    public async Task ExecuteAsync(UpdateTaskInput input, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(input.Id, cancellationToken);

        if (task is null)
            throw new NotFoundException($"Task with id '{input.Id}' was not found.");

        if (!await userRepository.ExistsAsync(input.UserId, cancellationToken))
            throw new NotFoundException($"User with id '{input.UserId}' was not found.");

        task.Title = input.Title.Trim();
        task.Description = input.Description.Trim();
        task.Status = input.Status;
        task.DueDate = input.DueDate;
        task.UserId = input.UserId;
        task.UpdatedAt = DateTime.UtcNow;

        await taskRepository.UpdateAsync(task, cancellationToken);
    }
}
