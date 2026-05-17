import { UserRole } from '../../enums/user-role.enum';

export interface CreateUserSignature {
  username: string;
  email: string;
  password: string;
  role: UserRole;
}
