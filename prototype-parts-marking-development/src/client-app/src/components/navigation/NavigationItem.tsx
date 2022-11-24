import { History } from 'history';
import React, { ReactNode } from 'react';
import { Route } from 'react-router-dom';

export interface INavigationItemRouteProps
{
  to?: string;
  exact?: boolean;
}

export interface INavigationItemContentProps
{
  active: boolean;
  history: History;
}

interface INavigationItemProps extends INavigationItemRouteProps
{
  component: (props: INavigationItemContentProps) => ReactNode;
}

const NavigationItem: React.FC<INavigationItemProps> = ({
  to,
  exact,
  component,
}) =>
{
  return (
    <Route
      path={to}
      exact={exact}
      // eslint-disable-next-line react/no-children-prop
      children={({
        match,
        history,
      }) =>
        component({ active: match !== null, history: history })
      }
    />
  );
};

export default NavigationItem;
