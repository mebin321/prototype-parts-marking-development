import React, { useEffect } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { BrowserRouter, Redirect, Route, Switch } from 'react-router-dom';

import { initializeAuthFromLocalStorage, loadAuthConfiguration, loadConfiguration } from '../store';
import AdministrationPage from './administration/AdministrationPage';
import LoginForm from './auth/LoginForm';
import LogoutPage from './auth/LogoutPage';
import HomePage from './home/Home';
import ItemSearchResults from './items/ItemSearchResults';
import NewPackageForm from './items/package/NewPackageForm';
import PackageDetails from './items/package/PackageDetails';
import PackagesList from './items/package/PackagesList';
import AddComponentsForm from './items/part/component/AddComponentsForm';
import ComponentDetails from './items/part/component/ComponentDetails';
import PartsPage from './items/part/PartsPage';
import AddPrototypesForm from './items/part/prototype/AddPrototypesForm';
import PrototypeDetails from './items/part/prototype/PrototypeDetails';
import NewPrototypeSetForm from './items/part/set/NewPrototypeSetForm';
import PrototypeSetDetails from './items/part/set/PrototypeSetDetails';
import Layout from './layout/Layout';
import PrintingPage from './print/PrintPage';

const mapDispatchToProps =
{
  initializeStore: initializeAuthFromLocalStorage,
  loadConfiguration: loadConfiguration,
  loadAuthConfiguration: loadAuthConfiguration,
};

const connector = connect(undefined, mapDispatchToProps);

interface IAppProps extends ConnectedProps<typeof connector>
{
}

const App: React.FC<IAppProps> = ({
  initializeStore,
  loadConfiguration,
  loadAuthConfiguration,
}) =>
{
  useEffect(() =>
  {
    initializeStore();
    loadConfiguration();
    loadAuthConfiguration();
  }, [initializeStore, loadConfiguration, loadAuthConfiguration]);

  return (
    <BrowserRouter>
      <div className="App">
        <Layout>
          <Switch>
            <Route path="/admin" component={AdministrationPage} />
            <Route path="/auth" exact component={LoginForm} />
            <Route path="/logout" exact component={LogoutPage} />
            <Route path="/prototype-sets/new" exact component={NewPrototypeSetForm} />
            <Route path='/prototype-sets/:prototypeSetId' exact component={PrototypeSetDetails} />
            <Route path="/prototype-sets/:prototypeSetId/prototypes/new" exact component={AddPrototypesForm} />
            <Route
              path="/prototype-sets/:prototypeSetId/prototypes/:prototypeId/components/new"
              exact
              component={AddComponentsForm}
            />
            <Route path="/prototype-sets/:prototypeSetId/prototypes/:prototypeId" exact component={PrototypeDetails} />
            <Route path="/prototype-sets/:prototypeSetId/components/:componentId" exact component={ComponentDetails} />
            <Route path="/prototype-sets" exact component={PartsPage} />
            <Route path="/prototypes" exact component={PartsPage} />
            <Route path="/components" exact component={PartsPage} />
            <Route path="/packages/new" exact component={NewPackageForm} />
            <Route path="/packages/:packageId" exact component={PackageDetails} />
            <Route path="/packages" exact component={PackagesList} />
            <Route path="/search" exact component={ItemSearchResults} />
            <Route path="/print" exact component={PrintingPage} />
            <Route path="/" exact component={HomePage}/>
            <Redirect to="/" />
          </Switch>
        </Layout>
      </div>
    </BrowserRouter>
  );
};

export default connector(App);
