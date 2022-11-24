import { IUserData } from './userData';

export interface IViewableUser extends IUserData
{
  title: string;
  description: string;
}
