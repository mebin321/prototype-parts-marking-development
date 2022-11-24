import React from 'react';
import { Field, Form as FinalForm } from 'react-final-form';
import { ConnectedProps, connect } from 'react-redux';
import { Redirect } from 'react-router-dom';
import { combineValidators, isRequired } from 'revalidate';
import { Button, Form, Grid, Header, Image, Label, Segment } from 'semantic-ui-react';

import TextField from '../../components/common/form/TextField';
import { IApplicationState, login } from '../../store';

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    accessToken: state.auth.accessToken,
    error: state.auth.error,
    loading: state.auth.loading,
  };
};

const mapDispatchToProps =
{
  onLogin: login,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

interface ILoginFormProps extends ConnectedProps<typeof connector>
{
}

interface IUserFormValues
{
  userName: string;
  password: string;
}

// Form Validation
const validate = combineValidators({
  userName: isRequired({ message: 'User name is required' }),
  password: isRequired({ message: 'Password is required' }),
});

const LoginForm: React.FC<ILoginFormProps> = ({
  accessToken,
  error,
  loading,
  onLogin,
}) =>
{
  const finalFormSubmitHandler = async (values: IUserFormValues) =>
  {
    onLogin(values.userName, values.password);
  };

  if (accessToken.token) return <Redirect to='/' />;

  return (
    <Grid textAlign='center' style={{ height: '100vh' }} verticalAlign='middle'>

      <Grid.Column style={{ maxWidth: 450 }}>
        <Header as='h2' textAlign='center'>
          <Image src='/favicon.ico' size='mini' />
        Log-in to your account
        </Header>
        <FinalForm
          onSubmit={finalFormSubmitHandler}
          validate={validate}
          render={({ handleSubmit, invalid, pristine }) => (
            <Form size='large' onSubmit={handleSubmit}>
              <Segment>
                <Field
                  autoFocus
                  fluid
                  name='userName'
                  placeholder='User Name'
                  icon='user'
                  iconPosition='left'
                  component={TextField}
                />
                <Field
                  fluid
                  name='password'
                  type='password'
                  placeholder='Password'
                  icon='lock'
                  iconPosition='left'
                  component={TextField}
                />
                <Button
                  primary
                  fluid
                  size='large'
                  type='submit'
                  content='Login'
                  disabled={pristine || invalid}
                  loading={loading}
                />
              </Segment>
            </Form>
          )}
        />
        { error && <Label basic color='red'>{error}</Label> }
      </Grid.Column>
    </Grid>
  );
};

export default connector(LoginForm);
