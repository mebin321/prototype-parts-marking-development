export interface IAuthToken
{
  token: string;
  expiresAt: string;
}

export const EmptyAuthToken: IAuthToken = Object.freeze({
  token: '',
  expiresAt: '',
});
