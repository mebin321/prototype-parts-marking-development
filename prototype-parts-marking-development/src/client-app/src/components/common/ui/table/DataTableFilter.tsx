import React, { useCallback } from 'react';
import { Table } from 'semantic-ui-react';

import { formatNumberInterval } from '../../../../models/common/interval';
import { getPropertyValue } from '../../../../utilities/objects';
import DateTimeInput from '../input/DateTimeInput';
import Dropdown from '../input/Dropdown';
import NumberInput from '../input/NumberInput';
import TextValidatingInput from '../input/TextValidatingInput';
import { IFilteredTableColumn } from './dataTableColumn';
import
{
  DataTableDateTimeFilterType,
  DataTableNumberFilterType,
  DataTableSelectFilterType,
  DataTableTextFilterType,
} from './dataTableFilterSpecification';

interface IDataTableFilterProps<TFilter extends object>
{
  isVisible: boolean;
  columns: IFilteredTableColumn<TFilter>[];
  filter?: TFilter;
}

const DataTableFilterComponent = <TFilter extends object>(props: IDataTableFilterProps<TFilter>) =>
{
  const { isVisible, columns, filter } = props;

  const createFilterInput = useCallback((column: IFilteredTableColumn<TFilter>, value: any) =>
  {
    const filterSpecification = column.filter;
    if (!filterSpecification) return null;

    const commonProps =
    {
      fluid: true,
      name: column.name,
      placeholder: 'Any',
    };

    switch (filterSpecification.type)
    {
      case DataTableTextFilterType:
        return (
          <TextValidatingInput
            {...commonProps}
            type='text'
            value={value ?? ''} // don't pass undefined to not confuse React that input is uncontrolled
            pattern={filterSpecification.pattern}
            validator={filterSpecification.validator}
            onChange={(_event, data) => filterSpecification.setter(data.value)}
          />
        );
      case DataTableNumberFilterType:
        return (
          <NumberInput
            {...commonProps}
            min={filterSpecification.min}
            max={filterSpecification.max}
            range={filterSpecification.range}
            editorPlaceholder={filterSpecification.placeholder}
            value={formatNumberInterval(value)}
            onChange={value => filterSpecification.setter(value)}
          />
        );
      case DataTableDateTimeFilterType:
        return (
          <DateTimeInput
            {...commonProps}
            min={filterSpecification.min}
            max={filterSpecification.max}
            value={value}
            onChange={value => filterSpecification.setter(value)}
          />
        );
      case DataTableSelectFilterType:
        return (
          <Dropdown
            {...commonProps}
            multiple={filterSpecification.multiple}
            allowChangeEventOnlyWhenClosed={filterSpecification.multiple}
            allowAdditions={filterSpecification.allowAdditions}
            selectOnBlur={false}
            selectOnNavigation={false}
            value={value}
            validator={filterSpecification.validator}
            options={filterSpecification.options ?? filterSpecification.onSearch}
            onChange={(_event, data) => filterSpecification.setter(data.value)}
          />
        );
      default:
        return null;
    }
  }, []);

  const filterCells = columns.map(column => (
    <Table.Cell
      key={column.name}
      style={{ borderStyle: 'none', padding: '0 0.5em', verticalAlign: 'top' }}
    >
      {createFilterInput(column, getPropertyValue(filter, column.filter?.getter))}
    </Table.Cell>
  ));

  return (
    <Table.Row style={isVisible ? {} : { display: 'none' }}>
      {filterCells}
    </Table.Row>
  );
};

export default DataTableFilterComponent;
