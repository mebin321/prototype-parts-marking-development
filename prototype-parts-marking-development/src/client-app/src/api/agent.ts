import axios from 'axios';
import _ from 'lodash';

import { store } from '../index';
import { IAuthData } from '../models/api/auth/authData';
import { IEvidenceYear, IEvidenceYearCreateData } from '../models/api/enumerations/evidenceYears';
import { IGateLevel, IGateLevelCreateData, IGateLevelUpdateData } from '../models/api/enumerations/gateLevels';
import { ILocation, ILocationCreateData, ILocationUpdateData } from '../models/api/enumerations/locations';
import { IItemOptions } from '../models/api/items/itemOptions';
import { IPackage } from '../models/api/items/package/package';
import { IPackageCreateData } from '../models/api/items/package/packageCreateData';
import { IPackageFilter, appendPackageFilterQueryParameters } from '../models/api/items/package/packageFilter';
import { IPackageUpdateData } from '../models/api/items/package/packageUpdateData';
import { IPrototype } from '../models/api/items/part/prototype';
import { IPrototypeCreateData } from '../models/api/items/part/prototypeCreateData';
import { IPrototypeFilter, appendPrototypeFilterQueryParameters } from '../models/api/items/part/prototypeFilter';
import { IPrototypeUpdateData } from '../models/api/items/part/prototypeUpdateData';
import { IPrototypeSet } from '../models/api/items/part/set/prototypeSet';
import { IPrototypeSetCreateData } from '../models/api/items/part/set/prototypeSetCreateData';
import
{
  IPrototypeSetFilter,
  appendPrototypeSetFilterQueryParameters,
} from '../models/api/items/part/set/prototypeSetFilter';
import
{
  IPrototypeSetAllItemsFilter,
  IPrototypeSetItemsFilter,
  appendPrototypeSetAllItemsFilterQueryParameters,
  appendPrototypeSetItemsFilterQueryParameters,
} from '../models/api/items/part/set/prototypeSetItemsFilter';
import { IPrototypeVariant } from '../models/api/items/part/variant/prototypeVariant';
import { IPrototypeVariantCreateData } from '../models/api/items/part/variant/prototypeVariantCreateData';
import
{
  IPrototypeVariantFilter,
  appendPrototypeVariantFilterQueryParameters,
} from '../models/api/items/part/variant/prototypeVariantFilter';
import { IApiMetadata } from '../models/api/metadata/apiMetadata';
import { IPrintItem } from '../models/api/print/printItem';
import { IPrintItemCreateData } from '../models/api/print/printItemCreateData';
import { IProject } from '../models/api/projects/project';
import
{
  IAdUsersResponse,
  IPackagesResponse,
  IPrintItemsResponse,
  IPrototypeSetsResponse,
  IPrototypeVariantsExtendedResponse,
  IPrototypeVariantsResponse,
  IPrototypesExtendedResponse,
  IPrototypesResponse,
  IUsersResponse,
} from '../models/api/responses';
import { IListRoles } from '../models/api/roles/rolesList';
import { IUserRole } from '../models/api/roles/userRole';
import { IUserRoleCreateData } from '../models/api/roles/userRoleCreateData';
import { IUserRolesConfiguration } from '../models/api/roles/userRolesConfiguration';
import { ISortParameters, NoSort } from '../models/api/sort/sortParameters';
import { IUserData } from '../models/api/users/userData';
import { IUserFilter, appendUserFilterQueryParameters } from '../models/api/users/userFilter';
import { IUpdateUserData } from '../models/api/users/userUpdateData';
import { authSuccess, logout } from '../store';
import
{
  GenericEnumerationEndpointWithSearch,
  GenericEnumerationEndpointWithoutSearch,
  IEnumerationEndpoints,
  OutletsEnumerationEndpoint,
  PartTypesEnumerationEndpoint,
  ProductGroupsEnumerationEndpoint,
} from './enumerations';
import
{
  Mutex,
  appendPaginationQueryParameters,
  appendQueryParameter,
  appendSortQueryParameters,
  getBaseUrl,
  responseBody,
  responseBodyWithETag,
  transformResponse,
} from './utilities';

