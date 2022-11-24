import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { useClearCache } from 'react-clear-cache';
import { ConnectedProps, connect } from 'react-redux';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import { Button } from 'semantic-ui-react';

import { INavigationItemDefinition } from '../../components/navigation/navigationItemDefinition';
import SideDrawer from '../../components/navigation/SideDrawer/SideDrawer';
import Toolbar from '../../components/navigation/Toolbar/Toolbar';
import usePermissions from '../../hooks/usePermissions';
import useWindowDimensions from '../../hooks/useWindowDimensions';
import { IApplicationState, search } from '../../store';
import { selectSearchStarted } from '../../store/search/selectors';

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    user: state.auth.user,
    isSearching: selectSearchStarted(state),
  };
};

const mapDispatchToProps =
{
  searchHandler: search,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

interface ILayoutProps extends RouteComponentProps, ConnectedProps<typeof connector>
{
}

const Layout: React.FC<ILayoutProps> = ({
  history,
  user,
  isSearching,
  searchHandler,
  children,
}) =>
{
  const windowDimensions = useWindowDimensions();
  const { canCreatePrototypePackages, canCreatePrototypeSet } = usePermissions();

  const [isSideDrawerVisible, setSideDrawerVisible] = useState(false);
  const [navigationItems, setNavigationItems] = useState<INavigationItemDefinition[]>([]);

  const { isLatestVersion, emptyCacheStorage } = useClearCache();

  useEffect(() =>
  {
    if (isSearching)
    {
      history.push('/search');
    }
  }, [history, isSearching]);

  useEffect(() =>
  {
    const newNavigationItems: INavigationItemDefinition[] =
    [
      { to: '/', exact: true, title: 'Home' },
      {
        title: 'Parts',
        items:
        [
          {
            title: 'Prototype Sets',
            items:
            [
              { title: 'Create', to: '/prototype-sets/new', exact: true, visible: canCreatePrototypeSet },
              { title: 'List', to: '/prototype-sets', exact: true },
            ],
          },
          { title: 'Prototypes', to: '/prototypes', exact: true },
          { title: 'Components', to: '/components', exact: true },
        ],
      },
      {
        title: 'Packages',
        items:
        [
          { title: 'Create', to: '/packages/new', exact: true, visible: canCreatePrototypePackages },
          { title: 'List', to: '/packages', exact: true },
        ],
      },
      { to: '/print', exact: true, title: 'Print' },
    ];

    setNavigationItems(newNavigationItems);
  }, [canCreatePrototypePackages, canCreatePrototypeSet]);

  const toggleSideDrawer = useCallback(() =>
  {
    setSideDrawerVisible(prevIsSideDrawerVisible => !prevIsSideDrawerVisible);
  }, []);

  const hideSideDrawer = useCallback(() =>
  {
    setSideDrawerVisible(false);
  }, []);

  const emptyCacheAndReload = (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) =>
  {
    event.preventDefault();
    emptyCacheStorage();
    window.location.reload();
  };

  const isMobile = windowDimensions.width < 600;

  return (
    <Fragment>
      <Toolbar
        isMobileView={isMobile}
        items={navigationItems}
        user={user}
        toggleSideDrawer={toggleSideDrawer}
        onSearch={searchHandler}
      />

    {!isLatestVersion && ( // Cache buster button
      <p style={{ textAlign: 'center' }}>
        <Button
          color='red'
          onClick={event => emptyCacheAndReload(event)}
        >
          Please click here to update PPMT version
        </Button>
      </p>
    )}

      {isMobile && <SideDrawer
        visible={isSideDrawerVisible}
        items={navigationItems}
        user={user}
        onClose={hideSideDrawer}
      />}
      <main>
        <ToastContainer position='top-center' autoClose={false} closeOnClick={false} className='toast' />
        {children}
      </main>
    </Fragment>
  );
};

export default withRouter(connector(Layout));
