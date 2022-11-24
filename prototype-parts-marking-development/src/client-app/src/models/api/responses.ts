import { IAdUser } from './adUsers/adUser';
import { IEvidenceYear } from './enumerations/evidenceYears';
import { IGateLevel } from './enumerations/gateLevels';
import { ILocation } from './enumerations/locations';
import { IOutlet } from './enumerations/outlets';
import { IPartType } from './enumerations/parts';
import { IProductGroup } from './enumerations/productGroups';
import { IPackage } from './items/package/package';
import { IPrototype } from './items/part/prototype';
import { IPrototypeExtended } from './items/part/prototypeExtended';
import { IPrototypeSet } from './items/part/set/prototypeSet';
import { IPrototypeVariant } from './items/part/variant/prototypeVariant';
import { IPrototypeVariantExtended } from './items/part/variant/prototypeVariantExtended';
import { IPaginatedResponse } from './paginatedResponse';
import { IPrintItem } from './print/printItem';
import { IUserRole } from './roles/userRole';
import { IUserData } from './users/userData';

export type IAdUsersResponse = IPaginatedResponse<IAdUser>;
export type IPackagesResponse = IPaginatedResponse<IPackage>;
export type IPrototypeSetsResponse = IPaginatedResponse<IPrototypeSet>;
export type IPrototypesResponse = IPaginatedResponse<IPrototype>;
export type IPrototypesExtendedResponse = IPaginatedResponse<IPrototypeExtended>;
export type IPrototypeVariantsResponse = IPaginatedResponse<IPrototypeVariant>;
export type IPrototypeVariantsExtendedResponse = IPaginatedResponse<IPrototypeVariantExtended>;
export type IPrintItemsResponse = IPaginatedResponse<IPrintItem>;
export type IUserRolesResponse = IPaginatedResponse<IUserRole>;
export type IUsersResponse = IPaginatedResponse<IUserData>;
export type EnumItemsResponse = IPaginatedResponse<IEvidenceYear> | IPaginatedResponse<IGateLevel>
  | IPaginatedResponse<ILocation> | IPaginatedResponse<IOutlet> | IPaginatedResponse<IPartType>
  | IPaginatedResponse<IProductGroup>;
