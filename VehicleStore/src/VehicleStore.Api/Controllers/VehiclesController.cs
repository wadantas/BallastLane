using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleStore.Api.Contracts.Signatures;
using VehicleStore.Api.Mappers;
using VehicleStore.Application.UseCases.Vehicles.DeleteVehicle;
using VehicleStore.Application.UseCases.Vehicles.GetAllVehicles;
using VehicleStore.Application.UseCases.Vehicles.GetVehicleById;
using VehicleStore.Application.UseCases.Vehicles.MarkVehicleAsSold;
using VehicleStore.Application.UseCases.Vehicles.RegisterVehicle;
using VehicleStore.Application.UseCases.Vehicles.UpdateVehicle;

namespace VehicleStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly RegisterVehicleUseCase _registerVehicleUseCase;
    private readonly UpdateVehicleUseCase _updateVehicleUseCase;
    private readonly DeleteVehicleUseCase _deleteVehicleUseCase;
    private readonly MarkVehicleAsSoldUseCase _markVehicleAsSoldUseCase;
    private readonly GetVehicleByIdUseCase _getVehicleByIdUseCase;
    private readonly GetAllVehiclesUseCase _getAllVehiclesUseCase;

    public VehiclesController(
        RegisterVehicleUseCase registerVehicleUseCase,
        UpdateVehicleUseCase updateVehicleUseCase,
        DeleteVehicleUseCase deleteVehicleUseCase,
        MarkVehicleAsSoldUseCase markVehicleAsSoldUseCase,
        GetVehicleByIdUseCase getVehicleByIdUseCase,
        GetAllVehiclesUseCase getAllVehiclesUseCase)
    {
        _registerVehicleUseCase = registerVehicleUseCase;
        _updateVehicleUseCase = updateVehicleUseCase;
        _deleteVehicleUseCase = deleteVehicleUseCase;
        _markVehicleAsSoldUseCase = markVehicleAsSoldUseCase;
        _getVehicleByIdUseCase = getVehicleByIdUseCase;
        _getAllVehiclesUseCase = getAllVehiclesUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var vehicles = await _getAllVehiclesUseCase.ExecuteAsync(cancellationToken);
        return Ok(vehicles.Select(VehicleApiMapper.ToResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var vehicle = await _getVehicleByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(VehicleApiMapper.ToResponse(vehicle));
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        [FromBody] RegisterVehicleSignature signature,
        CancellationToken cancellationToken)
    {
        var output = await _registerVehicleUseCase.ExecuteAsync(
            VehicleApiMapper.ToInput(signature),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = output.Id },
            VehicleApiMapper.ToResponse(output));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateVehicleSignature signature,
        CancellationToken cancellationToken)
    {
        await _updateVehicleUseCase.ExecuteAsync(
            VehicleApiMapper.ToInput(id, signature),
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _deleteVehicleUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/sold")]
    public async Task<IActionResult> MarkAsSold(Guid id, CancellationToken cancellationToken)
    {
        await _markVehicleAsSoldUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }
}
