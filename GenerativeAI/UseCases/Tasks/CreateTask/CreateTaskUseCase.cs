using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Exceptions;

namespace TaskManagement.Application.UseCases.Tasks.CreateTask;

public sealed class CreateTaskUseCase(
    ITaskRepository taskRepository,
    IUserRepository userRepository)
{
    public async Task<CreateTaskOutput> ExecuteAsync(CreateTaskInput input, CancellationToken cancellationToken = default)
    {
        if (!await userRepository.ExistsAsync(input.UserId, cancellationToken))
            throw new NotFoundException($"User with id '{input.UserId}' was not found.");

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = input.Title.Trim(),
            Description = input.Description.Trim(),
            Status = input.Status,
            DueDate = input.DueDate,
            UserId = input.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await taskRepository.CreateAsync(task, cancellationToken);

        return new CreateTaskOutput(task.Id);
    }
}
