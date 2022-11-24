import React from 'react';
import { Icon, Menu, Popup } from 'semantic-ui-react';

import { IUserData } from '../../../models/api/users/userData';
import { ISearchFilters } from '../../../store/search/types';
import ItemSearchBox from '../../search/ItemSearchBox';
import NavigationHorizontalMenuItem from '../NavigationHorizontalMenuItem';
import { INavigationItemDefinition } from '../navigationItemDefinition';
import NavigationDropdownItem from './NavigationDropdownItem';
import UserToolbarMenu from './UserToolbarMenu';

interface IToolbarProps
{
  isMobileView: boolean;
  items: INavigationItemDefinition[];
  user: IUserData;
  toggleSideDrawer: () => void;
  onSearch: (filters: ISearchFilters) => void;
}

const Toolbar: React.FC<IToolbarProps> = ({
  isMobileView,
  items,
  user,
  toggleSideDrawer,
  onSearch,
}) =>
{
  const searchBox = (
    <ItemSearchBox
      style={{ margin: 'auto auto auto 1em' }}
      onSearch={onSearch}
    />
  );

  if (isMobileView)
  {
    return (
      <Menu style={{ minWidth: 'fit-content', width: '100vw' }}>
        <Menu.Item icon='bars' onClick={toggleSideDrawer} />
        {searchBox}
      </Menu>
    );
  }

  return (
    <Menu>
      {
        items.map(item =>
        {
          return item.items
            ? <NavigationDropdownItem key={item.title} {...item} />
            : <NavigationHorizontalMenuItem key={item.title} fitted='vertically' {...item} />;
        })
      }
      {searchBox}
      <Menu.Menu position='right'>
        <NavigationHorizontalMenuItem
          fitted='vertically'
          to='/admin'
          title='Settings'
          icon='setting'
          iconSize='large'
        />
        {user?.username
          ? <Popup
              basic
              flowing
              pinned
              positionFixed
              position='bottom right'
              on='click'
              header={user.name}
              content={<UserToolbarMenu user={user} />}
              style={{ margin: '2px 0' }}
              trigger={
                <Menu.Item fitted='vertically' title={user.name}>
                  <Icon name='user' size='large' fitted />
                </Menu.Item>
              }
            />
          : <NavigationHorizontalMenuItem
              fitted='vertically'
              to='/auth'
              title='Log In'
              icon='user outline'
              iconSize='large'
            />
        }
      </Menu.Menu>
    </Menu>
  );
};

export default Toolbar;
