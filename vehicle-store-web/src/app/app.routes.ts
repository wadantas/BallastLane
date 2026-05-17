import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'vehicles', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then((m) => m.LoginComponent)
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./shared/layout/main-layout/main-layout.component').then((m) => m.MainLayoutComponent),
    children: [
      {
        path: 'vehicles',
        loadComponent: () =>
          import('./features/vehicles/vehicle-list/vehicle-list.component').then(
            (m) => m.VehicleListComponent
          )
      },
      {
        path: 'users/create',
        canActivate: [adminGuard],
        loadComponent: () =>
          import('./features/users/user-create/user-create.component').then((m) => m.UserCreateComponent)
      }
    ]
  },
  { path: '**', redirectTo: 'vehicles' }
];
