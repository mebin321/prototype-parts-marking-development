import _ from 'lodash';
import React, { FormEvent, Fragment, useCallback, useEffect, useState } from 'react';
import { Field } from 'react-final-form';
import { RouteComponentProps } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Button, CheckboxProps, Header, Table } from 'semantic-ui-react';

import agent from '../../../api/agent';
import { extractErrorDetails, extractErrorDetailsFromPutResponse, listAll } from '../../../api/utilities';
import CheckBoxField from '../../../components/common/form/CheckBoxField';
import usePermissions from '../../../hooks/usePermissions';
import { IUserRole } from '../../../models/api/roles/userRole';
import { toastDistinctError } from '../../../utilities/toast';

interface IRolesAssignment extends RouteComponentProps
{
  userId: number;
}

const RolesAssignment: React.FC<IRolesAssignment> = ({
  userId,
  history,
}) =>
{
  const { canModifyUserRole } = usePermissions();

  const [assignedUserRoles, setAssignedUserRoles] = useState<IUserRole[]>([]);
  const [availableRoles, setAvailableRoles] = useState<IUserRole[]>([]);
  const [initialUserRoles, setInitialUserRoles] = useState<IUserRole[]>([]);
  const [rolesChanged, setRolesChanged] = useState(false);

  useEffect(() =>
  {
    const loadUserData = async () =>
    {
      try
      {
        const selectedUserRoles = await agent.Users.listRoles(userId);
        setAssignedUserRoles(selectedUserRoles);
        setInitialUserRoles(selectedUserRoles);

        const items = await listAll((page: number) => agent.Roles.list(undefined, page));
        setAvailableRoles(items);
      }
      catch (error)
      {
        toastDistinctError(`Couldn't load user with ID ${userId}:`, extractErrorDetails(error));
      }
    };

    loadUserData();
  }, [userId]);

  useEffect(() =>
  {
    setRolesChanged(initialUserRoles.length !== assignedUserRoles.length ||
        _.intersectionBy(initialUserRoles, assignedUserRoles, role => role.moniker).length !== initialUserRoles.length);
  }, [initialUserRoles, assignedUserRoles]);

  const roleCheckboxChangeHandler = useCallback((checked: boolean | undefined, role: IUserRole) =>
  {
    setAssignedUserRoles(prevRoles => checked
      ? prevRoles?.concat(role)
      : prevRoles?.filter(rol => rol.moniker !== role.moniker));
  }, []);

  const updateUserRoles = async () =>
  {
    if (assignedUserRoles === undefined)
    {
      return;
    }

    try
    {
      await agent.Users.updateRoles(userId, assignedUserRoles.map(role => role.moniker));
      toast.success(`Successfully updated roles for user no.: ${userId}`, { autoClose: 5000 });
      history.goBack();
    }
    catch (error)
    {
      toast.error(`Couldn't update user roles: ${extractErrorDetailsFromPutResponse(error)}`);
    }
  };

  return (
    <Fragment>
      <Header as='h2' style={{ marginTop: '0.5em' }}>Roles</Header>
      {availableRoles && assignedUserRoles && (
       <Table celled selectable >
        <Table.Body>
        {availableRoles.map(role =>
          <Table.Row key={role.moniker}>
            <Table.Cell>
              {role.title}
            </Table.Cell>
            <Table.Cell style={{ borderLeft: '0 none' }}>
              <Field
                style={{ verticalAlign: 'center' }}
                name={role.title}
                disabled={!canModifyUserRole}
                checked={assignedUserRoles.map(r => r.title).includes(role.title)}
                onChange={(_e: FormEvent<HTMLInputElement>, d: CheckboxProps) =>
                  roleCheckboxChangeHandler(d.checked, role)}
                component={CheckBoxField}
              />
            </Table.Cell>
          </Table.Row>
        )}
        </Table.Body>
      </Table>
      )}
      {canModifyUserRole &&
        <Button
          primary
          type='button'
          content='Update user roles'
          disabled={!rolesChanged}
          onClick={updateUserRoles}
        />
      }
    </Fragment>
  );
};

export default RolesAssignment;
