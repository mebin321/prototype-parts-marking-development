import { IResponsePagination } from './responsePagination';

export interface IPaginatedResponse<T>
{
  items: T[];
  pagination: IResponsePagination;
}
