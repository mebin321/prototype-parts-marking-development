import React from 'react';

import { IDataTableCellFormatterContext } from '../../components/common/ui/table/dataTableFormatter';

export function itemCellFormatter<TData extends {deletedAt: Date}, TFilter extends object>({
  data,
  value,
}: IDataTableCellFormatterContext<TData, TFilter>)
{
  return (
    <span
      style={{
        ...(data.deletedAt !== null ? { color: '#bfbfbf', textDecoration: 'line-through' } : {}),
      }}
    >
      {value}
    </span>
  );
}
