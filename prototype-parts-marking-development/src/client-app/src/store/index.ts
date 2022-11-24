import { Reducer, combineReducers } from 'redux';

import { IPackageFilter } from '../models/api/items/package/packageFilter';
import { ItemType } from '../models/api/items/part/itemType';
import { IPrototypeFilter } from '../models/api/items/part/prototypeFilter';
import { IPrototypeSetFilter } from '../models/api/items/part/set/prototypeSetFilter';
import { parseLocalStorageJSON } from '../utilities/objects';
import authReducer from './auth/reducer';
import { AuthActionTypes, IAuthState } from './auth/types';
import
{
  DefaultPackagesTableSettings,
  DefaultPrototypeSetsTableSettings,
  DefaultPrototypesTableSettings,
  DefaultVariantsTableSettings,
} from './configuration/defaults';
import configurationReducer from './configuration/reducer';
import { ConfigurationActionTypes, IConfigurationState, ITableDisplaySettings } from './configuration/types';
import filteringReducer from './filtering/reducer';
import { FilteringActionTypes, IFilteringState } from './filtering/types';
import searchReducer from './search/reducer';
import { ISearchState, SearchActionTypes } from './search/types';
import { readItemsFilterFromLocalStorage } from './utilities';

export const AuthDataLocalStorageKey = 'auth';
export const UserRolesLocalStorageKey = 'roles';
export const PackagesFilterLocalStorageKey = 'packages.filter';
export const PackagesSortLocalStorageKey = 'packages.sort';
export const PrototypeSetsFilterLocalStorageKey = 'prototypeSets.filter';
export const PrototypeSetsSortLocalStorageKey = 'prototypeSets.sort';
export const PrototypesFilterLocalStorageKey = 'prototypes.filter';
export const PrototypesSortLocalStorageKey = 'prototypes.sort';
export const ComponentsFilterLocalStorageKey = 'components.filter';
export const ComponentsSortLocalStorageKey = 'components.sort';

export interface IApplicationState
{
  configuration: IConfigurationState;
  auth: IAuthState;
  filtering: IFilteringState;
  search: ISearchState;
}

export type ApplicationActionTypes = ConfigurationActionTypes | AuthActionTypes | SearchActionTypes
  | FilteringActionTypes;

export const reducers: Reducer<IApplicationState, ApplicationActionTypes> = combineReducers<IApplicationState>({
  configuration: configurationReducer,
  auth: authReducer,
  filtering: filteringReducer,
  search: searchReducer,
});

export function preloadStateFromLocalStorage(): any
{
  return {
    configuration:
    {
      packages:
      {
        tableSettings: parseLocalStorageJSON<ITableDisplaySettings>(
          'configuration.packages.tableSettings', DefaultPackagesTableSettings),
        itemOptions: {},
      },
      prototypeSets:
      {
        tableSettings: parseLocalStorageJSON<ITableDisplaySettings>(
          'configuration.prototypeSets.tableSettings', DefaultPrototypeSetsTableSettings),
        itemOptions: {},
      },
      prototypes:
      {
        tableSettings: parseLocalStorageJSON<ITableDisplaySettings>(
          'configuration.prototypes.tableSettings', DefaultPrototypesTableSettings),
        itemOptions: {},
      },
      variants:
      {
        tableSettings: parseLocalStorageJSON<ITableDisplaySettings>(
          'configuration.variants.tableSettings', DefaultVariantsTableSettings),
        itemOptions: {},
      },
    },
    filtering:
    {
      packages: readItemsFilterFromLocalStorage<IPackageFilter>(
        PackagesFilterLocalStorageKey, PackagesSortLocalStorageKey),
      prototypeSets: readItemsFilterFromLocalStorage<IPrototypeSetFilter>(
        PrototypeSetsFilterLocalStorageKey, PrototypeSetsSortLocalStorageKey),
      prototypes: readItemsFilterFromLocalStorage<IPrototypeFilter>(
        PrototypesFilterLocalStorageKey, PrototypesSortLocalStorageKey, { type: ItemType.Prototype }),
      components: readItemsFilterFromLocalStorage<IPrototypeFilter>(
        ComponentsFilterLocalStorageKey, ComponentsSortLocalStorageKey, { type: ItemType.Component }),
    },
  };
}

export
{
  loadConfiguration,
  setTableSettings,
} from './configuration/actions';

export
{
  initializeAuthFromLocalStorage,
  loadAuthConfiguration,
  authSuccess,
  login,
  logout,
} from './auth/actions';

export
{
  filterPackages,
  filterPrototypeSets,
  filterPrototypes,
  filterComponents,
} from './filtering/actions';

export
{
  search,
  searchPackages,
  searchPrototypes,
  searchVariants,
  foldResultsTarget,
  unfoldResultsTarget,
} from './search/actions';
