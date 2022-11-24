import _ from 'lodash';
import React, { SyntheticEvent, useCallback, useEffect, useMemo, useState } from 'react';
import { Checkbox, CheckboxProps, Dimmer, Icon, Loader, Table } from 'semantic-ui-react';

import { SortDirection } from '../../../../models/api/sort/sortDirection';
import { getPropertyValue } from '../../../../utilities/objects';
import { IDataTableColumn } from './dataTableColumn';
import DataTableFilter from './DataTableFilter';
import { isFilterValueEmpty } from './dataTableFilterUtilities';
import { DataTableCellFormatter } from './dataTableFormatter';
import { IDataTableSort, toggleSortDirection } from './dataTableSort';
import { formatCellValue } from './dataTableUtilities';
import TableExpandToggle from './TableExpandToggle';

interface IDataRow
{
  id: string | number;
}

interface IDataTableProps<TData extends IDataRow, TFilter extends object>
{
  columns: IDataTableColumn<TData, TFilter>[];
  visibleColumns?: string[];
  data?: TData[];
  filter?: TFilter;
  sort?: IDataTableSort;
  tooltip?: string;
  loading?: boolean;
  error?: string;
  filterVisible?: boolean;
  selectedRows?: TData[];
  formatter?: DataTableCellFormatter<TData, TFilter>;
  onFilterVisibilityChange?: (visible: boolean) => void;
  onSortChange?: (sort: IDataTableSort) => void;
  onRowsSelectionChange?: (selection: TData[]) => void;
  onRowClick?: (data: TData) => void;
}

