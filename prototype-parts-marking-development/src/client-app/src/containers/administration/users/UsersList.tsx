import _ from 'lodash';
import React, { SyntheticEvent, useCallback, useEffect, useMemo, useState } from 'react';
import { RouteComponentProps } from 'react-router-dom';
import { Button, Dropdown, DropdownProps, Form, Grid, Header, Search, SearchProps } from 'semantic-ui-react';

import agent from '../../../api/agent';
import { extractErrorDetails } from '../../../api/utilities';
import UsersTable from '../../../components/users/UsersTable';
import usePermissions from '../../../hooks/usePermissions';
import { IUsersResponse } from '../../../models/api/responses';
import { IUserData } from '../../../models/api/users/userData';
import { debounceInputChangeEventHandler, textSearchTimeout } from '../../../utilities/events';
import { toastDistinctError } from '../../../utilities/toast';
import { UserActiveState, UserActiveStateOptions, convertUserActiveStateToTristateBoolean } from './usersUtilities';

interface IUsersListProps extends RouteComponentProps
{
}

const UsersList: React.FC<IUsersListProps> = ({
  history,
  match,
}) =>
{
  const { canModifyUsers } = usePermissions();

  const [usersActiveStateSelection, setUsersActiveStateSelection] = useState(UserActiveState.Active);
  const [query, setQuery] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [loadingUsers, setLoadingUsers] = useState(false);
  const [users, setUsers] = useState<IUsersResponse>();

  useEffect(() =>
  {
    setLoadingUsers(true);
    const showOnlyActiveUsers = convertUserActiveStateToTristateBoolean(usersActiveStateSelection);
    agent.Users.list({ search: query, isActive: showOnlyActiveUsers }, pageNumber)
      .then(response => setUsers(response))
      .catch(error => toastDistinctError('Couldn\'t list users:', extractErrorDetails(error)))
      .finally(() => setLoadingUsers(false));
  }, [query, usersActiveStateSelection, pageNumber]);

  const selectedActiveStateChangeHandler = useCallback((_event: SyntheticEvent, data: DropdownProps) =>
  {
    setUsersActiveStateSelection(data.value ? String(data.value) : '');
  }, []);

  const userSearchHandler = useMemo(() => debounceInputChangeEventHandler(
    (_e: SyntheticEvent<HTMLElement, MouseEvent>, data: SearchProps) =>
    {
      const query = data.value?.trim() ?? '';
      setQuery(query);
      setPageNumber(1);
    },
    textSearchTimeout),
  []);

  const userRowClickHandler = useCallback((user: IUserData) =>
  {
    history.push(`${match.url}/${user.id}`);
  }, [history, match]);

  return (
    <Grid container>
      <Grid.Row>
        <Header as='h2'>
          <Dropdown
            inline
            selectOnBlur={false}
            selectOnNavigation={false}
            value={usersActiveStateSelection}
            options={UserActiveStateOptions}
            onChange={selectedActiveStateChangeHandler}
          />
          users
        </Header>
      </Grid.Row>
      <Grid.Row>
        <Form>
          <Search
            fluid
            size='tiny'
            showNoResults={false}
            loading={loadingUsers}
            onSearchChange={userSearchHandler}
          />
        </Form>
        {canModifyUsers &&
          <Button
            basic
            icon='user plus'
            size='medium'
            onClick={() => history.push('/admin/users/new')}
          />
        }
      </Grid.Row>
      <Grid.Row>
        <UsersTable
          data={users}
          pageNumber={pageNumber}
          onPageNumberChange={setPageNumber}
          onRowClick={userRowClickHandler}
        />
      </Grid.Row>
    </Grid>
  );
};

export default UsersList;
