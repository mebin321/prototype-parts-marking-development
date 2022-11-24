import React, { Fragment } from 'react';
import { Accordion, Menu } from 'semantic-ui-react';

import { IUserData } from '../../../models/api/users/userData';
import Backdrop from '../../common/ui/Backdrop';
import { INavigationItemDefinition } from '../navigationItemDefinition';
import createNavigationAccordionItem from './NavigationAccordionItem';
import NavigationVerticalMenuItem from './NavigationVerticalMenuItem';
import classes from './SideDrawer.module.css';

interface ISideDrawerProps
{
  visible: boolean;
  items: INavigationItemDefinition[];
  user: IUserData;
  onClose: () => void;
}

const SideDrawer: React.FC<ISideDrawerProps> = ({
  visible,
  items,
  user,
  onClose,
}) =>
{
  const sideDrawerClasses = [classes.SideDrawer];
  sideDrawerClasses.push(visible ? classes.Open : classes.Close);

  return (
    <Fragment>
      <Backdrop visible={visible} onClick={onClose} />
      <div className={sideDrawerClasses.join(' ')}>
        <Menu fluid vertical style={{ height: '100%', overflowY: 'auto' }}>
          <Accordion panels={items.map(item => createNavigationAccordionItem(item, onClose))} />

          <NavigationVerticalMenuItem to='/admin' title='Settings' icon='setting' iconSize='large' onClick={onClose} />
          {user.username
            ? <Fragment>
                <NavigationVerticalMenuItem
                  to={`/admin/users/${user.id}`}
                  exact
                  title={user.name}
                  icon='user'
                  onClick={onClose}
                />
                <NavigationVerticalMenuItem
                  to='/logout'
                  exact
                  title='Log out'
                  icon='log out'
                  onClick={onClose}
                />
              </Fragment>
            : <NavigationVerticalMenuItem
                to='/auth'
                exact
                title='Log in'
                icon='user outline'
                onClick={onClose}
              />
          }
        </Menu>
      </div>
    </Fragment>
  );
};

export default SideDrawer;
