import { IResponsePagination } from '../responsePagination';
import { IUserRole } from './userRole';

export interface IListRoles
{
  items: IUserRole[];
  pagination: IResponsePagination;
}
