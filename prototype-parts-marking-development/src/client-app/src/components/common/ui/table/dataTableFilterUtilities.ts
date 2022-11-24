import { IInterval } from '../../../../models/common/interval';
import { DataGetter, getPropertyValue, isValueEmpty } from '../../../../utilities/objects';
import { DropdownValue } from './dataTableFilterSpecification';

export function isFilterValueEmpty<T extends object>(filter: T | undefined, getter: DataGetter<T> | undefined)
{
  if (!filter || !getter) return true;

  const value = getPropertyValue(filter, getter);
  return isValueEmpty(value);
}

export function updateFilterSingleValueProperty<T extends object>(
  filter: T | undefined,
  property: keyof T,
  value: any
)
{
  const newFilter: any = filter ? { ...filter } : {};

  if (isValueEmpty(value))
  {
    delete newFilter[property];
  }
  else
  {
    newFilter[property] = value;
  }

  return newFilter;
}

export function updateFilterMultiValueProperty<T extends object>(
  filter: T | undefined,
  property: keyof T,
  value: DropdownValue | undefined
)
{
  const newFilter: any = filter ? { ...filter } : {};

  if (isValueEmpty(value))
  {
    delete newFilter[property];
  }
  else
  {
    newFilter[property] = Array.isArray(value) ? value : [value];
  }

  return newFilter;
}

export function updateFilterRangeProperties<T extends object>(
  filter: T | undefined,
  lowerBoundProperty: keyof T,
  upperBoundProperty: keyof T,
  value: IInterval<any> | undefined
)
{
  let newFilter = updateFilterSingleValueProperty(filter, lowerBoundProperty, value?.lowerBound);
  newFilter = updateFilterSingleValueProperty(newFilter, upperBoundProperty, value?.upperBound);
  return newFilter;
}
