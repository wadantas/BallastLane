import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { API_CONFIG } from '../../core/config/api.config';
import { CreateUserSignature } from '../../domain/interfaces/signatures/create-user.signature';
import { CreateUserResponse } from '../../domain/interfaces/responses/create-user.response';
import { UserResponse } from '../../domain/interfaces/responses/user.response';
import { User } from '../../domain/models/user.model';
import { UserMapper } from '../mappers/user.mapper';

@Injectable({ providedIn: 'root' })
export class UserApiService {
  private readonly url = `${API_CONFIG.baseUrl}${API_CONFIG.endpoints.users.base}`;

  constructor(private readonly http: HttpClient) {}

  getAll(): Observable<User[]> {
    return this.http
      .get<UserResponse[]>(this.url)
      .pipe(map((responses) => UserMapper.toDomainList(responses)));
  }

  create(signature: CreateUserSignature): Observable<string> {
    return this.http
      .post<CreateUserResponse>(this.url, signature)
      .pipe(map((response) => response.id));
  }
}
