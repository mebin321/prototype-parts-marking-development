import _ from 'lodash';

import { ISortParameters, NoSort } from '../models/api/sort/sortParameters';
import { isValueEmpty, parseLocalStorageJSON } from '../utilities/objects';

export function updateObject<T extends object>(oldObject: T, updatedValues: Partial<T>)
{
  return {
    ...oldObject,
    ...updatedValues,
  };
}

export function updateObjectProperty<T extends object, K extends keyof T>(
  oldObject: T,
  property: K,
  updatedValues: Partial<T[K]>)
{
  return {
    ...oldObject,
    [property]:
      {
        ...oldObject[property],
        ...updatedValues,
      },
  };
}

export function readItemsFilterFromLocalStorage<TFilter extends object>(
  filterKey: string,
  sortKey: string,
  defaultFilter: Partial<TFilter> = {})
{
  const filter = parseLocalStorageJSON<Partial<TFilter>>(filterKey, defaultFilter);
  const sort = parseLocalStorageJSON<ISortParameters>(sortKey, NoSort);

  const isFilterDefined = !isValueEmpty(filter) && !_.isEqual(filter, defaultFilter);
  const isSortDefined = !isValueEmpty(sort) && !!sort?.sortBy;

  return {
    filter: filter,
    sort: sort,
    visible: isFilterDefined || isSortDefined,
    loading: false,
  };
}
