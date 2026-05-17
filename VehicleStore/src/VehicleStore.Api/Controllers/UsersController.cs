using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleStore.Api.Contracts.Signatures;
using VehicleStore.Api.Mappers;
using VehicleStore.Application.UseCases.Users.CreateUser;
using VehicleStore.Application.UseCases.Users.GetAllUsers;

namespace VehicleStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly GetAllUsersUseCase _getAllUsersUseCase;

    public UsersController(
        CreateUserUseCase createUserUseCase,
        GetAllUsersUseCase getAllUsersUseCase)
    {
        _createUserUseCase = createUserUseCase;
        _getAllUsersUseCase = getAllUsersUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _getAllUsersUseCase.ExecuteAsync(cancellationToken);
        return Ok(users.Select(UserApiMapper.ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserSignature signature,
        CancellationToken cancellationToken)
    {
        var output = await _createUserUseCase.ExecuteAsync(
            UserApiMapper.ToInput(signature),
            cancellationToken);

        return Created(string.Empty, UserApiMapper.ToResponse(output));
    }
}
