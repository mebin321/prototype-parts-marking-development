import { AxiosInstance, AxiosPromise } from 'axios';

import { IEnumItem } from '../models/api/enumerations';
import
{
  EvidenceYearDescriptor,
  IEvidenceYear,
  IEvidenceYearCreateData,
} from '../models/api/enumerations/evidenceYears';
import
{
  GateLevelDescriptor,
  IGateLevel,
  IGateLevelCreateData,
  IGateLevelUpdateData,
} from '../models/api/enumerations/gateLevels';
import
{
  ILocation,
  ILocationCreateData,
  ILocationUpdateData,
  LocationDescriptor,
} from '../models/api/enumerations/locations';
import
{
  IOutlet,
  IOutletCreateData,
  IOutletUpdateData,
  OutletDescriptor,
} from '../models/api/enumerations/outlets';
import
{
  IPartType,
  IPartTypeCreateData,
  IPartTypeUpdateData,
  PartTypeDescriptor,
} from '../models/api/enumerations/parts';
import
{
  IProductGroup,
  IProductGroupCreateData,
  IProductGroupUpdateData,
  ProductGroupDescriptor,
} from '../models/api/enumerations/productGroups';
import { IPaginatedResponse } from '../models/api/paginatedResponse';
import { appendPaginationQueryParameters, appendQueryParameter, responseBody } from './utilities';

const OutletsEnumEndpoint = 'outlets';
const PartsEnumEndpoint = 'parts';
const ProductGroupsEnumEndpoint = 'productgroups';

export interface IEnumerationItemsCreateEndpoint<TData extends IEnumItem, TCreateData extends IEnumItem>
{
  readonly createItem: (item: TCreateData) => Promise<TData>;
}

export interface IEnumerationItemReadEndpoint<TData extends IEnumItem>
{
  readonly readItem: (identifier: string | number) => Promise<TData>;
  readonly listItems: (page?: number, pageSize?: number) => Promise<IPaginatedResponse<TData>>;
}

export interface IEnumerationItemsSearchEndpoint<TData extends IEnumItem>
  extends IEnumerationItemReadEndpoint<TData>
{
  readonly searchItems: (text?: string, page?: number, pageSize?: number) => Promise<IPaginatedResponse<TData>>;
}

export interface IEnumerationItemsUpdateEndpoint<TData extends IEnumItem, TUpdateData extends object>
{
  readonly updateItem: (identifier: string, item: TUpdateData) => Promise<TData>;
}

export interface IOutletsEnumerationRelationsEndpoint
{
  readonly listPermittedProductGroups: (moniker: string, text?: string) => Promise<IProductGroup[]>;
  readonly updatePermittedProductGroups: (moniker: string, productGroupMonikers: string[]) => AxiosPromise<void>;
}

export interface IPartTypesEnumerationRelationsEndpoint
{
  readonly listPermittedComponentParts: (moniker: string, text?: string) => Promise<IPartType[]>;
  readonly updatePermittedComponentParts: (moniker: string, partTypeMonikers: string[]) => AxiosPromise<void>;
}

export interface IProductGroupsEnumerationRelationsEndpoint
{
  readonly listPermittedParts: (moniker: string, text?: string) => Promise<IPartType[]>;
  readonly updatePermittedParts: (moniker: string, partTypeMonikers: string[]) => AxiosPromise<void>;
}

export type EvidenceYearsEndpoint =
  IEnumerationItemsCreateEndpoint<IEvidenceYear, IEvidenceYearCreateData>
  & IEnumerationItemReadEndpoint<IEvidenceYear>;

export type GateLevelsEndpoint =
  IEnumerationItemsCreateEndpoint<IGateLevel, IGateLevelCreateData>
  & IEnumerationItemsSearchEndpoint<IGateLevel>
  & IEnumerationItemsUpdateEndpoint<IGateLevel, IGateLevelUpdateData>;

export type LocationsEndpoint =
  IEnumerationItemsCreateEndpoint<ILocation, ILocationCreateData>
  & IEnumerationItemsSearchEndpoint<ILocation>
  & IEnumerationItemsUpdateEndpoint<ILocation, ILocationUpdateData>;

