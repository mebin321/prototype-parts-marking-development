import React from 'react';
import { Icon, Menu, MenuItemProps } from 'semantic-ui-react';

import NavigationItem, { INavigationItemContentProps } from './NavigationItem';
import { INavigationItemDefinition } from './navigationItemDefinition';

interface INavigationHorizontalMenuItemProps extends INavigationItemDefinition, Omit<MenuItemProps, 'icon'>
{
}

const NavigationHorizontalMenuItem: React.FC<INavigationHorizontalMenuItemProps> = ({
  visible,
  to,
  exact,
  title,
  description,
  icon,
  iconSize,
  items: _items,
  ...props
}) =>
{
  if (visible === false) return null;

  const MenuItemComponent: React.FC<INavigationItemContentProps> = ({
    active,
    history,
  }) =>
  {
    let tooltip = description;
    if (icon && !tooltip)
    {
      tooltip = title;
    }

    return (
      <Menu.Item
        title={tooltip}
        active={active}
        onClick={() => { if (to) history.push(to); }}
        {...props}
      >
        {icon
          ? <Icon fitted name={icon} size={iconSize} />
          : title}
      </Menu.Item>
    );
  };

  return <NavigationItem to={to} exact={exact} component={MenuItemComponent} />;
};

export default NavigationHorizontalMenuItem;
