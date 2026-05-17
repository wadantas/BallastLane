import { UserRole } from '../../enums/user-role.enum';

export interface LoginResponse {
  token: string;
  userId: string;
  username: string;
  role: UserRole;
}
