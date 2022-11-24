import _ from 'lodash';
import React, { Fragment, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { Button, Icon, Menu } from 'semantic-ui-react';

import PaginationControls from '../../../../components/common/ui/PaginationControls';
import DataTable from '../../../../components/common/ui/table/DataTable';
import { IDataTableColumn } from '../../../../components/common/ui/table/dataTableColumn';
import TableDisplaySettingsModal from '../../../../components/common/ui/table/TableDisplaySettingsModal';
import { IPrototypeVariantExtended } from '../../../../models/api/items/part/variant/prototypeVariantExtended';
import { IPrototypeVariantFilter } from '../../../../models/api/items/part/variant/prototypeVariantFilter';
import { IPrototypeVariantsExtendedResponse } from '../../../../models/api/responses';
import { IApplicationState, setTableSettings } from '../../../../store';
import
{
  ItemActiveState,
  clearHiddenColumnsFilter,
  createScrappedItemFilter,
} from '../../itemsFilterUtilities';
import { itemCellFormatter } from '../../itemsTableUtilities';
import { formatUser, getItemCommentForTableListing } from '../../itemsUtilities';
import { formatPrototypePartCode } from '../partUtilities';

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    tableSettings: state.configuration.variants.tableSettings,
  };
};

const mapDispatchToProps =
{
  storeTableSettings: setTableSettings,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

interface IVariantsExtendedTableProps extends ConnectedProps<typeof connector>
{
  filtered?: boolean;
  data?: IPrototypeVariantsExtendedResponse;
  filter?: IPrototypeVariantFilter;
  loading?: boolean;
  error?: string;
  filterVisible?: boolean;
  onFilterVisibilityChange?: (visible: boolean) => void;
  onFilterClear?: () => void;
  onPageNumberChange?: (pageNumber: number) => void;
  onFilterChange?: (filter: IPrototypeVariantFilter) => void;
  onRowsSelectionChange?: (selection: IPrototypeVariantExtended[]) => void;
  onRowClick?: (data: IPrototypeVariantExtended) => void;
}

const VariantsExtendedTable: React.FC<IVariantsExtendedTableProps> = ({
  filtered,
  data,
  filter,
  loading,
  error,
  filterVisible,
  tableSettings,
  storeTableSettings,
  onFilterVisibilityChange,
  onFilterClear,
  onPageNumberChange,
  onFilterChange,
  onRowsSelectionChange,
  onRowClick,
}) =>
{
  const allowOnFilterChangeRef = useRef(false);

  const [internalFilter, setInternalFilter] = useState(filter ?? {});
  const [showTableSettings, setShowTableSettings] = useState(false);

  const columns: IDataTableColumn<IPrototypeVariantExtended, IPrototypeVariantFilter>[] = useMemo(() =>
    [
      {
        name: 'Part Code',
        value: variant => formatPrototypePartCode(
          variant.prototype.prototypeSet, variant.prototype.partTypeCode, variant.prototype.index),
      },
      {
        name: 'Outlet',
        value: variant => variant.prototype.prototypeSet.outletTitle,
      },
      {
        name: 'Product Group',
        value: variant => variant.prototype.prototypeSet.productGroupTitle,
      },
      {
        name: 'Part Type',
        value: variant => variant.prototype.partTypeTitle,
      },
      {
        name: 'Evidence Year',
        value: variant => variant.prototype.prototypeSet.evidenceYearTitle,
      },
      {
        name: 'Location',
        value: variant => variant.prototype.prototypeSet.locationTitle,
      },
      {
        name: 'Identifier',
        value: variant => variant.prototype.prototypeSet.setIdentifier,
      },
      {
        name: 'Gate Level',
        value: variant => variant.prototype.prototypeSet.gateLevelTitle,
      },
      {
        name: 'Part Number',
        value: variant => variant.prototype.index,
      },
      {
        name: 'Owner',
        value: variant => formatUser(variant.prototype.owner),
      },
      {
        name: 'Customer',
        value: variant => variant.prototype.prototypeSet.customer,
      },
      {
        name: 'Project Number',
        value: variant => variant.prototype.prototypeSet.projectNumber,
      },
      {
        name: 'Project Name',
        value: variant => variant.prototype.prototypeSet.project,
      },
      {
        name: 'Created Date',
        value: 'createdAt',
      },
      {
        name: 'Created By',
        value: variant => formatUser(variant.createdBy),
      },
      {
        name: 'Modified Date',
        value: 'modifiedAt',
      },
      {
        name: 'Modified By',
        value: variant => formatUser(variant.modifiedBy),
      },
      {
        name: 'Deleted Date',
        value: 'deletedAt',
      },
      {
        name: 'Deleted By',
        value: variant => formatUser(variant.deletedBy),
      },
      {
        name: 'Version',
        value: 'version',
      },
      {
        name: 'Comment',
        value: getItemCommentForTableListing,
        tooltip: 'comment',
      },
      {
        name: 'Is Active',
        value: variant => variant.deletedAt !== null ? ItemActiveState.No : ItemActiveState.Yes,
        filter: filtered
          ? createScrappedItemFilter('isActive', setInternalFilter)
          : undefined,
      },
    ], [filtered]);

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
    // prevent calling onFilterChange when initial filter value is set on mount
    if (!allowOnFilterChangeRef.current)
    {
      allowOnFilterChangeRef.current = true;
      return;
    }

    if (onFilterChange) onFilterChange(internalFilter);

    // onFilterChange is omitted in dependencies to prevent firing onFilterChange when onFilterChange handler is changed
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [internalFilter]);

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
        tooltip='Click to show &amp; edit prototype details'
        loading={loading}
        error={error}
        filterVisible={filterVisible}
        formatter={itemCellFormatter}
        onFilterVisibilityChange={onFilterVisibilityChange}
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
          storeTableSettings('variants', settings);
        }}
      />
    </Fragment>
  );
};

export default connector(VariantsExtendedTable);
