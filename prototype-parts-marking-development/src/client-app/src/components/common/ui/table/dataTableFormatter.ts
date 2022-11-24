import React from 'react';

import { IDataTableColumn } from './dataTableColumn';

export interface IDataTableCellFormatterContext<TData extends object, TFilter extends object>
{
  column: IDataTableColumn<TData, TFilter>;
  data: TData;
  value: string;
}

export type DataTableCellFormatter<TData extends object, TFilter extends object> = (
  context: IDataTableCellFormatterContext<TData, TFilter>
) => React.ReactNode;
