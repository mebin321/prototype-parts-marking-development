import _ from 'lodash';
import React, { Fragment, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { Button, DropdownItemProps, Icon, Menu } from 'semantic-ui-react';

import { extractErrorDetails } from '../../../../api/utilities';
import PaginationControls from '../../../../components/common/ui/PaginationControls';
import DataTable from '../../../../components/common/ui/table/DataTable';
import { IDataTableColumn } from '../../../../components/common/ui/table/dataTableColumn';
import { IDataTableSort, dataTableSortFromSortParameters } from '../../../../components/common/ui/table/dataTableSort';
import { getColumnPropertyName } from '../../../../components/common/ui/table/dataTableUtilities';
import TableDisplaySettingsModal from '../../../../components/common/ui/table/TableDisplaySettingsModal';
import
{
  ItemActiveState,
  clearHiddenColumnsFilter,
  createCustomerFilter,
  createDateTimeRangeFilter,
  createGateLevelsEnumFilter,
  createItemUniqueIdentifierFilter,
  createLocationsEnumFilter,
  createOutletsEnumFilter,
  createProductGroupsEnumFilter,
  createProjectNameFilter,
  createProjectNumberFilter,
  createScrappedItemFilter,
  createUserFilter,
  createYearsRangeFilter,
  fetchCustomersAsDropdownOptions,
} from '../../../../containers/items/itemsFilterUtilities';
import { IPrototypeSet } from '../../../../models/api/items/part/set/prototypeSet';
import { IPrototypeSetFilter } from '../../../../models/api/items/part/set/prototypeSetFilter';
import { IPrototypeSetsResponse } from '../../../../models/api/responses';
import { ISortParameters } from '../../../../models/api/sort/sortParameters';
import { IApplicationState, setTableSettings } from '../../../../store';
import { capitalizeFirstWord } from '../../../../utilities/text';
import { toastDistinctError } from '../../../../utilities/toast';
import { itemCellFormatter } from '../../itemsTableUtilities';
import { formatUser } from '../../itemsUtilities';
import { formatPrototypePartCode } from '../partUtilities';

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    tableSettings: state.configuration.prototypeSets.tableSettings,
    sortableProperties: state.configuration.prototypeSets.itemOptions.sortableColumns,
  };
};

