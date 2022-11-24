import _ from 'lodash';
import React, { SyntheticEvent, useCallback, useMemo, useState } from 'react';
import { toast } from 'react-toastify';
import { Button, Container, Label, Search, SearchProps } from 'semantic-ui-react';

import agent from '../../../api/agent';
import { extractErrorDetails } from '../../../api/utilities';
import usePermissions from '../../../hooks/usePermissions';
import { IAdUser } from '../../../models/api/adUsers/adUser';
import { debounceInputChangeEventHandler, textSearchTimeout } from '../../../utilities/events';
import { toastDistinctError } from '../../../utilities/toast';
import styles from './NewUserForm.module.css';

const NewUserForm: React.FC = () =>
{
  const { canEnableUsers } = usePermissions();

  const [loadingUsers, setLoadingUsers] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [availableUsers, setAvailableUsers] = useState<IAdUser[]>([]);
  const [selectedUser, setSelectedUser] = useState('');

  const searchHandler = useMemo(() => debounceInputChangeEventHandler(
    (_e: SyntheticEvent<HTMLElement, MouseEvent>, data: SearchProps) =>
    {
      const query = data.value?.trim();
      if (!query)
      {
        setAvailableUsers([]);
        setSelectedUser('');
        return;
      }

      setLoadingUsers(true);
      agent.AdUsers
        .search(query)
        .then(adUsers => setAvailableUsers(adUsers))
        .catch(error => toastDistinctError('Couldn\'t search users:', extractErrorDetails(error)))
        .finally(() => setLoadingUsers(false));
    },
    textSearchTimeout),
  []);

  const userSelectHandler = useCallback((e: SyntheticEvent<HTMLSelectElement>) =>
  {
    setSelectedUser(e.currentTarget.value);
  }, []);

  const restoreUser = useCallback(async () =>
  {
    // If the name has a common name base as other items, choose one that is exactly the same
    const selectedUsersDetails = await agent.Users.list({ isActive: false, search: selectedUser });
    const selectedUserDetail = selectedUsersDetails.items.find(user => user.username === selectedUser);

    if (canEnableUsers && selectedUserDetail !== undefined && selectedUserDetail.deletedAt !== null)
    {
      await agent.Users.restore(selectedUserDetail.id);
      return true;
    }

    return false;
  }, [canEnableUsers, selectedUser]);

  const createUser = useCallback(async () =>
  {
    try
    {
      await agent.Users.create(selectedUser);
    }
    catch (error)
    {
      if (!restoreUser())
      {
        throw error;
      }
    }

    setAvailableUsers([]);
    setSelectedUser('');
    toast.success(`Successfully added user ${selectedUser}`, { autoClose: 5000 });
  }, [selectedUser, restoreUser]);

  const submitHandler = async () =>
  {
    setSubmitting(true);
    if (!selectedUser)
    {
      return;
    }

    createUser()
      .catch(async (error) =>
      {
        toast.error(`Couldn't add user: ${extractErrorDetails(error)}`);
      })
      .finally(() => setSubmitting(false));
  };

  return (
    <Container fluid>
      <Label className='simple'>Type e-mail address or account ID</Label>
      <Search
        fluid
        size='mini'
        className={styles.searchAdUser}
        showNoResults={false}
        loading={loadingUsers}
        onSearchChange={searchHandler}
      />
      <Container fluid style={{ margin: '1em' }}>
        <Label className='simple' style={{ display: 'block' }}>Select from the list of available users</Label>
        <select
          size={10}
          style={{ minWidth: '300px', width: '50%', margin: '0.833em', overflow: 'auto' }}
          onChange={userSelectHandler}
        >
          {availableUsers.map(user =>
            <option key={user.username} label={`${user.name} - ${user.username}`}>{user.username}</option>)}
        </select>
        <Container fluid>
          <Button
            primary
            name='submit'
            content='Add'
            disabled={!selectedUser}
            loading={submitting}
            onClick={submitHandler}
          />
        </Container>
      </Container>
    </Container>
  );
};

export default NewUserForm;