export const BaseUrl = getBaseUrl();

const ApiMetadataEndpoint = 'api';
const AuthEndpoint = 'auth';
const AdUsersEndpoint = 'adusers';
const UsersEndpoint = 'users';
const ActiveUsersEndpoint = 'active-users';
const InactiveUsersEndpoint = 'inactive-users';
const RolesEndpoint = 'roles';
const PackagesEndpoint = 'prototypes-packages';
const ActivePackagesEndpoint = 'active-prototypes-packages';
const ScrappedPackagesEndpoint = 'scrapped-prototypes-packages';
const PrototypeSetsEndpoint = 'prototype-sets';
const ActivePrototypeSetsEndpoint = 'active-prototype-sets';
const ScrappedPrototypeSetsEndpoint = 'scrapped-prototype-sets';
const PrototypesEndpoint = 'prototypes';
const ActivePrototypesEndpoint = 'active-prototypes';
const ScrappedPrototypesEndpoint = 'scrapped-prototypes';
const ComponentsEndpoint = 'components';
const VariantsEndpoint = 'variants';
const EvidenceYearsEndpoint = 'evidence-years';
const GateLevelsEnumEndpoint = 'gatelevels';
const LocationsEnumEndpoint = 'locations';
const CustomersEndpoint = 'customers';
const GlobalProjectsEndpoint = 'global-projects';
const PrintingLabelsEndpoint = 'printing-labels';

export const client = axios.create({ baseURL: BaseUrl, transformResponse: transformResponse });
const mutex = new Mutex();

const Metadata = Object.freeze({
  getApiMetadata: () =>
    client.get<IApiMetadata>(ApiMetadataEndpoint)
      .then(responseBody),
});

const AdUsers = Object.freeze({
  search: async (text: string, pageSize = -1) =>
  {
    const page = 1;
    const usernameSearchResponse = await client
      .get<IAdUsersResponse>(
        appendPaginationQueryParameters(appendQueryParameter(AdUsersEndpoint, 'Username', text), page, pageSize))
      .then(responseBody);
    const emailSearchResponse = await client
      .get<IAdUsersResponse>(
        appendPaginationQueryParameters(appendQueryParameter(AdUsersEndpoint, 'Email', text), page, pageSize))
      .then(responseBody);

    return _.unionBy(usernameSearchResponse?.items, emailSearchResponse?.items, user => user.username);
  },
});

const Enumerations: IEnumerationEndpoints = Object.freeze({
  EvidenceYears: new GenericEnumerationEndpointWithoutSearch<IEvidenceYear, IEvidenceYearCreateData, {}>(
    client, EvidenceYearsEndpoint),
  GateLevels: new GenericEnumerationEndpointWithSearch<IGateLevel, IGateLevelCreateData, IGateLevelUpdateData>(
    client, GateLevelsEnumEndpoint),
  Locations: new GenericEnumerationEndpointWithSearch<ILocation, ILocationCreateData, ILocationUpdateData>(
    client, LocationsEnumEndpoint),
  Outlets: new OutletsEnumerationEndpoint(client),
  Parts: new PartTypesEnumerationEndpoint(client),
  ProductGroups:
    new ProductGroupsEnumerationEndpoint(client),
});

