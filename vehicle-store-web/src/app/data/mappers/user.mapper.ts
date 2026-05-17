import { UserResponse } from '../../domain/interfaces/responses/user.response';
import { User } from '../../domain/models/user.model';

export class UserMapper {
  static toDomain(response: UserResponse): User {
    return {
      id: response.id,
      username: response.username,
      email: response.email,
      role: response.role,
      createdAt: response.createdAt
    };
  }

  static toDomainList(responses: UserResponse[]): User[] {
    return responses.map(UserMapper.toDomain);
  }
}
