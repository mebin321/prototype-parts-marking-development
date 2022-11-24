import React from 'react';
import { Icon, Menu } from 'semantic-ui-react';

import NavigationItem, { INavigationItemContentProps } from '../NavigationItem';
import { INavigationItemDefinition } from '../navigationItemDefinition';

interface INavigationVerticalMenuItemProps extends INavigationItemDefinition
{
  onClick?: () => void;
}

const NavigationVerticalMenuItem: React.FC<INavigationVerticalMenuItemProps> = ({
  to,
  exact,
  title,
  icon,
  onClick,
}) =>
{
  const MenuItemComponent: React.FC<INavigationItemContentProps> = ({
    active,
    history,
  }) =>
  {
    return (
      <Menu.Item
        active={active}
        style={{ paddingTop: '0.5em', paddingBottom: '0.5em' }}
        onClick={() =>
        {
          if (onClick) onClick();
          if (to) history.push(to);
        }}
      >
        <div style={{ display: 'inline' }}><Icon name={icon} />{title}</div>
      </Menu.Item>
    );
  };

  return <NavigationItem to={to} exact={exact} component={MenuItemComponent} />;
};

export default NavigationVerticalMenuItem;
