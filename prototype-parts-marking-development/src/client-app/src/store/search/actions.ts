import { Dispatch } from 'redux';

import agent from '../../api/agent';
import { extractErrorDetails } from '../../api/utilities';
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
import
{
  ISearchFilters,
  ISearchPackagesClearAction,
  ISearchPackagesFailureAction,
  ISearchPackagesStartAction,
  ISearchPackagesSuccessAction,
  ISearchPrototypesClearAction,
  ISearchPrototypesFailureAction,
  ISearchPrototypesStartAction,
  ISearchPrototypesSuccessAction,
  ISearchResultsFoldAction,
  ISearchResultsUnfoldAction,
  ISearchVariantsClearAction,
  ISearchVariantsFailureAction,
  ISearchVariantsStartAction,
  ISearchVariantsSuccessAction,
  SEARCH_PACKAGES_CLEAR,
  SEARCH_PACKAGES_FAILURE,
  SEARCH_PACKAGES_START,
  SEARCH_PACKAGES_SUCCESS,
  SEARCH_PROTOTYPES_CLEAR,
  SEARCH_PROTOTYPES_FAILURE,
  SEARCH_PROTOTYPES_START,
  SEARCH_PROTOTYPES_SUCCESS,
  SEARCH_RESULTS_FOLD,
  SEARCH_RESULTS_UNFOLD,
  SEARCH_VARIANTS_CLEAR,
  SEARCH_VARIANTS_FAILURE,
  SEARCH_VARIANTS_START,
  SEARCH_VARIANTS_SUCCESS,
  SearchTarget,
} from './types';

function searchPackagesClearResults(): ISearchPackagesClearAction
{
  return {
    type: SEARCH_PACKAGES_CLEAR,
  };
}

function searchPackagesStart(filter: IPackageFilter, sort?: ISortParameters): ISearchPackagesStartAction
{
  return {
    type: SEARCH_PACKAGES_START,
    filter: filter,
    sort: sort,
  };
}

function searchPackagesSuccess(data: IPackagesResponse): ISearchPackagesSuccessAction
{
  return {
    type: SEARCH_PACKAGES_SUCCESS,
    data: data,
  };
}

function searchPackagesFailure(error: string): ISearchPackagesFailureAction
{
  return {
    type: SEARCH_PACKAGES_FAILURE,
    error: error,
  };
}

function searchPrototypesClearResults(): ISearchPrototypesClearAction
{
  return {
    type: SEARCH_PROTOTYPES_CLEAR,
  };
}

function searchPrototypesStart(filter: IPrototypeFilter, sort?: ISortParameters): ISearchPrototypesStartAction
{
  return {
    type: SEARCH_PROTOTYPES_START,
    filter: filter,
    sort: sort,
  };
}

function searchPrototypesSuccess(data: IPrototypesExtendedResponse): ISearchPrototypesSuccessAction
{
  return {
    type: SEARCH_PROTOTYPES_SUCCESS,
    data: data,
  };
}

function searchPrototypesFailure(error: string): ISearchPrototypesFailureAction
{
  return {
    type: SEARCH_PROTOTYPES_FAILURE,
    error: error,
  };
}

function searchVariantsClearResults(): ISearchVariantsClearAction
{
  return {
    type: SEARCH_VARIANTS_CLEAR,
  };
}

function searchVariantsStart(filter: IPrototypeVariantFilter, sort?: ISortParameters): ISearchVariantsStartAction
{
  return {
    type: SEARCH_VARIANTS_START,
    filter: filter,
    sort: sort,
  };
}

function searchVariantsSuccess(data: IPrototypeVariantsExtendedResponse): ISearchVariantsSuccessAction
{
  return {
    type: SEARCH_VARIANTS_SUCCESS,
    data: data,
  };
}

function searchVariantsFailure(error: string): ISearchVariantsFailureAction
{
  return {
    type: SEARCH_VARIANTS_FAILURE,
    error: error,
  };
}

export function search(filters: ISearchFilters)
{
  return async (dispatch: Dispatch) =>
  {
    dispatch(searchPackagesClearResults());
    dispatch(searchPrototypesClearResults());
    dispatch(searchVariantsClearResults());

    if (filters.packages)
    {
      searchPackages(filters.packages)(dispatch);
    }

    if (filters.prototypes)
    {
      searchPrototypes(filters.prototypes)(dispatch);
    }

    if (filters.variants)
    {
      searchVariants(filters.variants)(dispatch);
    }
  };
}

export function searchPackages(filter: IPackageFilter, sort?: ISortParameters, page?: number, pageSize?: number)
{
  return async (dispatch: Dispatch) =>
  {
    dispatch(searchPackagesStart(filter, sort));
    try
    {
      const results = await agent.Packages.list(filter, sort, page, pageSize);
      dispatch(searchPackagesSuccess(results));
    }
    catch (error)
    {
      let errorMessage = 'Failed to search packages ';
      errorMessage += page ? `(page #${page}) ` : '';
      errorMessage += extractErrorDetails(error);
      dispatch(searchPackagesFailure(errorMessage));
    }
  };
}

export function searchPrototypes(filter: IPrototypeFilter, sort?: ISortParameters, page?: number, pageSize?: number)
{
  return async (dispatch: Dispatch) =>
  {
    dispatch(searchPrototypesStart(filter, sort));
    try
    {
      const results = await agent.Prototypes.list(filter, sort, page, pageSize);
      dispatch(searchPrototypesSuccess(results));
    }
    catch (error)
    {
      let errorMessage = 'Failed to search prototypes ';
      errorMessage += page ? `(page #${page}) ` : '';
      errorMessage += extractErrorDetails(error);
      dispatch(searchPrototypesFailure(errorMessage));
    }
  };
}

export function searchVariants(filter: IPrototypeVariantFilter, page?: number, pageSize?: number)
{
  return async (dispatch: Dispatch) =>
  {
    dispatch(searchVariantsStart(filter));
    try
    {
      const results = await agent.Variants.list(filter, page, pageSize);
      dispatch(searchVariantsSuccess(results));
    }
    catch (error)
    {
      let errorMessage = 'Failed to search variants ';
      errorMessage += page ? `(page #${page}) ` : '';
      errorMessage += extractErrorDetails(error);
      dispatch(searchVariantsFailure(errorMessage));
    }
  };
}

export function foldResultsTarget(target: SearchTarget): ISearchResultsFoldAction
{
  return {
    type: SEARCH_RESULTS_FOLD,
    target: target,
  };
}

export function unfoldResultsTarget(target: SearchTarget): ISearchResultsUnfoldAction
{
  return {
    type: SEARCH_RESULTS_UNFOLD,
    target: target,
  };
}
