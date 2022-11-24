import { IDataTableColumn } from './dataTableColumn';

export function formatCellValue(value: any): string
{
  if (value === undefined) return '';
  if (value === null) return '';

  switch (typeof value)
  {
    case 'object':
      if (value instanceof Date) return value.toLocaleString();
      return String(value);
    default:
      return String(value);
  }
}

export function getColumnPropertyName(columns: IDataTableColumn<any>[], name: string | undefined)
{
  let propertyName = '';

  if (name)
  {
    // find column with matching name (shown to user) and where value is defined by property name
    const column = columns.find(column => column.name === name && typeof column.value === 'string');
    if (column)
    {
      propertyName = column.value.toString();
    }
  }

  return propertyName;
}
