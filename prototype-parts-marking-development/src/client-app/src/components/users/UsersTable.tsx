import React from 'react';
import { Grid } from 'semantic-ui-react';

import { IUsersResponse } from '../../models/api/responses';
import { IUserData } from '../../models/api/users/userData';
import PaginationControls from '../common/ui/PaginationControls';
import DataTable from '../common/ui/table/DataTable';
import { IDataTableColumn } from '../common/ui/table/dataTableColumn';

interface IUsersTableProps
{
  data?: IUsersResponse;
  pageNumber?: number;
  loading?: boolean;
  error?: string;
  onPageNumberChange?: (pageNumber: number) => void;
  onRowClick?: (data: IUserData) => void;
}

const columns: IDataTableColumn<IUserData>[] =
[
  { name: 'Full Name', value: 'name' },
  { name: 'Login Name', value: 'username' },
  { name: 'Email', value: 'email' },
];

const UsersTable: React.FC<IUsersTableProps> = ({
  data,
  loading,
  error,
  onPageNumberChange,
  onRowClick,
}) =>
{
  return (
    <Grid container>
      <Grid.Row>
        <DataTable
          columns={columns}
          data={data?.items}
          tooltip='Click to show &amp; edit user details'
          loading={loading}
          error={error}
          onRowClick={onRowClick}
        />
      </Grid.Row>

      <Grid.Row>
        <PaginationControls
          totalPages={data?.pagination?.totalPages}
          onPageNumberChange={onPageNumberChange}
          pageNumber={data?.pagination?.page}
        />
      </Grid.Row>
    </Grid>
  );
};

export default UsersTable;
