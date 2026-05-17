import { environment } from '../../../environments/environment';

export const API_CONFIG = {
  baseUrl: environment.apiBaseUrl,
  endpoints: {
    auth: {
      login: '/api/auth/login'
    },
    vehicles: {
      base: '/api/vehicles',
      byId: (id: string) => `/api/vehicles/${id}`,
      sold: (id: string) => `/api/vehicles/${id}/sold`
    },
    users: {
      base: '/api/users'
    }
  }
} as const;
