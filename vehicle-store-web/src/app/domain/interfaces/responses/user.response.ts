import { UserRole } from '../../enums/user-role.enum';

export interface UserResponse {
  id: string;
  username: string;
  email: string;
  role: UserRole;
  createdAt: string;
}