export type OutletsEndpoint =
  IEnumerationItemsCreateEndpoint<IOutlet, IOutletCreateData>
  & IEnumerationItemsSearchEndpoint<IOutlet>
  & IEnumerationItemsUpdateEndpoint<IOutlet, IOutletUpdateData>
  & IOutletsEnumerationRelationsEndpoint;

export type PartTypesEndpoint =
  IEnumerationItemsCreateEndpoint<IPartType, IPartTypeCreateData>
  & IEnumerationItemsSearchEndpoint<IPartType>
  & IEnumerationItemsUpdateEndpoint<IPartType, IPartTypeUpdateData>
  & IPartTypesEnumerationRelationsEndpoint;

export type ProductGroupsEndpoint =
  IEnumerationItemsCreateEndpoint<IProductGroup, IProductGroupCreateData>
  & IEnumerationItemsSearchEndpoint<IProductGroup>
  & IEnumerationItemsUpdateEndpoint<IProductGroup, IProductGroupUpdateData>
  & IProductGroupsEnumerationRelationsEndpoint;

export interface IEnumerationEndpoints
{
  readonly EvidenceYears: EvidenceYearsEndpoint;
  readonly GateLevels: GateLevelsEndpoint;
  readonly Locations: LocationsEndpoint;
  readonly Outlets: OutletsEndpoint;
  readonly Parts: PartTypesEndpoint;
  readonly ProductGroups: ProductGroupsEndpoint;
}

class GenericEnumerationEndpoint<
  TData extends IEnumItem,
  TCreateData extends IEnumItem,
  TUpdateData extends object>
implements
  IEnumerationItemsCreateEndpoint<TData, TCreateData>,
  IEnumerationItemsUpdateEndpoint<TData, TUpdateData>
{
  protected readonly client: AxiosInstance;
  protected readonly endpoint: string;

  public constructor(client: AxiosInstance, endpoint: string)
  {
    if (!client)
    {
      throw new Error('HTTP client must be defined');
    }

    if (!endpoint)
    {
      throw new Error('Endpoint must be non-empty string');
    }

    this.client = client;
    this.endpoint = endpoint.toLowerCase();
  }

  protected async listItemsInternal(text = '', page = 1, pageSize = -1)
  {
    let url = appendQueryParameter(this.endpoint, 'Search', text);
    url = appendPaginationQueryParameters(url, page, pageSize);

    const response = await this.client.get<IPaginatedResponse<TData>>(url);
    return responseBody(response);
  }

  public async createItem(item: TCreateData)
  {
    return this.client.post<TData>(`${this.endpoint}/`, item)
      .then(responseBody);
  }

  public async readItem(identifier: string | number)
  {
    return this.client.get<TData>(`${this.endpoint}/${identifier}`)
      .then(responseBody);
  }

  public async updateItem(identifier: string | number, item: TUpdateData)
  {
    return this.client.put<TData>(`${this.endpoint}/${identifier}`, item)
      .then(responseBody);
  }
}

export class GenericEnumerationEndpointWithoutSearch<
  TData extends IEnumItem,
  TCreateData extends IEnumItem,
  TUpdateData extends object>
  extends GenericEnumerationEndpoint<TData, TCreateData, TUpdateData>
  implements
    IEnumerationItemsCreateEndpoint<TData, TCreateData>,
    IEnumerationItemReadEndpoint<TData>,
    IEnumerationItemsUpdateEndpoint<TData, TUpdateData>
{
  public async listItems(page = 1, pageSize = -1)
  {
    return this.listItemsInternal(undefined, page, pageSize);
  }
}

export class GenericEnumerationEndpointWithSearch<
  TData extends IEnumItem,
  TCreateData extends IEnumItem,
  TUpdateData extends object>
  extends GenericEnumerationEndpointWithoutSearch<TData, TCreateData, TUpdateData>
  implements
    IEnumerationItemsCreateEndpoint<TData, TCreateData>,
    IEnumerationItemsSearchEndpoint<TData>,
    IEnumerationItemsUpdateEndpoint<TData, TUpdateData>
{
  public async searchItems(text = '', page = 1, pageSize = -1)
  {
    return this.listItemsInternal(text, page, pageSize);
  }
}

