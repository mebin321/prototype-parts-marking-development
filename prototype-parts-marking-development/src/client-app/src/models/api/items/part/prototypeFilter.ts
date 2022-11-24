import { appendFilterQueryParameters } from '../../../../api/utilities';
import IPartCode from '../../../partCode';

export interface IPrototypeFilter
{
  isActive?: boolean;
  search?: string;
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
  partTypeCodes?: string[];
  partTypeTitles?: string[];
  type?: string;
  setIdentifiers?: string[];
  indexes?: number[];
  indexLowerLimit?: number;
  indexUpperLimit?: number;
  prototypeSets?: number[];
  owners?: number[];
  customers?: string[];
  projects?: string[];
  projectNumbers?: string[];
  materialNumbers?: string[];
  revisionCodes?: string[];
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

export function appendPrototypeFilterQueryParameters(url: string, filter: IPrototypeFilter)
{
  return appendFilterQueryParameters(url, filter);
}

export function createPrototypeFilterFromPartCode(partCode: IPartCode): IPrototypeFilter
{
  return {
    outletCodes: [partCode.outlet],
    productGroupCodes: [partCode.productGroup],
    partTypeCodes: [partCode.partType],
    evidenceYearCodes: [partCode.evidenceYear],
    locationCodes: [partCode.location],
    setIdentifiers: [partCode.uniqueIdentifier],
    gateLevelCodes: [partCode.gateLevel],
    indexes: partCode.numberOfPrototypes >= 0 ? [partCode.numberOfPrototypes] : undefined,
  };
}