const DataTable = <TData extends IDataRow, TFilter extends object>(props: IDataTableProps<TData, TFilter>) =>
{
  const {
    columns,
    visibleColumns,
    data,
    filter,
    sort = { direction: SortDirection.None },
    tooltip,
    loading,
    error,
    filterVisible,
    selectedRows,
    formatter = ({ value }) => value,
    onFilterVisibilityChange,
    onSortChange,
    onRowsSelectionChange,
    onRowClick,
  } = props;

  const [isFilterVisible, setFilterVisible] = useState(false);
  const [isCheckAllCheckboxesChecked, setCheckAllCheckboxesChecked] = useState(false);
  const [internalSelectedRows, setInternalSelectedRows] = useState<TData[]>(selectedRows ?? []);

  const columnsToShow = useMemo(
    () => visibleColumns && visibleColumns.length > 0
      ? visibleColumns
        .map(columnName => columns.find(column => column.name === columnName))
        .filter(column => !!column) as IDataTableColumn<TData, TFilter>[]
      : columns,
    [columns, visibleColumns]);
  const isAnyColumnFilterDefined = useMemo(() => columnsToShow.some(column => column.filter?.type), [columnsToShow]);

  const additionalColumnsCount = useMemo(() =>
  {
    return onRowsSelectionChange ? 1 : 0;
  }, [onRowsSelectionChange]);

  // uncheck all checkboxes on data change
  useEffect(() =>
  {
    setCheckAllCheckboxesChecked(false);
    setInternalSelectedRows([]);
    if (onRowsSelectionChange) onRowsSelectionChange([]);
  }, [data, onRowsSelectionChange]);

  // update internal selected rows from property
  useEffect(() =>
  {
    const newSelectedRows = selectedRows ?? [];
    setInternalSelectedRows(prevSelectedRows =>
      _.xorBy(prevSelectedRows, newSelectedRows, row => row.id).length < 1
        ? prevSelectedRows
        : newSelectedRows
    );
  }, [selectedRows]);

  const toggleAllRowCheckboxesHandler = useCallback((_e: SyntheticEvent, d: CheckboxProps) =>
  {
    const newSelectedRows = d.checked && data ? data : [];
    setCheckAllCheckboxesChecked(prevChecked => !prevChecked);
    setInternalSelectedRows(newSelectedRows);
    if (onRowsSelectionChange) onRowsSelectionChange(newSelectedRows);
  }, [data, onRowsSelectionChange]);

  const toggleRowCheckboxHandler = useCallback((checked: boolean | undefined, data: TData) =>
  {
    setInternalSelectedRows(prevSelectedRows =>
    {
      const newSelectedRows =
        checked
          ? prevSelectedRows.concat(data)
          : prevSelectedRows.filter(row => row.id !== data.id);

      if (onRowsSelectionChange) onRowsSelectionChange(newSelectedRows);
      return newSelectedRows;
    });
  }, [onRowsSelectionChange]);

  const rowSelectHandler = useCallback((data: TData) =>
  {
    // prevent redirect if cell text is selected (to allow copy selected text into clipboard)
    const selection = window.getSelection();
    if (selection && !selection.isCollapsed) return;

    if (onRowClick) onRowClick(data);
  }, [onRowClick]);

  const filterVisibilityToggleHandler = useCallback((show: boolean) =>
  {
    setFilterVisible(show);
    if (onFilterVisibilityChange) onFilterVisibilityChange(show);
  }, [onFilterVisibilityChange]);

  const sortChangeHandler = useCallback((event: SyntheticEvent) =>
  {
    if (!onSortChange) return;

    const eventTarget: HTMLElement = event.currentTarget as HTMLElement;
    if (!eventTarget)
    {
      return;
    }

    const sortedColumn = eventTarget.innerText.trim();

    const newSort = { ...sort };
    if (newSort.column === sortedColumn)
    {
      newSort.direction = toggleSortDirection(newSort.direction);
      if (newSort.direction === SortDirection.None)
      {
        newSort.column = undefined;
      }
    }
    else
    {
      newSort.column = sortedColumn;
      newSort.direction = SortDirection.Ascending;
    }

    if (!_.isEqual(sort, newSort))
    {
      onSortChange(newSort);
    }
  }, [sort, onSortChange]);

  const determineSortIcon = (tableSort: IDataTableSort, column: string) =>
  {
    if (tableSort.column !== column) return undefined;

    if (tableSort.direction === SortDirection.Ascending) return 'sort up';
    if (tableSort.direction === SortDirection.Descending) return 'sort down';

    return 'sort';
  };

  // use internal state if filter visibility is uncontrolled
  const showFilter = filterVisible === undefined ? isFilterVisible : filterVisible;

  return (
    <Dimmer.Dimmable>
      <Dimmer inverted active={loading}>
        <Loader />
      </Dimmer>

      {/* div container to show horizontal scroll bar if table is too wide
          minHeight is hack to have enough space for dropdown menus - prevents vertical scroll bar on this container */}
      <div style={{
        overflow: 'auto',
        ...(isAnyColumnFilterDefined ? { minHeight: '30em' } : {}),
      }}>
        <Table
          // selectable only when showing data and onRowClick handler is defined
          selectable={!!onRowClick && !error}
          // maxWidth is 99% to not show horizontal scroll bar if table could fit to viewport
          style={{ border: '0 none', boxSizing: 'border-box', maxWidth: '99%' }}
        >
          <Table.Header>
            <Table.Row>
              {onRowsSelectionChange &&
                <Table.Cell rowSpan={2} style={{ verticalAlign: 'bottom' }}>
                  <Checkbox checked={isCheckAllCheckboxesChecked} onChange={toggleAllRowCheckboxesHandler} />
                </Table.Cell>
              }
              {columnsToShow.map(column => (
                <Table.Cell
                  key={column.name}
                  className='TableHeaderCell'
                  title={(column.sortable && onSortChange) ? `Click to sort by ${column.name}` : undefined}
                  style={(column.sortable && onSortChange) ? { cursor: 'pointer' } : undefined}
                  onClick={(column.sortable && onSortChange) ? sortChangeHandler : undefined}
                >
                  <span>{column.name}</span>
                  <div
                    style={{ float: 'right' }}
                  >
                    <Icon
                      size='small'
                      name={determineSortIcon(sort, column.name)}
                    />
                    <Icon
                      size='small'
                      name={isFilterValueEmpty(filter, column.filter?.getter) ? undefined : 'filter'}
                    />
                  </div>

                </Table.Cell>
              ))}
            </Table.Row>
            {isAnyColumnFilterDefined &&
            <DataTableFilter
              columns={columnsToShow}
              filter={filter}
              isVisible={showFilter}
            />}
          </Table.Header>

          <Table.Body>
            {isAnyColumnFilterDefined &&
            <TableExpandToggle
              colSpan={columnsToShow.length + additionalColumnsCount}
              toggled={showFilter}
              onToggle={filterVisibilityToggleHandler}
            />}

            {error
              ? <Table.Row>
                  <Table.Cell
                    error
                    selectable={false}
                    textAlign='center'
                    colSpan={columnsToShow.length + additionalColumnsCount}
                  >
                    <Icon name='times circle' />
                    {error}
                  </Table.Cell>
                </Table.Row>
              : data?.map(row => (
                  <Table.Row
                    key={row.id}
                    title={tooltip}
                    style={onRowClick && { cursor: 'pointer' }}
                    onClick={() => rowSelectHandler(row)}
                  >
                    {onRowsSelectionChange &&
                      <Table.Cell>
                        <Checkbox
                          checked={internalSelectedRows.map(selectedRow => selectedRow.id).includes(row.id)}
                          onChange={(e, d) => { e.stopPropagation(); toggleRowCheckboxHandler(d.checked, row); }}/>
                      </Table.Cell>
                    }
                    {columnsToShow.map(column => (
                      <Table.Cell
                        key={`${column.name} ${row.id}`}
                        className='TableBodyCell'
                        {... column.tooltip ? { title: formatCellValue(getPropertyValue(row, column.tooltip)) } : {}}
                      >
                        {formatter({
                          column: column,
                          data: row,
                          value: formatCellValue(getPropertyValue(row, column.value)),
                        })}
                      </Table.Cell>
                    ))}
                  </Table.Row>
              ))
            }
          </Table.Body>
        </Table>
      </div>
    </Dimmer.Dimmable>
  );
};

export default DataTable;
