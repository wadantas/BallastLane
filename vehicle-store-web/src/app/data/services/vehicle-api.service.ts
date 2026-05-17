import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { API_CONFIG } from '../../core/config/api.config';
import { RegisterVehicleSignature } from '../../domain/interfaces/signatures/register-vehicle.signature';
import { UpdateVehicleSignature } from '../../domain/interfaces/signatures/update-vehicle.signature';
import { RegisterVehicleResponse } from '../../domain/interfaces/responses/register-vehicle.response';
import { VehicleResponse } from '../../domain/interfaces/responses/vehicle.response';
import { Vehicle } from '../../domain/models/vehicle.model';
import { VehicleMapper } from '../mappers/vehicle.mapper';

@Injectable({ providedIn: 'root' })
export class VehicleApiService {
  private readonly baseUrl = `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.vehicles.base}`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<Vehicle[]> {
    return this.http
      .get<VehicleResponse[]>(this.baseUrl)
      .pipe(map((responses) => VehicleMapper.toDomainList(responses)));
  }

  getById(id: string): Observable<Vehicle> {
    return this.http
      .get<VehicleResponse>(`${API_CONFIG.baseUrl}${API_CONFIG.endpoints.vehicles.byId(id)}`)
      .pipe(map((response) => VehicleMapper.toDomain(response)));
  }

  register(signature: RegisterVehicleSignature): Observable<string> {
    return this.http
      .post<RegisterVehicleResponse>(this.baseUrl, signature)
      .pipe(map((response) => response.id));
  }

  update(id: string, signature: UpdateVehicleSignature): Observable<void> {
    return this.http.put<void>(
      `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.vehicles.byId(id)}`,
      signature
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(
      `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.vehicles.byId(id)}`
    );
  }

  markAsSold(id: string): Observable<void> {
    return this.http.patch<void>(
      `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.vehicles.sold(id)}`,
      null
    );
  }
}
