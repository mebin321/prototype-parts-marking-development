import React, { Fragment, useCallback, useMemo } from 'react';
import { ConnectedProps, connect } from 'react-redux';

import PaginationControls from '../../../components/common/ui/PaginationControls';
import DataTable from '../../../components/common/ui/table/DataTable';
import { IDataTableColumn } from '../../../components/common/ui/table/dataTableColumn';
import { IDataTableSort, dataTableSortFromSortParameters } from '../../../components/common/ui/table/dataTableSort';
import { getColumnPropertyName } from '../../../components/common/ui/table/dataTableUtilities';
import { IPrototype } from '../../../models/api/items/part/prototype';
import { IPrototypeSet } from '../../../models/api/items/part/set/prototypeSet';
import { IPrototypesResponse } from '../../../models/api/responses';
import { ISortParameters } from '../../../models/api/sort/sortParameters';
import { IApplicationState } from '../../../store';
import { capitalizeFirstWord } from '../../../utilities/text';
import { itemCellFormatter } from '../itemsTableUtilities';
import { formatPrototypePartCode } from './partUtilities';

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    sortableProperties: state.configuration.prototypes.itemOptions.sortableColumns,
  };
};

const connector = connect(mapStateToProps);

interface IPartsTableProps extends ConnectedProps<typeof connector>
{
  prototypeSet?: IPrototypeSet;
  data?: IPrototypesResponse;
  sort?: ISortParameters;
  loading?: boolean;
  error?: string;
  onPageNumberChange?: (pageNumber: number) => void;
  onSortChange?: (sort: ISortParameters) => void;
  onRowsSelectionChange?: (selection: IPrototype[]) => void;
  onPrototypeRowClick?: (data: IPrototype) => void;
}

const PartsTable: React.FC<IPartsTableProps> = ({
  prototypeSet,
  data,
  sort,
  loading,
  error,
  sortableProperties,
  onPageNumberChange,
  onSortChange,
  onRowsSelectionChange,
  onPrototypeRowClick,
}) =>
{
  const isColumnSortable = useCallback((propertyName: string) =>
  {
    return sortableProperties?.includes(capitalizeFirstWord(propertyName)) ?? false;
  }, [sortableProperties]);

  const columns = useMemo(() =>
  {
    const partColumns: IDataTableColumn<IPrototype>[] =
    [
      {
        name: 'Part Number',
        value: 'index',
        sortable: isColumnSortable('index'),
      },
      {
        name: 'Part Type',
        value: 'partTypeTitle',
        sortable: isColumnSortable('partTypeTitle'),
      },
    ];

    if (prototypeSet)
    {
      partColumns.unshift({
        name: 'Part Code',
        value: item => formatPrototypePartCode(prototypeSet, item.partTypeCode, item.index),
        sortable: false,
      });
    }

    return partColumns;
  }, [prototypeSet, isColumnSortable]);

  const sortChangeHandler = useCallback((sort: IDataTableSort) =>
  {
    if (!onSortChange) return;

    const propertyName = getColumnPropertyName(columns, sort.column);
    onSortChange({ sortBy: propertyName, sortDirection: sort.direction });
  }, [columns, onSortChange]);

  return (
    <Fragment>
      <DataTable
        columns={columns}
        data={data?.items}
        sort={dataTableSortFromSortParameters(sort, columns)}
        tooltip='Click to show &amp; edit prototype details'
        loading={loading}
        error={error}
        formatter={itemCellFormatter}
        onSortChange={sortChangeHandler}
        onRowsSelectionChange={onRowsSelectionChange}
        onRowClick={onPrototypeRowClick}
      />
      <PaginationControls
        totalPages={data?.pagination?.totalPages}
        pageNumber={data?.pagination?.page}
        onPageNumberChange={onPageNumberChange}
      />
    </Fragment>
  );
};

export default connector(PartsTable);