const Users = Object.freeze({
  create: (username: string) =>
    client.post<IUserData>(UsersEndpoint, { username: username })
      .then(responseBody),
  read: (id: number) =>
    client.get<IUserData>(`${UsersEndpoint}/${id}`)
      .then(responseBody),
  update: (id: number, data: IUpdateUserData) =>
    client.put<IUserData>(`${UsersEndpoint}/${id}`, data)
      .then(responseBody),
  remove: (id: number) =>
    client.delete<void>(`${ActiveUsersEndpoint}/${id}`),
  restore: (id: number) =>
    client.delete<void>(`${InactiveUsersEndpoint}/${id}`),
  list: (filter: IUserFilter = {}, page = 1, pageSize = -1) =>
  {
    let url = appendUserFilterQueryParameters(UsersEndpoint, filter);
    url = appendPaginationQueryParameters(url, page, pageSize);

    return client.get<IUsersResponse>(url)
      .then(responseBody);
  },
  addRole: (id: number, roleMoniker: string) =>
    client.post<IUserRole[]>(`${UsersEndpoint}/${id}/${RolesEndpoint}`, { moniker: roleMoniker })
      .then(responseBody),
  updateRoles: (id: number, roleMonikers: string[]) =>
    client.put<IUserRole[]>(
      `${UsersEndpoint}/${id}/${RolesEndpoint}`, roleMonikers.map(roleMoniker => { return { moniker: roleMoniker }; }))
      .then(responseBody),
  removeRole: (id: number, roleMoniker: string) =>
    client.delete<IUserRole[]>(`${UsersEndpoint}/${id}/${RolesEndpoint}/${roleMoniker}`)
      .then(responseBody),
  listRoles: (id: number) =>
    client.get<IUserRole[]>(`${UsersEndpoint}/${id}/${RolesEndpoint}`)
      .then(responseBody),
});

const Roles = Object.freeze({
  create: (role: IUserRoleCreateData) =>
    client.post<IUserRole>(RolesEndpoint, role)
      .then(responseBody),
  read: (roleMoniker: string) =>
    client.get<IUserRole>(`${RolesEndpoint}/${roleMoniker}`)
      .then(responseBody),
  update: (roleMoniker: string, data: IUserRoleCreateData) =>
    client.put<IUserRole>(`${RolesEndpoint}/${roleMoniker}`, data)
      .then(responseBody),
  remove: (roleMoniker: string) =>
    client.delete<void>(`${RolesEndpoint}/${roleMoniker}`),
  list: (text = '', page = 1, pageSize = -1) =>
  {
    let url = appendQueryParameter(RolesEndpoint, 'Search', text);
    url = appendPaginationQueryParameters(url, page, pageSize);

    return client.get<IListRoles>(url)
      .then(responseBody);
  },
  listUsers: (roleMoniker: string, filter: IUserFilter = {}, page = 1, pageSize = -1) =>
  {
    let url = `${RolesEndpoint}/${roleMoniker}/${UsersEndpoint}`;
    url = appendUserFilterQueryParameters(url, filter);
    url = appendPaginationQueryParameters(url, page, pageSize);

    return client.get<IUsersResponse>(url)
      .then(responseBody);
  },
  configuration: () => client.options<IUserRolesConfiguration>(RolesEndpoint)
    .then(responseBody),
});

const Auth = Object.freeze({
  login: (username: string, password: string) =>
    client.post<IAuthData>(`${AuthEndpoint}/authenticate`, { username: username, password: password })
      .then(responseBody),
  currentUser: () =>
    client.get<IUserData>(`${AuthEndpoint}/current-user`)
      .then(responseBody),
  refreshToken: (refreshToken: string) =>
    client.post<IAuthData>(`${AuthEndpoint}/refresh-access-token`, { token: refreshToken })
      .then(responseBody),
});

