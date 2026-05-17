import { Component, OnInit, inject, input, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { VehicleApiService } from '../../../data/services/vehicle-api.service';
import { Vehicle } from '../../../domain/models/vehicle.model';
import { RegisterVehicleSignature } from '../../../domain/interfaces/signatures/register-vehicle.signature';
import { UpdateVehicleSignature } from '../../../domain/interfaces/signatures/update-vehicle.signature';
import { ApiErrorResponse } from '../../../domain/interfaces/responses/api-error.response';

@Component({
  selector: 'app-vehicle-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './vehicle-form.component.html',
  styleUrl: './vehicle-form.component.scss'
})
export class VehicleFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly vehicleApi = inject(VehicleApiService);

  readonly vehicle = input<Vehicle | null>(null);
  readonly saved = output<void>();
  readonly cancelled = output<void>();

  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    plateNumber: ['', [Validators.required, Validators.maxLength(20)]],
    document: ['', [Validators.required, Validators.maxLength(50)]],
    brand: ['', [Validators.required, Validators.maxLength(100)]],
    model: ['', [Validators.required, Validators.maxLength(100)]],
    year: [new Date().getFullYear(), [Validators.required, Validators.min(1900)]],
    price: [0, [Validators.required, Validators.min(0.01)]]
  });

  get isEditMode(): boolean {
    return this.vehicle() !== null;
  }

  ngOnInit(): void {
    const v = this.vehicle();
    if (v) {
      this.form.patchValue({
        plateNumber: v.plateNumber,
        document: v.document,
        brand: v.brand,
        model: v.model,
        year: v.year,
        price: v.price
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    this.errorMessage.set(null);

    const value = this.form.getRawValue();
    const editVehicle = this.vehicle();

    const handleSuccess = (): void => {
      this.loading.set(false);
      this.saved.emit();
    };

    const handleError = (err: HttpErrorResponse): void => {
      this.loading.set(false);
      const body = err.error as ApiErrorResponse | undefined;
      this.errorMessage.set(body?.error ?? 'Failed to save vehicle.');
    };

    if (editVehicle) {
      this.vehicleApi.update(editVehicle.id, value as UpdateVehicleSignature).subscribe({
        next: handleSuccess,
        error: handleError
      });
    } else {
      this.vehicleApi.register(value as RegisterVehicleSignature).subscribe({
        next: handleSuccess,
        error: handleError
      });
    }
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
