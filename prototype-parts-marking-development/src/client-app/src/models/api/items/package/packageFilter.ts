import { appendFilterQueryParameters } from '../../../../api/utilities';
import IPartCode from '../../../partCode';

export interface IPackageFilter
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
  packageIdentifiers?: string[];
  initialCounts?: number[];
  initialCountLowerLimit?: number;
  initialCountUpperLimit?: number;
  actualCounts?: number[];
  actualCountLowerLimit?: number;
  actualCountUpperLimit?: number;
  customers?: string[];
  projects?: string[];
  projectNumbers?: string[];
  owners?: number[];
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

export function appendPackageFilterQueryParameters(url: string, filter: IPackageFilter)
{
  return appendFilterQueryParameters(url, filter);
}

export function createPackageFilterFromPartCode(partCode: IPartCode): IPackageFilter
{
  return {
    outletCodes: [partCode.outlet],
    productGroupCodes: [partCode.productGroup],
    partTypeCodes: [partCode.partType],
    evidenceYearCodes: [partCode.evidenceYear],
    locationCodes: [partCode.location],
    packageIdentifiers: [partCode.uniqueIdentifier],
    gateLevelCodes: [partCode.gateLevel],
    initialCounts: partCode.numberOfPrototypes >= 0 ? [partCode.numberOfPrototypes] : undefined,
  };
}
