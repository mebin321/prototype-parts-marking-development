import React, { useEffect, useState } from 'react';
import { Field, Form as FinalForm } from 'react-final-form';
import { ConnectedProps, connect } from 'react-redux';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { toast } from 'react-toastify';
import { combineValidators, composeValidators, isRequired } from 'revalidate';
import { Button, Container, Form, Header, Modal, Segment } from 'semantic-ui-react';

import agent from '../../../api/agent';
import { extractErrorDetails, extractErrorDetailsFromPutResponse } from '../../../api/utilities';
import TextField from '../../../components/common/form/TextField';
import usePermissions from '../../../hooks/usePermissions';
import { IUserData } from '../../../models/api/users/userData';
import { IUpdateUserData } from '../../../models/api/users/userUpdateData';
import { IApplicationState } from '../../../store';
import { parseIdFromParameter } from '../../../utilities/routing';
import { toastDistinctError } from '../../../utilities/toast';
import { isEmail } from '../../../utilities/validation/validators';
import RolesAssignment from './UserRolesAssignment';

const validate = combineValidators({
  name: isRequired('Full Name'),
  email: composeValidators(isRequired, isEmail)('Email'),
});

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    user: state.auth.user,
  };
};

const connector = connect(mapStateToProps);

interface IUserFormProps extends RouteComponentProps<{userId: string}>, ConnectedProps<typeof connector>
{
}

const UserForm: React.FC<IUserFormProps> = ({
  history,
  match,
  user,
  location,
}) =>
{
  const { canDisableUsers, canModifyUsers } = usePermissions();

  const [userData, setUserData] = useState<IUserData>();
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [loading, setLoading] = useState(false);
  const [updating, setUpdating] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [isCurrentUser, setIsCurrentUser] = useState(true);

  const userId = parseIdFromParameter('user', match.params.userId);

  useEffect(() =>
  {
    const loadUserData = async () =>
    {
      setLoading(true);
      try
      {
        const data = await agent.Users.read(userId);
        setUserData(data);
      }
      catch (error)
      {
        toastDistinctError(`Couldn't load user with ID ${userId}:`, extractErrorDetails(error));
      }
      finally
      {
        setLoading(false);
      }
    };

    loadUserData();
  }, [userId]);

  useEffect(() =>
  {
    if (!userData)
    {
      setIsCurrentUser(true);
      return;
    }

    if (userData?.id === user.id)
    {
      setIsCurrentUser(true);
    }
    else
    {
      setIsCurrentUser(false);
    }
  }, [userData, user.id]);

  const updateUserHandler = async (values: IUpdateUserData) =>
  {
    setUpdating(true);
    try
    {
      const userId = parseIdFromParameter('user', match.params.userId);
      const dataToUpdate = { name: values.name, email: values.email };
      await agent.Users.update(userId, dataToUpdate);
      setUpdating(false);
      toast.success(`Successfully updated user ${values.name}`, { autoClose: 5000 });
      history.goBack();
    }
    catch (error)
    {
      toast.error(`Couldn't update the user: ${extractErrorDetailsFromPutResponse(error)}`);
      setUpdating(false);
    }
  };

  const deleteUserHandler = async () =>
  {
    setDeleting(true);
    try
    {
      await agent.Users.remove(userId);
      setDeleting(false);
      toast.success(`Successfully deleted user ${userData?.name}`, { autoClose: 5000 });
      history.goBack();
    }
    catch (error)
    {
      toast.error(`Couldn't delete the user: ${extractErrorDetails(error)}`);
      setDeleting(false);
    }
    finally
    {
      setShowDeleteConfirm(false);
    }
  };

  const canModify = isCurrentUser || canModifyUsers;
  const canDelete = !isCurrentUser && canDisableUsers;

  return (
    <Container>
      <FinalForm onSubmit={updateUserHandler} validate={validate} render={({ handleSubmit, invalid, pristine }) => (
        <Form loading={loading} onSubmit={handleSubmit} style={{ maxWidth: '650px' }}>
        <Segment clearing>
          <Header as='h2'>Edit user</Header>
          <Field
            component={TextField}
            name='name'
            label='Full Name'
            readOnly={!canModify}
            initialValue={userData?.name}
          />
          <Field
            component={TextField}
            name='email'
            label='Email'
            readOnly={!canModify}
            initialValue={userData?.email}
          />
          <Field
            component={TextField}
            name='login'
            label='Login Name'
            readOnly={true}
            initialValue={userData?.username}
          />
          {canModify &&
            <Button
              primary
              type='submit'
              content='Update'
              disabled={invalid || pristine}
              loading={updating}
            />
          }
          {canDelete && (
            <Button
              negative
              type='button'
              content='Delete'
              floated='right'
              disabled={!userData}
              onClick={() => setShowDeleteConfirm(true)}
            />
          )}
          </Segment>

          <Segment clearing>
            <div style={{ maxWidth: 'max-content' }}>
              <RolesAssignment
                history={history}
                match={match}
                userId={userId}
                location={location}
              />
            </div>
          </Segment>

        </Form>
      )}>
      </FinalForm>

      <Modal
        size='tiny'
        open={showDeleteConfirm}
        closeOnEscape
        closeOnDimmerClick
        onClose={() => setShowDeleteConfirm(false)}
      >
        <Modal.Header>Delete User</Modal.Header>
        <Modal.Content>
          <p>Are you sure you want to remove user {userData?.name} ({userData?.username})?</p>
        </Modal.Content>
        <Modal.Actions>
          <Button onClick={() => setShowDeleteConfirm(false)}>No</Button>
          <Button negative loading={deleting} onClick={deleteUserHandler}>Yes</Button>
        </Modal.Actions>
      </Modal>
    </Container>
  );
};

export default withRouter(connector(UserForm));
