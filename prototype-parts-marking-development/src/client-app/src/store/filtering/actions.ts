import { Dispatch } from 'redux';

import
{
  ComponentsFilterLocalStorageKey,
  ComponentsSortLocalStorageKey,
  PackagesFilterLocalStorageKey,
  PackagesSortLocalStorageKey,
  PrototypeSetsFilterLocalStorageKey,
  PrototypeSetsSortLocalStorageKey,
  PrototypesFilterLocalStorageKey,
  PrototypesSortLocalStorageKey,
} from '..';
import agent from '../../api/agent';
import { extractErrorDetails } from '../../api/utilities';
import { IPackageFilter } from '../../models/api/items/package/packageFilter';
import { IPrototypeFilter } from '../../models/api/items/part/prototypeFilter';
import { IPrototypeSetFilter } from '../../models/api/items/part/set/prototypeSetFilter';
import { IPackagesResponse, IPrototypeSetsResponse, IPrototypesExtendedResponse } from '../../models/api/responses';
import { ISortParameters } from '../../models/api/sort/sortParameters';
import
{
  FILTER_COMPONENTS_FAILURE,
  FILTER_COMPONENTS_START,
  FILTER_COMPONENTS_SUCCESS,
  FILTER_PACKAGES_FAILURE,
  FILTER_PACKAGES_START,
  FILTER_PACKAGES_SUCCESS,
  FILTER_PROTOTYPES_FAILURE,
  FILTER_PROTOTYPES_START,
  FILTER_PROTOTYPES_SUCCESS,
  FILTER_PROTOTYPE_SETS_FAILURE,
  FILTER_PROTOTYPE_SETS_START,
  FILTER_PROTOTYPE_SETS_SUCCESS,
  IFilterComponentsFailureAction,
  IFilterComponentsStartAction,
  IFilterComponentsSuccessAction,
  IFilterPackagesFailureAction,
  IFilterPackagesStartAction,
  IFilterPackagesSuccessAction,
  IFilterPrototypeSetsFailureAction,
  IFilterPrototypeSetsStartAction,
  IFilterPrototypeSetsSuccessAction,
  IFilterPrototypesFailureAction,
  IFilterPrototypesStartAction,
  IFilterPrototypesSuccessAction,
  ISetComponentsFilterVisibilityAction,
  ISetPackagesFilterVisibilityAction,
  ISetPrototypeSetsFilterVisibilityAction,
  ISetPrototypesFilterVisibilityAction,
  SET_COMPONENTS_FILTER_VISIBILITY,
  SET_PACKAGES_FILTER_VISIBILITY,
  SET_PROTOTYPES_FILTER_VISIBILITY,
  SET_PROTOTYPE_SETS_FILTER_VISIBILITY,
} from './types';

// #region Packages
export function setPackagesFilterVisibility(visible: boolean): ISetPackagesFilterVisibilityAction
{
  return {
    type: SET_PACKAGES_FILTER_VISIBILITY,
    visible: visible,
  };
}

function filterPackagesStart(
  filter: IPackageFilter | undefined,
  sort?: ISortParameters
): IFilterPackagesStartAction
{
  localStorage.setItem(PackagesFilterLocalStorageKey, filter ? JSON.stringify(filter) : '');
  localStorage.setItem(PackagesSortLocalStorageKey, sort ? JSON.stringify(sort) : '');

  return {
    type: FILTER_PACKAGES_START,
    filter: filter,
    sort: sort,
  };
}

function filterPackagesSuccess(data: IPackagesResponse): IFilterPackagesSuccessAction
{
  return {
    type: FILTER_PACKAGES_SUCCESS,
    data: data,
  };
}

function filterPackagesFailure(error: string): IFilterPackagesFailureAction
{
  return {
    type: FILTER_PACKAGES_FAILURE,
    error: error,
  };
}

export function filterPackages(
  filter: IPackageFilter | undefined,
  sort?: ISortParameters,
  page = 1,
  pageSize = -1
)
{
  return async (dispatch: Dispatch) =>
  {
    dispatch(filterPackagesStart(filter, sort));
    try
    {
      const results = await agent.Packages.list(filter, sort, page, pageSize);
      dispatch(filterPackagesSuccess(results));
    }
    catch (error)
    {
      dispatch(filterPackagesFailure(extractErrorDetails(error)));
    }
  };
}
// #endregion Packages

// #region Prototype Sets
export function setPrototypeSetsFilterVisibility(visible: boolean): ISetPrototypeSetsFilterVisibilityAction
{
  return {
    type: SET_PROTOTYPE_SETS_FILTER_VISIBILITY,
    visible: visible,
  };
}

