import React from 'react';
import { Menu } from 'semantic-ui-react';

import { IUserData } from '../../../models/api/users/userData';
import NavigationHorizontalMenuItem from '../NavigationHorizontalMenuItem';

interface IUserToolbarMenuProps
{
  user: IUserData;
}

const UserToolbarMenu: React.FC<IUserToolbarMenuProps> = ({
  user,
}) =>
{
  return (
    <Menu vertical secondary fluid style={{ margin: '1em 0 0 0' }}>
      <NavigationHorizontalMenuItem
        to={`/admin/users/${user.id}`}
        title='View profile'
        description='View &amp; edit your profile'
      />
      <NavigationHorizontalMenuItem
        to='/logout'
        title='Log out'
        description='End this session and allow to log in as different user'
      />
    </Menu>
  );
};

export default UserToolbarMenu;
