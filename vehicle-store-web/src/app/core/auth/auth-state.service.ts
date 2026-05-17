import { Injectable, signal, computed } from '@angular/core';
import { AuthSession } from '../../domain/models/auth-session.model';
import { UserRole } from '../../domain/enums/user-role.enum';

const SESSION_KEY = 'vehicle_store_session';

@Injectable({ providedIn: 'root' })
export class AuthStateService {
  private readonly sessionSignal = signal<AuthSession | null>(this.loadSession());

  readonly session = this.sessionSignal.asReadonly();
  readonly isAuthenticated = computed(() => this.sessionSignal() !== null);
  readonly isAdmin = computed(() => this.sessionSignal()?.role === UserRole.Admin);
  readonly username = computed(() => this.sessionSignal()?.username ?? '');

  setSession(session: AuthSession): void {
    sessionStorage.setItem(SESSION_KEY, JSON.stringify(session));
    this.sessionSignal.set(session);
  }

  clearSession(): void {
    sessionStorage.removeItem(SESSION_KEY);
    this.sessionSignal.set(null);
  }

  getToken(): string | null {
    return this.sessionSignal()?.token ?? null;
  }

  private loadSession(): AuthSession | null {
    const raw = sessionStorage.getItem(SESSION_KEY);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as AuthSession;
    } catch {
      sessionStorage.removeItem(SESSION_KEY);
      return null;
    }
  }
}
