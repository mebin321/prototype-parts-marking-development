import React, { Fragment } from 'react';
import { Route, Switch } from 'react-router-dom';
import { Menu } from 'semantic-ui-react';

import NavigationHorizontalMenuItem from '../../../components/navigation/NavigationHorizontalMenuItem';
import ComponentsExtendedList from './component/ComponentsExtendedList';
import PrototypesExtendedList from './prototype/PrototypesExtendedList';
import PrototypeSetsList from './set/PrototypeSetsList';

const PartsPage: React.FC = () =>
{
  return (
    <Fragment>
        <Menu pointing secondary>
          <NavigationHorizontalMenuItem to='/prototype-sets' title='Prototype Sets' />
          <NavigationHorizontalMenuItem to='/prototypes' title='Prototypes' />
          <NavigationHorizontalMenuItem to='/components' title='Components' />
        </Menu>

        <Switch>
          <Route path={'/prototype-sets'} exact component={PrototypeSetsList} />
          <Route path={'/prototypes'} exact component={PrototypesExtendedList} />
          <Route path={'/components'} exact component={ComponentsExtendedList} />
        </Switch>
    </Fragment>
  );
};

export default PartsPage;
