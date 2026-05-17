import { Vehicle } from '../../domain/models/vehicle.model';
import { VehicleResponse } from '../../domain/interfaces/responses/vehicle.response';

export class VehicleMapper {
  static toDomain(response: VehicleResponse): Vehicle {
    return {
      id: response.id,
      plateNumber: response.plateNumber,
      document: response.document,
      brand: response.brand,
      model: response.model,
      year: response.year,
      price: response.price,
      isSold: response.isSold,
      createdAt: response.createdAt,
      updatedAt: response.updatedAt
    };
  }

  static toDomainList(responses: VehicleResponse[]): Vehicle[] {
    return responses.map(VehicleMapper.toDomain);
  }
}
