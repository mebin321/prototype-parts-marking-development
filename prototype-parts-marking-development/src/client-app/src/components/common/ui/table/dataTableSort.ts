import { SortDirection } from '../../../../models/api/sort/sortDirection';
import { ISortParameters } from '../../../../models/api/sort/sortParameters';
import { IDataTableColumn } from './dataTableColumn';

export interface IDataTableSort
{
  column?: string;
  direction: SortDirection;
}

export function toggleSortDirection(direction: SortDirection)
{
  switch (direction)
  {
    case SortDirection.Ascending:
      return SortDirection.Descending;
    case SortDirection.Descending:
      return SortDirection.None;
    default:
      return SortDirection.Ascending;
  }
}

export function dataTableSortFromSortParameters(
  sort: ISortParameters | undefined,
  columns: IDataTableColumn<any>[]
): IDataTableSort
{
  const columnName = sort?.sortBy
    ? columns.find(column => column.value === sort?.sortBy)?.name
    : undefined;
  return { column: columnName, direction: sort?.sortDirection ?? SortDirection.None };
}
