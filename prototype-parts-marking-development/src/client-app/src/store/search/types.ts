import { Action } from 'redux';

import { IPackageFilter } from '../../models/api/items/package/packageFilter';
import { IPrototypeFilter } from '../../models/api/items/part/prototypeFilter';
import { IPrototypeVariantFilter } from '../../models/api/items/part/variant/prototypeVariantFilter';
import
{
  IPackagesResponse,
  IPrototypeVariantsExtendedResponse,
  IPrototypesExtendedResponse,
} from '../../models/api/responses';
import { ISortParameters } from '../../models/api/sort/sortParameters';

export interface ISearchFilters
{
  packages?: IPackageFilter;
  prototypes?: IPrototypeFilter;
  variants?: IPrototypeVariantFilter;
}

export interface ISearchSubstate<TFilter, TResults>
{
  filter?: TFilter;
  sort?: ISortParameters;
  results?: TResults;
  expanded?: boolean;
  error?: string;
  loading: boolean;
}

export interface ISearchState
{
  packages: ISearchSubstate<IPackageFilter, IPackagesResponse>;
  prototypes: ISearchSubstate<IPrototypeFilter, IPrototypesExtendedResponse>;
  variants: ISearchSubstate<IPrototypeVariantFilter, IPrototypeVariantsExtendedResponse>;
}

export type SearchTarget = keyof ISearchState;

export const SEARCH_PACKAGES_CLEAR = 'SEARCH_PACKAGES_CLEAR';
export const SEARCH_PACKAGES_START = 'SEARCH_PACKAGES_START';
export const SEARCH_PACKAGES_SUCCESS = 'SEARCH_PACKAGES_SUCCESS';
export const SEARCH_PACKAGES_FAILURE = 'SEARCH_PACKAGES_FAILURE';
export const SEARCH_PROTOTYPES_CLEAR = 'SEARCH_PROTOTYPES_CLEAR';
export const SEARCH_PROTOTYPES_START = 'SEARCH_PROTOTYPES_START';
export const SEARCH_PROTOTYPES_SUCCESS = 'SEARCH_PROTOTYPES_SUCCESS';
export const SEARCH_PROTOTYPES_FAILURE = 'SEARCH_PROTOTYPES_FAILURE';
export const SEARCH_VARIANTS_CLEAR = 'SEARCH_VARIANTS_CLEAR';
export const SEARCH_VARIANTS_START = 'SEARCH_VARIANTS_START';
export const SEARCH_VARIANTS_SUCCESS = 'SEARCH_VARIANTS_SUCCESS';
export const SEARCH_VARIANTS_FAILURE = 'SEARCH_VARIANTS_FAILURE';
export const SEARCH_RESULTS_FOLD = 'SEARCH_RESULTS_FOLD';
export const SEARCH_RESULTS_UNFOLD = 'SEARCH_RESULTS_UNFOLD';

export interface ISearchPackagesClearAction extends Action<typeof SEARCH_PACKAGES_CLEAR>
{
}

export interface ISearchPackagesStartAction extends Action<typeof SEARCH_PACKAGES_START>
{
  filter: IPackageFilter;
  sort?: ISortParameters;
}

export interface ISearchPackagesSuccessAction extends Action<typeof SEARCH_PACKAGES_SUCCESS>
{
  data: IPackagesResponse;
}

export interface ISearchPackagesFailureAction extends Action<typeof SEARCH_PACKAGES_FAILURE>
{
  error: string;
}

export interface ISearchPrototypesClearAction extends Action<typeof SEARCH_PROTOTYPES_CLEAR>
{
}

export interface ISearchPrototypesStartAction extends Action<typeof SEARCH_PROTOTYPES_START>
{
  filter: IPrototypeFilter;
  sort?: ISortParameters;
}

export interface ISearchPrototypesSuccessAction extends Action<typeof SEARCH_PROTOTYPES_SUCCESS>
{
  data: IPrototypesExtendedResponse;
}

export interface ISearchPrototypesFailureAction extends Action<typeof SEARCH_PROTOTYPES_FAILURE>
{
  error: string;
}

export interface ISearchVariantsClearAction extends Action<typeof SEARCH_VARIANTS_CLEAR>
{
}

export interface ISearchVariantsStartAction extends Action<typeof SEARCH_VARIANTS_START>
{
  filter: IPrototypeVariantFilter;
  sort?: ISortParameters;
}

export interface ISearchVariantsSuccessAction extends Action<typeof SEARCH_VARIANTS_SUCCESS>
{
  data: IPrototypeVariantsExtendedResponse;
}

export interface ISearchVariantsFailureAction extends Action<typeof SEARCH_VARIANTS_FAILURE>
{
  error: string;
}

export interface ISearchResultsFoldAction extends Action<typeof SEARCH_RESULTS_FOLD>
{
  target: SearchTarget;
}

export interface ISearchResultsUnfoldAction extends Action<typeof SEARCH_RESULTS_UNFOLD>
{
  target: SearchTarget;
}

export type SearchActionTypes = ISearchPackagesClearAction | ISearchPackagesStartAction
  | ISearchPackagesSuccessAction | ISearchPackagesFailureAction | ISearchPrototypesClearAction
  | ISearchPrototypesStartAction | ISearchPrototypesSuccessAction | ISearchPrototypesFailureAction
  | ISearchVariantsClearAction | ISearchVariantsStartAction | ISearchVariantsSuccessAction
  | ISearchVariantsFailureAction | ISearchResultsFoldAction | ISearchResultsUnfoldAction;
