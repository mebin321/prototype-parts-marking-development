import { appendFilterQueryParameters } from '../../../../../api/utilities';

export interface IPrototypeSetItemsFilter
{
  type?: string;
  index?: number;
}

export interface IPrototypeSetAllItemsFilter extends IPrototypeSetItemsFilter
{
  isActive?: boolean;
}

export function appendPrototypeSetItemsFilterQueryParameters(url: string, filter: IPrototypeSetItemsFilter)
{
  return appendFilterQueryParameters(url, filter);
}

export function appendPrototypeSetAllItemsFilterQueryParameters(url: string, filter: IPrototypeSetItemsFilter)
{
  return appendFilterQueryParameters(url, filter);
}