function filterPrototypeSetsStart(
  filter: IPrototypeSetFilter | undefined,
  sort?: ISortParameters
): IFilterPrototypeSetsStartAction
{
  localStorage.setItem(PrototypeSetsFilterLocalStorageKey, filter ? JSON.stringify(filter) : '');
  localStorage.setItem(PrototypeSetsSortLocalStorageKey, sort ? JSON.stringify(sort) : '');

  return {
    type: FILTER_PROTOTYPE_SETS_START,
    filter: filter,
    sort: sort,
  };
}

function filterPrototypeSetsSuccess(data: IPrototypeSetsResponse): IFilterPrototypeSetsSuccessAction
{
  return {
    type: FILTER_PROTOTYPE_SETS_SUCCESS,
    data: data,
  };
}

function filterPrototypeSetsFailure(error: string): IFilterPrototypeSetsFailureAction
{
  return {
    type: FILTER_PROTOTYPE_SETS_FAILURE,
    error: error,
  };
}

export function filterPrototypeSets(
  filter: IPrototypeSetFilter | undefined,
  sort?: ISortParameters,
  page = 1,
  pageSize = -1
)
{
  return async (dispatch: Dispatch) =>
  {
    dispatch(filterPrototypeSetsStart(filter, sort));
    try
    {
      const results = await agent.PrototypeSets.list(filter, sort, page, pageSize);
      dispatch(filterPrototypeSetsSuccess(results));
    }
    catch (error)
    {
      dispatch(filterPrototypeSetsFailure(extractErrorDetails(error)));
    }
  };
}
// #endregion Prototype Sets

// #region Prototypes
export function setPrototypesFilterVisibility(visible: boolean): ISetPrototypesFilterVisibilityAction
{
  return {
    type: SET_PROTOTYPES_FILTER_VISIBILITY,
    visible: visible,
  };
}

function filterPrototypesStart(
  filter: IPrototypeFilter | undefined,
  sort?: ISortParameters
): IFilterPrototypesStartAction
{
  localStorage.setItem(PrototypesFilterLocalStorageKey, filter ? JSON.stringify(filter) : '');
  localStorage.setItem(PrototypesSortLocalStorageKey, sort ? JSON.stringify(sort) : '');

  return {
    type: FILTER_PROTOTYPES_START,
    filter: filter,
    sort: sort,
  };
}

function filterPrototypesSuccess(data: IPrototypesExtendedResponse): IFilterPrototypesSuccessAction
{
  return {
    type: FILTER_PROTOTYPES_SUCCESS,
    data: data,
  };
}

function filterPrototypesFailure(error: string): IFilterPrototypesFailureAction
{
  return {
    type: FILTER_PROTOTYPES_FAILURE,
    error: error,
  };
}

export function filterPrototypes(
  filter: IPrototypeFilter | undefined,
  sort?: ISortParameters,
  page = 1,
  pageSize = -1
)
{
  return async (dispatch: Dispatch) =>
  {
    dispatch(filterPrototypesStart(filter, sort));
    try
    {
      const results = await agent.Prototypes.list(filter, sort, page, pageSize);
      dispatch(filterPrototypesSuccess(results));
    }
    catch (error)
    {
      dispatch(filterPrototypesFailure(extractErrorDetails(error)));
    }
  };
}
// #endregion Prototypes

// #region Components
export function setComponentsFilterVisibility(visible: boolean): ISetComponentsFilterVisibilityAction
{
  return {
    type: SET_COMPONENTS_FILTER_VISIBILITY,
    visible: visible,
  };
}

function filterComponentsStart(
  filter: IPrototypeFilter | undefined,
  sort?: ISortParameters
): IFilterComponentsStartAction
{
  localStorage.setItem(ComponentsFilterLocalStorageKey, filter ? JSON.stringify(filter) : '');
  localStorage.setItem(ComponentsSortLocalStorageKey, sort ? JSON.stringify(sort) : '');

  return {
    type: FILTER_COMPONENTS_START,
    filter: filter,
    sort: sort,
  };
}

function filterComponentsSuccess(data: IPrototypesExtendedResponse): IFilterComponentsSuccessAction
{
  return {
    type: FILTER_COMPONENTS_SUCCESS,
    data: data,
  };
}

function filterComponentsFailure(error: string): IFilterComponentsFailureAction
{
  return {
    type: FILTER_COMPONENTS_FAILURE,
    error: error,
  };
}

export function filterComponents(
  filter: IPrototypeFilter | undefined,
  sort?: ISortParameters,
  page = 1,
  pageSize = -1
)
{
  return async (dispatch: Dispatch) =>
  {
    dispatch(filterComponentsStart(filter, sort));
    try
    {
      const results = await agent.Prototypes.list(filter, sort, page, pageSize);
      dispatch(filterComponentsSuccess(results));
    }
    catch (error)
    {
      dispatch(filterComponentsFailure(extractErrorDetails(error)));
    }
  };
}
// #endregion Components