const Packages = Object.freeze({
  list: (filter: IPackageFilter = {}, sort: ISortParameters = NoSort, page = 1, pageSize = -1) =>
  {
    let url = appendPackageFilterQueryParameters(PackagesEndpoint, filter);
    url = appendSortQueryParameters(url, sort);
    url = appendPaginationQueryParameters(url, page, pageSize);
    return client.get<IPackagesResponse>(url)
      .then(responseBody);
  },
  create: (packageToCreate: IPackageCreateData) =>
    client.post<IPackage>(PackagesEndpoint, packageToCreate)
      .then(responseBody),
  read: (packageId: number) =>
    client.get<IPackage>(`${PackagesEndpoint}/${packageId}`)
      .then(responseBodyWithETag),
  update: (packageId: number, etag: string, data: IPackageUpdateData) =>
    client.put<void>(`${PackagesEndpoint}/${packageId}`, data, { headers: { 'If-Match': `"${etag}"` } }),
  configuration: () => client.options<IItemOptions>(PackagesEndpoint)
    .then(responseBody),
  scrap: (packageId: number) =>
    client.delete<void>(`${ActivePackagesEndpoint}/${packageId}`),
  restore: (packageId: number) =>
    client.delete<void>(`${ScrappedPackagesEndpoint}/${packageId}`),
});

const PrototypeSets = Object.freeze({
  list: (filter: IPrototypeSetFilter = {}, sort: ISortParameters = NoSort, page = 1, pageSize = -1) =>
  {
    let url = appendPrototypeSetFilterQueryParameters(PrototypeSetsEndpoint, filter);
    url = appendSortQueryParameters(url, sort);
    url = appendPaginationQueryParameters(url, page, pageSize);
    return client.get<IPrototypeSetsResponse>(url)
      .then(responseBody);
  },
  listItems: (
    prototypeSetId: number,
    filter: IPrototypeSetAllItemsFilter = {},
    sort: ISortParameters = NoSort,
    page = 1,
    pageSize = -1
  ) =>
  {
    let url = `${PrototypeSetsEndpoint}/${prototypeSetId}/${PrototypesEndpoint}`;
    url = appendPrototypeSetAllItemsFilterQueryParameters(url, filter);
    url = appendSortQueryParameters(url, sort);
    url = appendPaginationQueryParameters(url, page, pageSize);
    return client.get<IPrototypesResponse>(url)
      .then(responseBody);
  },
  listActiveItems: (
    prototypeSetId: number,
    filter: IPrototypeSetItemsFilter = {},
    sort: ISortParameters = NoSort,
    page = 1,
    pageSize = -1
  ) =>
  {
    let url = `${PrototypeSetsEndpoint}/${prototypeSetId}/${ActivePrototypesEndpoint}`;
    url = appendPrototypeSetItemsFilterQueryParameters(url, filter);
    url = appendSortQueryParameters(url, sort);
    url = appendPaginationQueryParameters(url, page, pageSize);
    return client.get<IPrototypesResponse>(url)
      .then(responseBody);
  },
  listScrappedItems: (
    prototypeSetId: number,
    filter: IPrototypeSetItemsFilter = {},
    sort: ISortParameters = NoSort,
    page = 1,
    pageSize = -1
  ) =>
  {
    let url = `${PrototypeSetsEndpoint}/${prototypeSetId}/${ScrappedPrototypesEndpoint}`;
    url = appendPrototypeSetItemsFilterQueryParameters(url, filter);
    url = appendSortQueryParameters(url, sort);
    url = appendPaginationQueryParameters(url, page, pageSize);
    return client.get<IPrototypesResponse>(url)
      .then(responseBody);
  },
  create: (prototypeSet: IPrototypeSetCreateData) =>
    client.post<IPrototypeSet>(PrototypeSetsEndpoint, prototypeSet)
      .then(responseBody),
  read: (prototypeSetId: number) =>
    client.get<IPrototypeSet>(`${PrototypeSetsEndpoint}/${prototypeSetId}`)
      .then(responseBodyWithETag),
  configuration: () => client.options<IItemOptions>(PrototypeSetsEndpoint)
    .then(responseBody),
  scrap: (prototypeSetId: number) =>
    client.delete<void>(`${ActivePrototypeSetsEndpoint}/${prototypeSetId}`),
  restore: (prototypeSetId: number) =>
    client.delete<void>(`${ScrappedPrototypeSetsEndpoint}/${prototypeSetId}`),
});

