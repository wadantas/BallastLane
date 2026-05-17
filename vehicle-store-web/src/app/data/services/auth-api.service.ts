import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { API_CONFIG } from '../../core/config/api.config';
import { LoginSignature } from '../../domain/interfaces/signatures/login.signature';
import { LoginResponse } from '../../domain/interfaces/responses/login.response';
import { AuthSession } from '../../domain/models/auth-session.model';
import { AuthMapper } from '../mappers/auth.mapper';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly url = `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.auth.login}`;

  constructor(private readonly http: HttpClient) {}

  login(signature: LoginSignature): Observable<AuthSession> {
    return this.http
      .post<LoginResponse>(this.url, signature)
      .pipe(map((response) => AuthMapper.toSession(response)));
  }
}
