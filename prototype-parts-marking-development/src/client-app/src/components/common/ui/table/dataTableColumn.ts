import { DataGetter } from '../../../../utilities/objects';
import { DataTableFilterSpecification } from './dataTableFilterSpecification';

export interface IFilteredTableColumn<TFilter extends object>
{
  readonly name: string;
  readonly filter?: DataTableFilterSpecification<TFilter>;
  readonly sortable?: boolean;
}

export interface IDataTableColumn<TData extends object, TFilter extends object = never>
  extends IFilteredTableColumn<TFilter>
{
  readonly tooltip?: DataGetter<TData>;
  readonly value: DataGetter<TData>;
}