const Prototypes = Object.freeze({
  list: (filter: IPrototypeFilter = {}, sort: ISortParameters = NoSort, page = 1, pageSize = -1) =>
  {
    let url = appendPrototypeFilterQueryParameters(PrototypesEndpoint, filter);
    url = appendSortQueryParameters(url, sort);
    url = appendPaginationQueryParameters(url, page, pageSize);
    return client.get<IPrototypesExtendedResponse>(url)
      .then(responseBody);
  },
  listVariants: (prototypeSetId: number, prototypeId: number, page = 1, pageSize = -1) =>
  {
    let url = `${PrototypeSetsEndpoint}/${prototypeSetId}/${PrototypesEndpoint}/${prototypeId}/${VariantsEndpoint}`;
    url = appendPaginationQueryParameters(url, page, pageSize);
    return client.get<IPrototypeVariantsResponse>(url)
      .then(responseBody);
  },
  createPrototype: (prototypeSetId: number, etag: string, prototypes: IPrototypeCreateData[]) =>
    client.post<IPrototype[]>(
      `${PrototypeSetsEndpoint}/${prototypeSetId}/${PrototypesEndpoint}`,
      prototypes,
      { headers: { 'If-Match': `"${etag}"` } }
    ).then(responseBody),
  createComponent: (prototypeSetId: number, etag: string, prototypes: IPrototypeCreateData[]) =>
    client.post<IPrototype[]>(
      `${PrototypeSetsEndpoint}/${prototypeSetId}/${ComponentsEndpoint}`,
      prototypes,
      { headers: { 'If-Match': `"${etag}"` } }
    ).then(responseBody),
  read: (prototypeSetId: number, prototypeId: number) =>
    client.get<IPrototype>(`${PrototypeSetsEndpoint}/${prototypeSetId}/${PrototypesEndpoint}/${prototypeId}`)
      .then(responseBodyWithETag),
  update: (prototypeSetId: number, prototypeId: number, etag: string, data: IPrototypeUpdateData) =>
    client.put<void>(
      `${PrototypeSetsEndpoint}/${prototypeSetId}/${PrototypesEndpoint}/${prototypeId}`,
      data,
      { headers: { 'If-Match': `"${etag}"` } }),
  scrap: (prototypeSetId: number, prototypeId: number) =>
    client.delete<void>(`${PrototypeSetsEndpoint}/${prototypeSetId}/${ActivePrototypesEndpoint}/${prototypeId}`),
  restore: (prototypeSetId: number, prototypeId: number) =>
    client.delete<void>(`${PrototypeSetsEndpoint}/${prototypeSetId}/${ScrappedPrototypesEndpoint}/${prototypeId}`),
  configuration: () => client.options<IItemOptions>(PrototypesEndpoint)
    .then(responseBody),
});

const Variants = Object.freeze({
  list: (filter: IPrototypeVariantFilter = {}, page = 1, pageSize = -1) =>
  {
    let url = appendPrototypeVariantFilterQueryParameters(VariantsEndpoint, filter);
    url = appendPaginationQueryParameters(url, page, pageSize);
    return client.get<IPrototypeVariantsExtendedResponse>(url)
      .then(responseBody);
  },
  create: (prototypeSetId: number, prototypeId: number, etag: string, data: IPrototypeVariantCreateData) =>
    client.post<IPrototypeVariant>(
      `${PrototypeSetsEndpoint}/${prototypeSetId}/${PrototypesEndpoint}/${prototypeId}/${VariantsEndpoint}`,
      data,
      { headers: { 'If-Match': `"${etag}"` } }
    ).then(responseBody),
  read: (prototypeSetId: number, prototypeId: number, version: number) =>
    client.get<IPrototypeVariant>(
      `${PrototypeSetsEndpoint}/${prototypeSetId}/${PrototypesEndpoint}/${prototypeId}/${VariantsEndpoint}/${version}`)
      .then(responseBody),
});

