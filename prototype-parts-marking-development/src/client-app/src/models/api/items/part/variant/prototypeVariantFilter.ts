import { appendFilterQueryParameters } from '../../../../../api/utilities';

export interface IPrototypeVariantFilter
{
  isActive?: boolean;
  search?: string;
}

export function appendPrototypeVariantFilterQueryParameters(url: string, filter: IPrototypeVariantFilter)
{
  return appendFilterQueryParameters(url, filter);
}
