import { Component, OnInit, inject, signal } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { VehicleApiService } from '../../../data/services/vehicle-api.service';
import { Vehicle } from '../../../domain/models/vehicle.model';
import { ApiErrorResponse } from '../../../domain/interfaces/responses/api-error.response';
import { VehicleFormComponent } from '../vehicle-form/vehicle-form.component';

@Component({
  selector: 'app-vehicle-list',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, VehicleFormComponent],
  templateUrl: './vehicle-list.component.html',
  styleUrl: './vehicle-list.component.scss'
})
export class VehicleListComponent implements OnInit {
  private readonly vehicleApi = inject(VehicleApiService);

  readonly vehicles = signal<Vehicle[]>([]);
  readonly loading = signal(true);
  readonly errorMessage = signal<string | null>(null);
  readonly showForm = signal(false);
  readonly editingVehicle = signal<Vehicle | null>(null);

  ngOnInit(): void {
    this.loadVehicles();
  }

  loadVehicles(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.vehicleApi.getAll().subscribe({
      next: (list) => {
        this.vehicles.set(list);
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.loading.set(false);
        const body = err.error as ApiErrorResponse | undefined;
        this.errorMessage.set(body?.error ?? 'Failed to load vehicles.');
      }
    });
  }

  openCreate(): void {
    this.editingVehicle.set(null);
    this.showForm.set(true);
  }

  openEdit(vehicle: Vehicle): void {
    this.editingVehicle.set(vehicle);
    this.showForm.set(true);
  }

  closeForm(): void {
    this.showForm.set(false);
    this.editingVehicle.set(null);
  }

  onFormSaved(): void {
    this.closeForm();
    this.loadVehicles();
  }

  markAsSold(vehicle: Vehicle): void {
    if (vehicle.isSold || !confirm(`Mark ${vehicle.plateNumber} as sold?`)) {
      return;
    }

    this.vehicleApi.markAsSold(vehicle.id).subscribe({
      next: () => this.loadVehicles(),
      error: (err: HttpErrorResponse) => {
        const body = err.error as ApiErrorResponse | undefined;
        alert(body?.error ?? 'Failed to mark vehicle as sold.');
      }
    });
  }

  deleteVehicle(vehicle: Vehicle): void {
    if (!confirm(`Delete vehicle ${vehicle.plateNumber}?`)) {
      return;
    }

    this.vehicleApi.delete(vehicle.id).subscribe({
      next: () => this.loadVehicles(),
      error: (err: HttpErrorResponse) => {
        const body = err.error as ApiErrorResponse | undefined;
        alert(body?.error ?? 'Failed to delete vehicle.');
      }
    });
  }
}
