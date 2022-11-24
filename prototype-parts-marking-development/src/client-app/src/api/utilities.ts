import { AxiosResponse, AxiosTransformer } from 'axios';

import { IPaginatedResponse } from '../models/api/paginatedResponse';
import { ISortParameters } from '../models/api/sort/sortParameters';
import { capitalizeFirstWord } from '../utilities/text';

// cannot use validator.isISO8601 function because 4-digit number is also valid ISO8601 date
// therefore package/set identifiers (e.g. 0002) would be mistaken with date
const dateISOFormatPattern = /^\d{1,4}-\d{1,2}-\d{1,2}T\d{1,2}:\d{1,2}:\d{1,2}(\.\d+)?(Z|[+-]?\d{1,2}:\d{1,2})$/;

export class Mutex
{
  private mutex = Promise.resolve();

  lock(): PromiseLike<() => void>
  {
    let begin: (unlock: () => void) => void = _unlock => {};

    this.mutex = this.mutex.then(() =>
    {
      return new Promise(begin);
    });

    return new Promise(resolve =>
    {
      begin = resolve;
    });
  }

  async dispatch<T>(fn: (() => T) | (() => PromiseLike<T>)): Promise<T>
  {
    const unlock = await this.lock();
    try
    {
      return await Promise.resolve(fn());
    }
    finally
    {
      unlock();
    }
  }
}

export type TaggedData<T extends object> =
{
  [P in keyof T]: T[P];
} & { readonly etag: string };

export function responseBody<T extends object>(response: AxiosResponse<T>)
{
  return response?.data;
}

export function responseBodyWithETag<T extends object>(response: AxiosResponse<T>): TaggedData<T>
{
  return { ...response?.data, etag: response.headers?.etag };
}

function reviver(_key: string, value: any)
{
  if (typeof value === 'string' && dateISOFormatPattern.test(value))
  {
    return new Date(value);
  }

  return value;
}

export const transformResponse: AxiosTransformer = (data) =>
{
  if (!data) return data; // prevent unexpected end of JSON input error

  if (typeof data === 'string')
  {
    return JSON.parse(data, reviver);
  }

  return data;
};

function extractResponseTitle(response: any)
{
  const errorTitle = response?.data?.title;
  const errorTitleString = errorTitle ? String(errorTitle) : '';
  return errorTitleString.trim();
}

function extractResponseStatus(response: any)
{
  const responseStatusCode = response?.status;
  const responseStatusText = response?.statusText;

  let result = '';
  if (responseStatusCode)
  {
    result = `Error ${responseStatusCode}`;
    if (responseStatusText)
    {
      result += ` (${responseStatusText})`;
    }
  }

  return result;
}

function extractErrorTitle(error: any)
{
  const response = error?.response;
  const responseTitle = extractResponseTitle(response);
  const responseStatus = extractResponseStatus(response);

  return responseTitle || responseStatus;
}

function extractErrorMessage(error: any, includeTraceId: boolean)
{
  const responseData = error?.response?.data;
  if (!responseData) return '';

  let errorMessage = '';
  if (responseData.detail)
  {
    errorMessage = String(responseData.detail);
  }
  else if (responseData.errors)
  {
    for (const errorKey in responseData.errors)
    {
      const errorValue = responseData.errors[errorKey];
      const errorValueFormatted = Array.isArray(errorValue) ? errorValue.join(', ') : String(errorValue);
      errorMessage += `${errorKey}: ${errorValueFormatted}\n`;
    }

    errorMessage = errorMessage.trim();
    errorMessage = '\n' + errorMessage;
  }

  if (includeTraceId && responseData.traceId && errorMessage)
  {
    errorMessage += ` (trace ID: ${responseData.traceId})`;
  }

  return errorMessage;
}

