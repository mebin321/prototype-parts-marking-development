import React from 'react';
import { Route, RouteComponentProps, Switch } from 'react-router-dom';
import { Grid, Menu } from 'semantic-ui-react';

import NavigationHorizontalMenuItem from '../../components/navigation/NavigationHorizontalMenuItem';
import usePermissions from '../../hooks/usePermissions';
import EnumsPage from './enumerations/EnumsPage';
import EnumRelationsPage from './enumerations/relations/EnumRelationsPage';
import NewUserForm from './users/NewUserForm';
import UserForm from './users/UserForm';
import UsersList from './users/UsersList';

interface IAdministrationPageProps extends RouteComponentProps
{
}

const AdministrationPage: React.FC<IAdministrationPageProps> = ({
  match,
}) =>
{
  const { canReadEntityRelations } = usePermissions();

  return (
    <Grid container>
      <Grid.Row>
        <Menu pointing secondary>
          <NavigationHorizontalMenuItem to={match.url + '/users'} title='Users' />
          <NavigationHorizontalMenuItem to={match.url + '/enumerations'} title='Enumerations' />
          {canReadEntityRelations &&
            <NavigationHorizontalMenuItem to={match.url + '/enumeration-relations'} title='Relations' />}
        </Menu>
      </Grid.Row>
      <Grid.Row>
        <Switch>
          <Route path={match.url + '/users'} exact component={UsersList} />
          <Route path={match.url + '/users/new'} exact component={NewUserForm} />
          <Route path={match.url + '/users/:userId'} exact component={UserForm} />
          <Route path={match.url + '/enumerations'} exact component={EnumsPage} />
          {canReadEntityRelations &&
            <Route path={match.url + '/enumeration-relations'} component={EnumRelationsPage} />}
        </Switch>
      </Grid.Row>
    </Grid>
  );
};

export default AdministrationPage;
