import { saveAs } from 'file-saver';
import React, { useCallback, useEffect, useState } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { toast } from 'react-toastify';
import { Button, Container, Header, Icon } from 'semantic-ui-react';

import agent from '../../api/agent';
import { extractErrorDetails, listAll } from '../../api/utilities';
import PaginationControls from '../../components/common/ui/PaginationControls';
import DataTable from '../../components/common/ui/table/DataTable';
import { IDataTableColumn } from '../../components/common/ui/table/dataTableColumn';
import RemovePrintItemPrompt from '../../components/print/RemovePrintItemPrompt';
import usePermissions from '../../hooks/usePermissions';
import { IPrintItem } from '../../models/api/print/printItem';
import { IPrintItemsResponse } from '../../models/api/responses';
import { IApplicationState } from '../../store';
import { IColumnOption, convertToCSV } from '../../utilities/csv';
import { transformDescription } from './printUtils';

export interface ICompletePrintItem extends IPrintItem
{
  fullDescription: string;
}

const exportedColumns: IColumnOption<ICompletePrintItem>[] =
[
  { key: 'customer', header: 'Customer' },
  { key: 'productGroup', header: 'Product Group' },
  { key: 'partType', header: 'Part Type' },
  { key: 'description', header: 'Part Description' },
  { key: 'partCode', header: 'Part Code' },
  { key: 'createdAt', header: 'Created Date' },
  { key: 'fullDescription', header: 'Full Part Description' },
  { key: 'outlet', header: 'Outlet Title' },
  { key: 'location', header: 'Location Title' },
  { key: 'projectNumber', header: 'Project Number' },
  { key: 'gateLevel', header: 'Gate Level Title' },
  { key: 'materialNumber', header: 'Material Number' },
  { key: 'revisionCode', header: 'Revision Code' },
];

const tableColumns: IDataTableColumn<IPrintItem>[] =
[
  { name: 'Part Code', value: 'partCode' },
  { name: 'Customer', value: 'customer' },
  { name: 'Product Group', value: 'productGroup' },
  { name: 'Part Type', value: 'partType' },
  { name: 'Part Description', value: 'description' },
  { name: 'Created Date', value: 'createdAt' },
];

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    user: state.auth.user,
  };
};

const connector = connect(mapStateToProps);

interface IPrintingPageProps extends ConnectedProps<typeof connector>
{
}