export function extractErrorDetails(error: any, includeTraceId = false)
{
  let errorTitle = extractErrorTitle(error);
  let errorDetails = extractErrorMessage(error, includeTraceId);

  // use generic error message only when
  // no status code with explanation is extracted and
  // no error message from the server response is extracted
  // because otherwise it has no added value, see example below
  // status code: 404 NOT FOUND
  // error message: Request failed with status code 404
  if (!errorTitle && !errorDetails)
  {
    errorDetails = error?.message ? String(error.message) : '';
  }

  if (errorTitle.endsWith('.'))
  {
    errorTitle = errorTitle.substring(0, errorTitle.length - 1);
  }

  return [errorTitle, errorDetails].filter(x => x).join(': ');
}

export function extractErrorDetailsFromPutResponse(error: any, includeTraceId = false)
{
  const statusCode = error?.response?.status;

  if (statusCode === 409)
  {
    return 'Save conflict: Someone else made changes to this item concurrently.\n\n' +
           'Please mark somewhere changes made to this item, then reload this item, make changes and then save again.';
  }

  if (statusCode === 412)
  {
    return 'Save conflict: Item state is not the most recent one.\n\n' +
           'Please mark somewhere changes made to this item, then reload this item, make changes and then save again.';
  }

  return extractErrorDetails(error, includeTraceId);
}

function convertQueryParameterValueToString(value: any)
{
  if (value instanceof Date)
  {
    return value.toISOString();
  }

  return value.toString();
}

function appendQueryParameterInternal(url: URL, name: string, value: any)
{
  if (value === undefined || value === null) return url;

  if (Array.isArray(value))
  {
    for (const element of value)
    {
      appendQueryParameterInternal(url, name, element);
    }

    return;
  }

  const valueString = convertQueryParameterValueToString(value);
  if (!valueString) return url;

  url.searchParams.append(name, valueString);
}

export function appendQueryParameters<T extends object>(url: string, parameters: T)
{
  // add localhost as base URL during instantiation to support relative URLs
  const baseUrl = 'http://127.0.0.1/';
  const urlObject = new URL(url, baseUrl);

  for (const key in parameters)
  {
    appendQueryParameterInternal(urlObject, key, parameters[key]);
  }

  // remove localhost before returning URL
  let resultUrl = urlObject.toString();
  if (resultUrl.startsWith(baseUrl) && !url.startsWith(baseUrl))
  {
    resultUrl = resultUrl.replace(baseUrl, '');
  }

  return resultUrl;
}

export function appendQueryParameter(url: string, name: string, value: any)
{
  return appendQueryParameters(url, { [name]: value });
}

export function appendPaginationQueryParameters(url: string, page = 1, pageSize = -1)
{
  const paginationParameters: any = {};
  paginationParameters.Page = page;
  if (pageSize > 0)
  {
    paginationParameters.PageSize = pageSize;
  }

  return appendQueryParameters(url, paginationParameters);
}

export function appendSortQueryParameters(url: string, sort: ISortParameters)
{
  return appendQueryParameters(url, { SortBy: sort.sortBy, sortDirection: sort.sortDirection });
}

export function appendFilterQueryParameters(url: string, filter: object, mapping = new Map<string, string>())
{
  const queryParameters: any = {};
  for (const key in filter)
  {
    const value = filter[key as keyof object];
    if (value === undefined)
    {
      continue;
    }

    let name = mapping.get(key);
    if (!name)
    {
      name = capitalizeFirstWord(key);
    }

    queryParameters[name] = value;
  }

  return appendQueryParameters(url, queryParameters);
}

export async function listAll<T>(fetchFromServer: (page: number) => Promise<IPaginatedResponse<T>>)
{
  const items: T[] = [];
  const response1 = await fetchFromServer(1);
  items.push(...response1.items);
  for (let page = 2; page <= response1.pagination.totalPages; page++)
  {
    const response2 = await fetchFromServer(page);
    items.push(...response2.items);
  }

  return items;
}

// function that create BaseUrl value according to where is this app hosted
// If it is run from Localhost BaseUrl value is "https://ppmt-test1.conti.de/"
// Else BaseUrl value is window.location.origin.
export function getBaseUrl()
{
  return window.location.hostname.toLowerCase() === 'localhost'
    ? 'https://ppmt-test1.conti.de/api/v1'
    : `${window.location.origin}/api/v1`;
}
