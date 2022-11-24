import { appendFilterQueryParameters } from '../../../api/utilities';

export interface IUserFilter
{
  isActive?: boolean;
  search?: string;
}

export function appendUserFilterQueryParameters(url: string, filter: IUserFilter)
{
  return appendFilterQueryParameters(url, filter);
}