const GlobalProjects = Object.freeze({
  ListCustomers: () => client.get<string[]>(`${CustomersEndpoint}`).then(responseBody),

  ListProjects: (
    projectNumber?: string,
    customer?: string,
    searchText?: string,
    limit?: number) =>
  {
    let url = appendQueryParameter(GlobalProjectsEndpoint, 'ProjectNumber', projectNumber);
    url = appendQueryParameter(url, 'Customer', customer);
    url = appendQueryParameter(url, 'Search', searchText);
    url = appendQueryParameter(url, 'Limit', limit);
    return client.get<IProject[]>(url).then(responseBody);
  },
});

const PrintingLabels = Object.freeze({
  list: (ownerId: number, page = 1, pageSize = -1) =>
  {
    let url = appendQueryParameter(PrintingLabelsEndpoint, 'OwnerId', ownerId);
    url = appendPaginationQueryParameters(url, page, pageSize);

    return client.get<IPrintItemsResponse>(url).then(responseBody);
  },
  create: (items: IPrintItemCreateData[]) =>
    client.post<IPrintItem[]>(PrintingLabelsEndpoint, items).then(responseBody),
  remove: (id: number) =>
    client.delete<void>(`${PrintingLabelsEndpoint}/${id}`),
});

async function refreshAccessToken(): Promise<void>
{
  const unlock = await mutex.lock();
  try
  {
    const currentTime = Date.now();
    const state = store.getState();
    const accessTokenExpiration = Date.parse(state.auth.accessToken.expiresAt);
    const refreshTokenExpiration = Date.parse(state.auth.refreshToken.expiresAt);

    // access token is not expired - refresh is not needed (it might be refreshed by concurrent request)
    if (accessTokenExpiration > currentTime)
    {
      return Promise.resolve();
    }

    // no authentication data is found in local storage
    if (isNaN(refreshTokenExpiration))
    {
      return Promise.reject(new Error('Not logged in'));
    }

    // refresh token is expired - do logout to reflect this in UI
    if (refreshTokenExpiration < currentTime)
    {
      store.dispatch(logout());
      return Promise.reject(new Error('Cannot refresh access token because refresh token has expired'));
    }

    // try to refresh access token using refresh token
    const newAuthData = await Auth.refreshToken(state.auth.refreshToken.token);
    const newUserRoles = await Users.listRoles(newAuthData.user.id);
    store.dispatch(authSuccess(newAuthData, newUserRoles));
    return Promise.resolve();
  }
  finally
  {
    unlock();
  }
}

client.interceptors.request.use(request =>
{
  const state = store.getState();
  const token = state?.auth?.accessToken?.token;

  if (token)
  {
    request.headers = { ...request.headers, Authorization: `Bearer ${token}` };
  }

  return request;
});

client.interceptors.response.use(undefined, error =>
{
  const request = error.config;
  const status = error.response?.status;
  // TODO status 400 is temporary and shall be removed when all requests requiring access token will be fixed on backend
  if ((status === 400 || status === 401) &&
      !request.retried && !request.url?.startsWith(`${AuthEndpoint}/refresh-access-token`))
  {
    return refreshAccessToken()
      .catch(authError =>
      {
        return authError.isAxiosError
          ? Promise.reject(authError) // return axios error from refreshing access token
          : Promise.reject(error); // return original request error
      })
      .then(() =>
      {
        request.retried = true; // mark request as retried
        return client(request); // retry original request
      });
  }

  return Promise.reject(error);
});

const Agent = Object.freeze({
  Metadata,
  Auth,
  AdUsers,
  Users,
  Roles,
  Enumerations,
  Packages,
  PrototypeSets,
  Prototypes,
  Variants,
  GlobalProjects,
  PrintingLabels,
});
export default Agent;
