using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleStore.Api.Contracts.Signatures;
using VehicleStore.Api.Mappers;
using VehicleStore.Application.UseCases.Auth.Login;

namespace VehicleStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;

    public AuthController(LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginSignature signature,
        CancellationToken cancellationToken)
    {
        var output = await _loginUseCase.ExecuteAsync(
            UserApiMapper.ToInput(signature),
            cancellationToken);

        return Ok(UserApiMapper.ToResponse(output));
    }
}
