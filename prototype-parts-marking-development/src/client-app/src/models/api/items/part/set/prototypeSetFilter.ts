import { appendFilterQueryParameters } from '../../../../../api/utilities';
import IPartCode from '../../../../partCode';

export interface IPrototypeSetFilter
{
  isActive?: boolean;
  outletCodes?: string[];
  outletTitles?: string[];
  productGroupCodes?: string[];
  productGroupTitles?: string[];
  gateLevelCodes?: string[];
  gateLevelTitles?: string[];
  locationCodes?: string[];
  locationTitles?: string[];
  evidenceYearCodes?: string[];
  evidenceYearTitles?: number[];
  evidenceYearLowerLimit?: number;
  evidenceYearUpperLimit?: number;
  setIdentifiers?: string[];
  customers?: string[];
  projects?: string[];
  projectNumbers?: string[];
  createdBy?: number[];
  modifiedBy?: number[];
  deletedBy?: number[];
  createdAtLowerLimit?: Date;
  createdAtUpperLimit?: Date;
  modifiedAtLowerLimit?: Date;
  modifiedAtUpperLimit?: Date;
  deletedAtLowerLimit?: Date;
  deletedAtUpperLimit?: Date;
}

export function appendPrototypeSetFilterQueryParameters(url: string, filter: IPrototypeSetFilter)
{
  return appendFilterQueryParameters(url, filter);
}

export function createPrototypeSetFilterFromPartCode(partCode: IPartCode): IPrototypeSetFilter
{
  return {
    outletCodes: [partCode.outlet],
    productGroupCodes: [partCode.productGroup],
    evidenceYearCodes: [partCode.evidenceYear],
    locationCodes: [partCode.location],
    setIdentifiers: [partCode.uniqueIdentifier],
    gateLevelCodes: [partCode.gateLevel],
  };
}
