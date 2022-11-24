import { Options } from 'csv-stringify';
import stringify from 'csv-stringify/lib/sync';

import { formatDateTimeToLocalCultureInvariant } from './datetime';

const CommonOptions: Options =
{
  cast:
  {
    date: value => formatDateTimeToLocalCultureInvariant(value),
  },
};

const ExcelCsvDialect: Options =
{
  bom: true,
  delimiter: ';',
  quote: '"',
  escape: '"',
  record_delimiter: '\r\n',
  eof: true,
  header: true,
};

export interface IColumnOption<T extends object>
{
  key: Extract<keyof T, string>;
  header: string;
}

export function convertToCSV<T extends object>(data: T[], columns?: IColumnOption<T>[])
{
  return stringify(data, { ...CommonOptions, ...ExcelCsvDialect, columns: columns });
}
