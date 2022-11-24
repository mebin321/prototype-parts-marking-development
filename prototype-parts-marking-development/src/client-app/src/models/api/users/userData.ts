export interface IUserData
{
  id: number;
  name: string;
  email: string;
  username: string;
  createdAt?: string;
  modifiedAt?: string;
  deletedAt?: string;
}

export const EmptyUserData: IUserData = Object.freeze({
  id: -1,
  name: '',
  email: '',
  username: '',
});