export class OutletsEnumerationEndpoint
  extends GenericEnumerationEndpointWithSearch<IOutlet, IOutletCreateData, IOutletUpdateData>
  implements
    IEnumerationItemsCreateEndpoint<IOutlet, IOutletCreateData>,
    IEnumerationItemsSearchEndpoint<IOutlet>,
    IEnumerationItemsUpdateEndpoint<IOutlet, IOutletUpdateData>,
    IOutletsEnumerationRelationsEndpoint
{
  public constructor(client: AxiosInstance)
  {
    super(client, OutletsEnumEndpoint);
  }

  public async listPermittedProductGroups(moniker: string, text?: string)
  {
    const url = appendQueryParameter(`${this.endpoint}/${moniker}/productgroups`, 'Search', text);
    return this.client.get<IProductGroup[]>(url)
      .then(responseBody);
  }

  public async updatePermittedProductGroups(moniker: string, productGroupMonikers: string[])
  {
    return this.client.put<void>(`${this.endpoint}/${moniker}/productgroups`, productGroupMonikers);
  }
}

export class PartTypesEnumerationEndpoint
  extends GenericEnumerationEndpointWithSearch<IPartType, IPartTypeCreateData, IPartTypeUpdateData>
  implements
    IEnumerationItemsCreateEndpoint<IPartType, IPartTypeCreateData>,
    IEnumerationItemsSearchEndpoint<IPartType>,
    IEnumerationItemsUpdateEndpoint<IPartType, IPartTypeUpdateData>,
    IPartTypesEnumerationRelationsEndpoint
{
  public constructor(client: AxiosInstance)
  {
    super(client, PartsEnumEndpoint);
  }

  public async listPermittedComponentParts(moniker: string, text?: string)
  {
    const url = appendQueryParameter(`${this.endpoint}/${moniker}/component-parts`, 'Search', text);
    return this.client.get<IPartType[]>(url)
      .then(responseBody);
  }

  public async updatePermittedComponentParts(moniker: string, partTypeMonikers: string[])
  {
    return this.client.put<void>(`${this.endpoint}/${moniker}/component-parts`, partTypeMonikers);
  }
}

export class ProductGroupsEnumerationEndpoint
  extends GenericEnumerationEndpointWithSearch<IProductGroup, IProductGroupCreateData, IProductGroupUpdateData>
  implements
    IEnumerationItemsCreateEndpoint<IProductGroup, IProductGroupCreateData>,
    IEnumerationItemsSearchEndpoint<IProductGroup>,
    IEnumerationItemsUpdateEndpoint<IProductGroup, IProductGroupUpdateData>,
    IProductGroupsEnumerationRelationsEndpoint
{
  public constructor(client: AxiosInstance)
  {
    super(client, ProductGroupsEnumEndpoint);
  }

  public async listPermittedParts(moniker: string, text?: string)
  {
    const url = appendQueryParameter(`${this.endpoint}/${moniker}/parts`, 'Search', text);
    return this.client.get<IPartType[]>(url)
      .then(responseBody);
  }

  public async updatePermittedParts(moniker: string, partTypeMonikers: string[])
  {
    return this.client.put<void>(`${this.endpoint}/${moniker}/parts`, partTypeMonikers);
  }
}

export const EvidenceYearsEnum = 'Evidence Year';
export const GateLevelsEnum = 'Gate Levels';
export const LocationsEnum = 'Locations';
export const OutletsEnum = 'Outlets';
export const PartsEnum = 'Parts';
export const ProductGroupsEnum = 'Product Groups';

export const AllEnumerations = Object.freeze({
  [EvidenceYearsEnum]: EvidenceYearDescriptor,
  [GateLevelsEnum]: GateLevelDescriptor,
  [LocationsEnum]: LocationDescriptor,
  [OutletsEnum]: OutletDescriptor,
  [PartsEnum]: PartTypeDescriptor,
  [ProductGroupsEnum]: ProductGroupDescriptor,
});

export function getEnumEndpoint(name: string): keyof IEnumerationEndpoints
{
  switch (name)
  {
    case EvidenceYearsEnum:
      return 'EvidenceYears';
    case GateLevelsEnum:
      return 'GateLevels';
    case LocationsEnum:
      return 'Locations';
    case OutletsEnum:
      return 'Outlets';
    case ProductGroupsEnum:
      return 'ProductGroups';
    case PartsEnum:
      return 'Parts';
    default:
      throw new Error(`Cannot convert unknown enumeration ${name} to endpoint`);
  }
}
