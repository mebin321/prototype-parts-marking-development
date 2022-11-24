import React from 'react';
import { Accordion, Icon } from 'semantic-ui-react';

import { INavigationItemDefinition } from '../navigationItemDefinition';
import NavigationVerticalMenuItem from './NavigationVerticalMenuItem';

const createNavigationAccordionItem = (
  {
    visible,
    to,
    exact,
    items,
    title,
    icon,
  }: INavigationItemDefinition,
  onClick?: () => void
) =>
{
  if (visible === false) return null;

  if (!items)
  {
    return (
      <NavigationVerticalMenuItem
        key={title}
        to={to}
        exact={exact}
        title={title}
        icon={icon}
        onClick={onClick}
      />
    );
  }

  return {
    key: title,
    title: { content: <div style={{ display: 'inline' }}><Icon name={icon} />{title}</div> },
    content:
    {
      content:
        <Accordion.Accordion
          style={{ marginTop: '0', paddingLeft: '1.5em' }}
          panels={items.map(item => createNavigationAccordionItem(item, onClick))}
        />,
      style: { paddingTop: '0', paddingBottom: '0' },
    },
  };
};

export default createNavigationAccordionItem;
