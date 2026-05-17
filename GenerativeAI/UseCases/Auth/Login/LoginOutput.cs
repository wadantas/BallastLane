namespace TaskManagement.Application.UseCases.Auth.Login;

public sealed record LoginOutput(string Token, Guid UserId, string Name, string Username);
