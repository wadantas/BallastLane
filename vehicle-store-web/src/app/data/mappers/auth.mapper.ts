import { AuthSession } from '../../domain/models/auth-session.model';
import { LoginResponse } from '../../domain/interfaces/responses/login.response';

export class AuthMapper {
  static toSession(response: LoginResponse): AuthSession {
    return {
      token: response.token,
      userId: response.userId,
      username: response.username,
      role: response.role
    };
  }
}
