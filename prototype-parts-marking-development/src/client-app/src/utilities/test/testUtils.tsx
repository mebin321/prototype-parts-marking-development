import { History, createMemoryHistory } from 'history';
import React, { ComponentType } from 'react';
import { Route, RouteComponentProps, Router } from 'react-router-dom';

/** Options for creating React Router wrapper via Route to contain match object. */
export interface IRouterWrapperOptions
{
  /** The relative URL of the current site (found in match.url) e.g. /admin/users/1. */
  url?: string;
  /** The route path possibly containing parameters (found in match.path) e.g. /admin/users/:id. */
  path?: string;
  /** History object containing list of visited URLs and allowing to navigate to URLs from history or any URL. */
  history?: History;
}

/**
 * Wrap a component into <Router> and provide also match object.
 * This is achieved by rendering component via <Route>.
 * Hint: when history object is passed to this function, then it is possible to mock history functions e.g. push.
 *
 * @param WrappedComponent component to wrap into <Router>
 * @param url the relative URL of the current site (found in match.url) e.g. /admin/users/1
 * @param path the route path possibly containing parameters (found in match.path) e.g. /admin/users/:id
 * @param history object containing list of visited URLs and allowing to navigate to URLs from history or any URL
 *
 * @param T wrapped component props
 *
 * @returns new component which wraps the provided component into <Router>
 */
export const withRouterWrapper = <T, >(
  WrappedComponent: ComponentType<T>,
  {
    url = '/',
    path = url,
    history = createMemoryHistory({ initialEntries: [url] }),
  }: IRouterWrapperOptions = {}
) =>
{
  const Component: React.FC<T> = props =>
  {
    const renderWrappedComponent = (routerProps: RouteComponentProps<any>) =>
      <WrappedComponent {...routerProps} {...props}/>;
    return (
      <Router history={history}>
        <Route path={path} render={renderWrappedComponent} />
      </Router>
    );
  };

  Component.displayName = 'WithRouterWrapper';
  return Component;
};
