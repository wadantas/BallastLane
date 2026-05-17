using Microsoft.Extensions.DependencyInjection;
using VehicleStore.Application.UseCases.Auth.Login;
using VehicleStore.Application.UseCases.Users.CreateUser;
using VehicleStore.Application.UseCases.Users.GetAllUsers;
using VehicleStore.Application.UseCases.Vehicles.DeleteVehicle;
using VehicleStore.Application.UseCases.Vehicles.GetAllVehicles;
using VehicleStore.Application.UseCases.Vehicles.GetVehicleById;
using VehicleStore.Application.UseCases.Vehicles.MarkVehicleAsSold;
using VehicleStore.Application.UseCases.Vehicles.RegisterVehicle;
using VehicleStore.Application.UseCases.Vehicles.UpdateVehicle;

namespace VehicleStore.Application.DependencyInjection;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<RegisterVehicleUseCase>();
        services.AddScoped<UpdateVehicleUseCase>();
        services.AddScoped<DeleteVehicleUseCase>();
        services.AddScoped<MarkVehicleAsSoldUseCase>();
        services.AddScoped<GetVehicleByIdUseCase>();
        services.AddScoped<GetAllVehiclesUseCase>();
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetAllUsersUseCase>();
        services.AddScoped<LoginUseCase>();

        return services;
    }
}
