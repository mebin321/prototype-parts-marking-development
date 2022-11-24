import React from 'react';
import { Dropdown } from 'semantic-ui-react';

import NavigationItem, { INavigationItemContentProps } from '../NavigationItem';
import { INavigationItemDefinition } from '../navigationItemDefinition';

interface INavigationDropdownItemProps extends INavigationItemDefinition
{
  secondary?: boolean;
}

const NavigationDropdownItem: React.FC<INavigationDropdownItemProps> = ({
  visible,
  to,
  exact,
  items,
  title,
  icon,
  secondary,
}) =>
{
  if (visible === false) return null;

  const DropdownComponent: React.FC<INavigationItemContentProps> = ({
    active,
    history,
  }) =>
  {
    const itemClickHandler = to ? () => history.push(to) : undefined;

    return (
      <Dropdown.Item
        text={title}
        icon={icon}
        active={active}
        onClick={itemClickHandler}
      />
    );
  };

  if (items)
  {
    const dropdownIcon = secondary ? 'caret right' : undefined;
    return (
      <Dropdown item text={title} icon={dropdownIcon}>
        <Dropdown.Menu>
          {items.map(item => <NavigationDropdownItem secondary key={item.title} {...item} />)}
        </Dropdown.Menu>
      </Dropdown>
    );
  }

  return <NavigationItem to={to} exact={exact} component={DropdownComponent} />;
};

export default NavigationDropdownItem;
