import { Action } from 'redux';

import { IPackageFilter } from '../../models/api/items/package/packageFilter';
import { IPrototypeFilter } from '../../models/api/items/part/prototypeFilter';
import { IPrototypeSetFilter } from '../../models/api/items/part/set/prototypeSetFilter';
import { IPackagesResponse, IPrototypeSetsResponse, IPrototypesExtendedResponse } from '../../models/api/responses';
import { ISortParameters } from '../../models/api/sort/sortParameters';

export interface IFilterSubstate<TFilter, TResults>
{
  visible: boolean;
  filter?: TFilter;
  sort?: ISortParameters;
  results?: TResults;
  error?: string;
  loading: boolean;
}

export interface IFilteringState
{
  packages: IFilterSubstate<IPackageFilter, IPackagesResponse>;
  prototypeSets: IFilterSubstate<IPrototypeSetFilter, IPrototypeSetsResponse>;
  prototypes: IFilterSubstate<IPrototypeFilter, IPrototypesExtendedResponse>;
  components: IFilterSubstate<IPrototypeFilter, IPrototypesExtendedResponse>;
}

export const SET_PACKAGES_FILTER_VISIBILITY = 'SET_PACKAGES_FILTER_VISIBILITY';
export const FILTER_PACKAGES_START = 'FILTER_PACKAGES_START';
export const FILTER_PACKAGES_SUCCESS = 'FILTER_PACKAGES_SUCCESS';
export const FILTER_PACKAGES_FAILURE = 'FILTER_PACKAGES_FAILURE';

export const SET_PROTOTYPE_SETS_FILTER_VISIBILITY = 'SET_PROTOTYPE_SETS_FILTER_VISIBILITY';
export const FILTER_PROTOTYPE_SETS_START = 'FILTER_PROTOTYPE_SETS_START';
export const FILTER_PROTOTYPE_SETS_SUCCESS = 'FILTER_PROTOTYPE_SETS_SUCCESS';
export const FILTER_PROTOTYPE_SETS_FAILURE = 'FILTER_PROTOTYPE_SETS_FAILURE';

export const SET_PROTOTYPES_FILTER_VISIBILITY = 'SET_PROTOTYPES_FILTER_VISIBILITY';
export const FILTER_PROTOTYPES_START = 'FILTER_PROTOTYPES_START';
export const FILTER_PROTOTYPES_SUCCESS = 'FILTER_PROTOTYPES_SUCCESS';
export const FILTER_PROTOTYPES_FAILURE = 'FILTER_PROTOTYPES_FAILURE';

export const SET_COMPONENTS_FILTER_VISIBILITY = 'SET_COMPONENTS_FILTER_VISIBILITY';
export const FILTER_COMPONENTS_START = 'FILTER_COMPONENTS_START';
export const FILTER_COMPONENTS_SUCCESS = 'FILTER_COMPONENTS_SUCCESS';
export const FILTER_COMPONENTS_FAILURE = 'FILTER_COMPONENTS_FAILURE';

// #region Packages
export interface ISetPackagesFilterVisibilityAction extends Action<typeof SET_PACKAGES_FILTER_VISIBILITY>
{
  visible: boolean;
}

export interface IFilterPackagesStartAction extends Action<typeof FILTER_PACKAGES_START>
{
  filter: IPackageFilter | undefined;
  sort?: ISortParameters;
}

export interface IFilterPackagesSuccessAction extends Action<typeof FILTER_PACKAGES_SUCCESS>
{
  data: IPackagesResponse;
}

export interface IFilterPackagesFailureAction extends Action<typeof FILTER_PACKAGES_FAILURE>
{
  error: string;
}
// #endregion Packages

// #region Prototype Sets
export interface ISetPrototypeSetsFilterVisibilityAction extends Action<typeof SET_PROTOTYPE_SETS_FILTER_VISIBILITY>
{
  visible: boolean;
}

export interface IFilterPrototypeSetsStartAction extends Action<typeof FILTER_PROTOTYPE_SETS_START>
{
  filter: IPrototypeSetFilter | undefined;
  sort?: ISortParameters;
}

export interface IFilterPrototypeSetsSuccessAction extends Action<typeof FILTER_PROTOTYPE_SETS_SUCCESS>
{
  data: IPrototypeSetsResponse;
}

export interface IFilterPrototypeSetsFailureAction extends Action<typeof FILTER_PROTOTYPE_SETS_FAILURE>
{
  error: string;
}
// #endregion Prototype Sets

// #region Prototypes
export interface ISetPrototypesFilterVisibilityAction extends Action<typeof SET_PROTOTYPES_FILTER_VISIBILITY>
{
  visible: boolean;
}

export interface IFilterPrototypesStartAction extends Action<typeof FILTER_PROTOTYPES_START>
{
  filter: IPrototypeFilter | undefined;
  sort?: ISortParameters;
}

export interface IFilterPrototypesSuccessAction extends Action<typeof FILTER_PROTOTYPES_SUCCESS>
{
  data: IPrototypesExtendedResponse;
}

export interface IFilterPrototypesFailureAction extends Action<typeof FILTER_PROTOTYPES_FAILURE>
{
  error: string;
}
// #endregion Prototypes

// #region Components
export interface ISetComponentsFilterVisibilityAction extends Action<typeof SET_COMPONENTS_FILTER_VISIBILITY>
{
  visible: boolean;
}

export interface IFilterComponentsStartAction extends Action<typeof FILTER_COMPONENTS_START>
{
  filter: IPrototypeFilter | undefined;
  sort?: ISortParameters;
}

export interface IFilterComponentsSuccessAction extends Action<typeof FILTER_COMPONENTS_SUCCESS>
{
  data: IPrototypesExtendedResponse;
}

export interface IFilterComponentsFailureAction extends Action<typeof FILTER_COMPONENTS_FAILURE>
{
  error: string;
}
// #endregion Components

export type FilteringActionTypes =
  ISetPackagesFilterVisibilityAction | IFilterPackagesStartAction | IFilterPackagesSuccessAction
  | IFilterPackagesFailureAction | ISetPrototypeSetsFilterVisibilityAction | IFilterPrototypeSetsStartAction
  | IFilterPrototypeSetsSuccessAction | IFilterPrototypeSetsFailureAction | ISetPrototypesFilterVisibilityAction
  | IFilterPrototypesStartAction | IFilterPrototypesSuccessAction | IFilterPrototypesFailureAction
  | ISetComponentsFilterVisibilityAction | IFilterComponentsStartAction | IFilterComponentsSuccessAction
  | IFilterComponentsFailureAction;
