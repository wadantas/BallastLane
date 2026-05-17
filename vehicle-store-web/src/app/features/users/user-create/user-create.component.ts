import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { UserApiService } from '../../../data/services/user-api.service';
import { UserRole } from '../../../domain/enums/user-role.enum';
import { CreateUserSignature } from '../../../domain/interfaces/signatures/create-user.signature';
import { ApiErrorResponse } from '../../../domain/interfaces/responses/api-error.response';
import { User } from '../../../domain/models/user.model';

@Component({
  selector: 'app-user-create',
  standalone: true,
  imports: [ReactiveFormsModule, DatePipe],
  templateUrl: './user-create.component.html',
  styleUrl: './user-create.component.scss'
})
export class UserCreateComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly userApi = inject(UserApiService);

  readonly roles = [UserRole.User, UserRole.Admin];
  readonly UserRole = UserRole;

  readonly users = signal<User[]>([]);
  readonly listLoading = signal(true);
  readonly listError = signal<string | null>(null);
  readonly creating = signal(false);
  readonly successMessage = signal<string | null>(null);
  readonly errorMessage = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    role: [UserRole.User, Validators.required]
  });

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.listLoading.set(true);
    this.listError.set(null);

    this.userApi.getAll().subscribe({
      next: (list) => {
        this.users.set(list);
        this.listLoading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.listLoading.set(false);
        const body = err.error as ApiErrorResponse | undefined;
        this.listError.set(body?.error ?? 'Failed to load users.');
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.creating.set(true);
    this.successMessage.set(null);
    this.errorMessage.set(null);

    const signature: CreateUserSignature = this.form.getRawValue();

    this.userApi.create(signature).subscribe({
      next: (id) => {
        this.creating.set(false);
        this.successMessage.set(`User created successfully (id: ${id}).`);
        this.form.reset({ role: UserRole.User });
        this.loadUsers();
      },
      error: (err: HttpErrorResponse) => {
        this.creating.set(false);
        const body = err.error as ApiErrorResponse | undefined;
        this.errorMessage.set(body?.error ?? 'Failed to create user.');
      }
    });
  }
}