const PrintingPage: React.FC<IPrintingPageProps> = ({
  user,
}) =>
{
  const { canModifyPrintingLabels } = usePermissions();
  const [data, setData] = useState<IPrintItemsResponse>();
  const [error, setError] = useState('');
  const [selectedItems, setSelectedItems] = useState<IPrintItem[]>([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [showDeleteSelectedConfirm, setShowDeleteSelectedConfirm] = useState(false);
  const [showDeleteEverythingConfirm, setShowDeleteEverythingConfirm] = useState(false);
  const [isLoading, setLoading] = useState(false);
  const [isDeleting, setDeleting] = useState(false);
  const [isExporting, setExporting] = useState(false);
  const [isPermissionsLoad, setPermissionsLoad] = useState(false);

  const loadItems = useCallback(async (page: number) =>
  {
    setLoading(true);
    await agent.PrintingLabels.list(user.id, page)
      .then(response => { setData(response); setSelectedItems([]); })
      .catch(error => setError(extractErrorDetails(error)))
      .finally(() => setLoading(false));
  }, [user.id]);

  useEffect(() =>
  {
    if (canModifyPrintingLabels)
    {
      loadItems(pageNumber);
      setPermissionsLoad(true);
    }
  }, [canModifyPrintingLabels, loadItems, pageNumber]);

  // Print page visited by entering address in to url.
  useEffect(() =>
  {
    isPermissionsLoad
      ? setError('')
      : setError('Please log in to respective role to see list of labels to print.');
  }, [isPermissionsLoad]);

  // when deleting all items on last page (e.g. 2 of 2 items on page 3)
  // then the current page will be outside of available pages (e.g. current page is 3 but total pages is 2)
  // therefore go to the last available page
  useEffect(() =>
  {
    if (!data) return;

    if (data.pagination.page > data.pagination.totalPages)
    {
      setPageNumber(data.pagination.totalPages);
    }
  }, [data, loadItems]);

  const deleteSelectedItemsHandler = async () =>
  {
    try
    {
      setDeleting(true);
      for (const item of selectedItems)
      {
        await agent.PrintingLabels.remove(item.id);
      }

      toast.success(
        `Successfully removed ${selectedItems.length} label${selectedItems.length > 1 ? 's' : ''} to print`,
        { autoClose: 5000 });
      loadItems(data?.pagination.page ?? 1);
    }
    catch (error)
    {
      toast.error(`Couldn't remove labels to print: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setDeleting(false);
      setShowDeleteSelectedConfirm(false);
    }
  };

  const deleteAllItemsHandler = async () =>
  {
    try
    {
      setDeleting(true);
      const totalPages = data?.pagination.totalPages ?? 0;
      let deletedItems = 0;
      for (let page = totalPages; page > 0; page--)
      {
        const response = await agent.PrintingLabels.list(user.id, page);
        for (const item of response.items)
        {
          await agent.PrintingLabels.remove(item.id);
          deletedItems++;
        }
      }

      toast.success(`Successfully removed all ${deletedItems} labels to print`, { autoClose: 5000 });
      loadItems(data?.pagination.page ?? 1);
    }
    catch (error)
    {
      toast.error(`Couldn't remove all labels to print: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setDeleting(false);
      setShowDeleteEverythingConfirm(false);
    }
  };

  const exportItemsHandler = useCallback(async () =>
  {
    try
    {
      setExporting(true);
      const items = await listAll(page => agent.PrintingLabels.list(user.id, page));
      const printItems = items.map(transformDescription);
      const csv = convertToCSV(printItems, exportedColumns);
      const blob = new Blob([csv], { type: 'text/plain;charset=utf-8' });
      saveAs(blob, 'PPMT-labels.csv');
    }
    catch (error)
    {
      toast.error(`Couldn't export labels: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setExporting(false);
    }
  }, [user.id]);

  return (
    <Container style={{ position: 'relative' }}>
      <Header textAlign='center' as='h1'>Labels to Print</Header>
      <DataTable
        columns={tableColumns}
        data={data?.items}
        error={error}
        loading={isLoading}
        selectedRows={selectedItems}
        onRowsSelectionChange={setSelectedItems}
      />
      <div style={{ display: 'flex', flexWrap: 'wrap', marginTop: '2em' }}>
        <PaginationControls
          totalPages={data?.pagination.totalPages}
          pageNumber={data?.pagination.page}
          onPageNumberChange={setPageNumber}
        />
        <div style={{ width: '1em' }} />
        {/* wrap buttons in group to enforce square dimensions */}
        <Button.Group size='large'>
          <Button
            basic
            color='red'
            icon={
              <Icon.Group>
                <Icon name='tasks' />
                <Icon corner='top right' name='remove' />
              </Icon.Group>
            }
            title='Delete selected items'
            disabled={selectedItems.length < 1}
            onClick={() => setShowDeleteSelectedConfirm(true)}
          />
        </Button.Group>
        <div style={{ width: '0.25em' }} />
        <Button.Group size='large'>
          <Button
            basic
            color='red'
            icon='remove'
            title='Delete all items'
            disabled={!data?.pagination.totalCount}
            onClick={() => setShowDeleteEverythingConfirm(true)}
          />
        </Button.Group>
        <Button
          primary
          content='Export to CSV'
          icon='download'
          loading={isExporting}
          disabled={!data?.pagination.totalCount}
          style={{ position: 'absolute', right: '0' }}
          onClick={exportItemsHandler}
        />
      </div>

      <RemovePrintItemPrompt
        all={false}
        count={selectedItems.length}
        visible={showDeleteSelectedConfirm}
        loading={isDeleting}
        onCancel={() => setShowDeleteSelectedConfirm(false)}
        onConfirm={deleteSelectedItemsHandler}
      />
      <RemovePrintItemPrompt
        all={true}
        count={data?.pagination.totalCount ?? 0}
        visible={showDeleteEverythingConfirm}
        loading={isDeleting}
        onCancel={() => setShowDeleteEverythingConfirm(false)}
        onConfirm={deleteAllItemsHandler}
      />
    </Container>
  );
};

export default connector(PrintingPage);
