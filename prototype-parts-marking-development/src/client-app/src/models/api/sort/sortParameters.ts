import { SortDirection } from './sortDirection';

export interface ISortParameters
{
  sortBy: string;
  sortDirection: SortDirection;
}

export const NoSort = Object.freeze({
  sortBy: '',
  sortDirection: SortDirection.None,
});