const mapDispatchToProps =
{
  storeTableSettings: setTableSettings,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

interface IPrototypeSetsTableProps extends ConnectedProps<typeof connector>
{
  filtered?: boolean;
  data?: IPrototypeSetsResponse;
  filter?: IPrototypeSetFilter;
  sort?: ISortParameters;
  loading?: boolean;
  error?: string;
  filterVisible?: boolean;
  onFilterVisibilityChange?: (visible: boolean) => void;
  onFilterClear?: () => void;
  onPageNumberChange?: (pageNumber: number) => void;
  onFilterChange?: (filter: IPrototypeSetFilter) => void;
  onSortChange?: (sort: ISortParameters) => void;
  onRowsSelectionChange?: (selection: IPrototypeSet[]) => void;
  onRowClick?: (data: IPrototypeSet) => void;
}

const PrototypeSetsTable: React.FC<IPrototypeSetsTableProps> = ({
  filtered,
  data,
  filter,
  sort,
  loading,
  error,
  filterVisible,
  sortableProperties,
  tableSettings,
  storeTableSettings,
  onFilterVisibilityChange,
  onFilterClear,
  onPageNumberChange,
  onFilterChange,
  onSortChange,
  onRowsSelectionChange,
  onRowClick,
}) =>
{
  const allowOnFilterChangeRef = useRef(false);

  const [internalFilter, setInternalFilter] = useState(filter ?? {});
  const [showTableSettings, setShowTableSettings] = useState(false);
  const [customerOptions, setCustomerOptions] = useState<DropdownItemProps[]>();

  const isColumnSortable = useCallback((propertyName: string) =>
  {
    return sortableProperties?.includes(capitalizeFirstWord(propertyName)) ?? false;
  }, [sortableProperties]);

  const columns: IDataTableColumn<IPrototypeSet, IPrototypeSetFilter>[] = useMemo(() =>
    [
      {
        name: 'Part Code',
        value: item => formatPrototypePartCode(item, '??', 0),
        sortable: false,
      },
      {
        name: 'Outlet',
        value: 'outletTitle',
        sortable: isColumnSortable('outletTitle'),
        filter: filtered
          ? createOutletsEnumFilter('outletTitles', setInternalFilter)
          : undefined,
      },
      {
        name: 'Product Group',
        value: 'productGroupTitle',
        sortable: isColumnSortable('productGroupTitle'),
        filter: filtered
          ? createProductGroupsEnumFilter('productGroupTitles', setInternalFilter)
          : undefined,
      },
      {
        name: 'Evidence Year',
        value: 'evidenceYearTitle',
        sortable: isColumnSortable('evidenceYearTitle'),
        filter: filtered
          ? createYearsRangeFilter('evidenceYearLowerLimit', 'evidenceYearUpperLimit', setInternalFilter)
          : undefined,
      },
      {
        name: 'Location',
        value: 'locationTitle',
        sortable: isColumnSortable('locationTitle'),
        filter: filtered
          ? createLocationsEnumFilter('locationTitles', setInternalFilter)
          : undefined,
      },
      {
        name: 'Identifier',
        value: 'setIdentifier',
        sortable: isColumnSortable('setIdentifier'),
        filter: filtered
          ? createItemUniqueIdentifierFilter('setIdentifiers', setInternalFilter)
          : undefined,
      },
      {
        name: 'Gate Level',
        value: 'gateLevelTitle',
        sortable: isColumnSortable('gateLevelTitle'),
        filter: filtered
          ? createGateLevelsEnumFilter('gateLevelTitles', setInternalFilter)
          : undefined,
      },
      {
        name: 'Customer',
        value: 'customer',
        sortable: isColumnSortable('customer'),
        filter: filtered && customerOptions
          ? createCustomerFilter('customers', setInternalFilter, customerOptions)
          : undefined,
      },
      {
        name: 'Project Number',
        value: 'projectNumber',
        sortable: isColumnSortable('projectNumber'),
        filter: filtered
          ? createProjectNumberFilter('projectNumbers', setInternalFilter)
          : undefined,
      },
      {
        name: 'Project Name',
        value: 'project',
        sortable: isColumnSortable('project'),
        filter: filtered
          ? createProjectNameFilter('projects', setInternalFilter)
          : undefined,
      },
      {
        name: 'Created Date',
        value: 'createdAt',
        sortable: isColumnSortable('createdAt'),
        filter: filtered
          ? createDateTimeRangeFilter('createdAtLowerLimit', 'createdAtUpperLimit', setInternalFilter)
          : undefined,
      },
      {
        name: 'Created By',
        value: item => formatUser(item.createdBy),
        sortable: isColumnSortable('createdBy'),
        filter: filtered
          ? createUserFilter('createdBy', setInternalFilter)
          : undefined,
      },
      {
        name: 'Modified Date',
        value: 'modifiedAt',
        sortable: isColumnSortable('modifiedAt'),
        filter: filtered
          ? createDateTimeRangeFilter('modifiedAtLowerLimit', 'modifiedAtUpperLimit', setInternalFilter)
          : undefined,
      },
      {
        name: 'Modified By',
        value: item => formatUser(item.modifiedBy),
        sortable: isColumnSortable('modifiedBy'),
        filter: filtered
          ? createUserFilter('modifiedBy', setInternalFilter)
          : undefined,
      },
      {
        name: 'Deleted Date',
        value: 'deletedAt',
        sortable: isColumnSortable('deletedAt'),
        filter: filtered
          ? createDateTimeRangeFilter('deletedAtLowerLimit', 'deletedAtUpperLimit', setInternalFilter)
          : undefined,
      },
      {
        name: 'Deleted By',
        value: item => formatUser(item.deletedBy),
        sortable: isColumnSortable('deletedBy'),
        filter: filtered
          ? createUserFilter('deletedBy', setInternalFilter)
          : undefined,
      },
      {
        name: 'Is Active',
        value: prototype => prototype.deletedAt !== null ? ItemActiveState.No : ItemActiveState.Yes,
        filter: filtered
          ? createScrappedItemFilter('isActive', setInternalFilter)
          : undefined,
      },
    ], [filtered, isColumnSortable, customerOptions]);

  useEffect(() =>
  {
    const loadCustomers = async () =>
    {
      try
      {
        setCustomerOptions(await fetchCustomersAsDropdownOptions());
      }
      catch (error)
      {
        toastDistinctError('Couldn\'t load customers filter options:', extractErrorDetails(error));
      }
    };

    loadCustomers();
  }, []);

  // update internal filter from property
  useEffect(() =>
  {
    const newFilter = filter ?? {};
    setInternalFilter(prevFilter =>
    {
      if (_.isEqual(prevFilter, newFilter)) return prevFilter;

      allowOnFilterChangeRef.current = false;
      return newFilter;
    });
  }, [filter]);

  useEffect(() =>
  {
    // prevent calling onFilterChange when initial filter value is set on mount and onFilterChange handler change
    if (!allowOnFilterChangeRef.current)
    {
      allowOnFilterChangeRef.current = true;
      return;
    }

    if (internalFilter && onFilterChange) onFilterChange(internalFilter);

    // onFilterChange is omitted in dependencies to prevent firing onFilterChange when onFilterChange handler is changed
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [internalFilter]);

  const sortChangeHandler = useCallback((sort: IDataTableSort) =>
  {
    if (!onSortChange) return;

    const propertyName = getColumnPropertyName(columns, sort.column);
    onSortChange({ sortBy: propertyName, sortDirection: sort.direction });
  }, [columns, onSortChange]);

  const refreshData = useCallback(() =>
  {
    if (!onPageNumberChange || !data?.pagination?.page) return;

    onPageNumberChange(data.pagination.page);
  }, [data, onPageNumberChange]);

  return (
    <Fragment>
      <DataTable
        columns={columns}
        visibleColumns={tableSettings.visibleColumns}
        data={data?.items}
        filter={internalFilter}
        sort={dataTableSortFromSortParameters(sort, columns)}
        tooltip='Click to show &amp; edit prototype set details'
        loading={loading}
        error={error}
        filterVisible={filterVisible}
        formatter={itemCellFormatter}
        onFilterVisibilityChange={onFilterVisibilityChange}
        onSortChange={sortChangeHandler}
        onRowsSelectionChange={onRowsSelectionChange}
        onRowClick={onRowClick}
      />

      <div style={{ display: 'flex', flexWrap: 'wrap' }}>
        <PaginationControls
          totalPages={data?.pagination?.totalPages}
          pageNumber={data?.pagination?.page}
          onPageNumberChange={onPageNumberChange}
        />
        <div style={{ width: '1em' }} />
        {/* wrap buttons in menu to introduce shadow identic to Pagination component */}
        <Menu style={{ margin: '0', border: '0 none transparent' }}>
          <Button.Group basic size='large'>
            <Button
              icon='sync alternate'
              title='Refresh'
              onClick={refreshData}
            />
            <Button
              icon={
                <Icon.Group>
                  <Icon name='table' />
                  <Icon corner='bottom right' name='setting' />
                </Icon.Group>
              }
              title='Table display settings'
              onClick={() => setShowTableSettings(true)}
            />
            {filtered && onFilterClear &&
              <Button
                icon='eraser'
                title='Clear filter'
                onClick={onFilterClear}
              />
            }
          </Button.Group>
        </Menu>
      </div>

      <TableDisplaySettingsModal
        visible={showTableSettings}
        availableColumns={columns}
        settings={tableSettings}
        onCancel={() => setShowTableSettings(false)}
        onConfirm={(settings) =>
        {
          setShowTableSettings(false);
          clearHiddenColumnsFilter(internalFilter, columns, tableSettings.visibleColumns, settings.visibleColumns);
          storeTableSettings('prototypeSets', settings);
        }}
      />
    </Fragment>
  );
};

export default connector(PrototypeSetsTable);
