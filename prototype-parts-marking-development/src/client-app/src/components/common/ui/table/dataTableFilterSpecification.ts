import { ConfiguredValidator } from 'revalidate';
import { DropdownItemProps } from 'semantic-ui-react';

import { DateInterval, NumberInterval } from '../../../../models/common/interval';
import { DataGetter } from '../../../../utilities/objects';

export const DataTableTextFilterType = 'text';
export const DataTableNumberFilterType = 'number';
export const DataTableDateTimeFilterType = 'date-time';
export const DataTableSelectFilterType = 'select';

export type DropdownValue = undefined | boolean | number | string | (boolean | number | string)[];

export interface IDataTableTextFilterSpecification<T extends object>
{
  type: typeof DataTableTextFilterType;
  pattern?: RegExp;
  validator?: (value: string) => boolean;
  getter: DataGetter<T, string>;
  setter: (value: string) => void;
}

export interface IDataTableNumberFilterSpecification<T extends object>
{
  type: typeof DataTableNumberFilterType;
  min?: number;
  max?: number;
  range?: boolean;
  placeholder?: string;
  getter: DataGetter<T, NumberInterval>;
  setter: (value: NumberInterval) => void;
}

export interface IDataTableDateTimeFilterSpecification<T extends object>
{
  type: typeof DataTableDateTimeFilterType;
  min?: Date;
  max?: Date;
  getter: DataGetter<T, DateInterval>;
  setter: (value: DateInterval) => void;
}

interface IDataTableSelectFilterSpecification<T extends object>
{
  type: typeof DataTableSelectFilterType;
  multiple?: boolean;
  allowAdditions?: boolean;
  getter: DataGetter<T, DropdownValue>;
  setter: (value: DropdownValue) => void;
}

export interface IDataTableSelectStaticFilterSpecification<T extends object>
  extends IDataTableSelectFilterSpecification<T>
{
  options: DropdownItemProps[];
  validator?: undefined; // validator has no use on statically provided options
  onSearch?: undefined; // options are provided statically in options property
}

export interface IDataTableSelectDynamicFilterSpecification<T extends object>
  extends IDataTableSelectFilterSpecification<T>
{
  options?: undefined; // options are provided in onSearch function return value
  validator?: ConfiguredValidator;
  onSearch: (text: string) => DropdownItemProps[] | Promise<DropdownItemProps[]>;
}

export type DataTableFilterSpecification<T extends object> = IDataTableTextFilterSpecification<T>
  | IDataTableNumberFilterSpecification<T> | IDataTableDateTimeFilterSpecification<T>
  | IDataTableSelectStaticFilterSpecification<T> | IDataTableSelectDynamicFilterSpecification<T>;
