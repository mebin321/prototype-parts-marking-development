import { IUserData } from '../users/userData';
import { IAuthToken } from './authToken';

export interface IAuthData
{
  user: IUserData;
  accessToken: IAuthToken;
  refreshToken: IAuthToken;
}
