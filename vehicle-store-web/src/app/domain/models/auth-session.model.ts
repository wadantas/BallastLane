import { UserRole } from '../enums/user-role.enum';

export interface AuthSession {
  token: string;
  userId: string;
  username: string;
  role: UserRole;
}
