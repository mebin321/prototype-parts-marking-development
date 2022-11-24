import { IPackage } from '../../../models/api/items/package/package';
import IPartCode from '../../../models/partCode';
import { formatPartCode } from '../itemsUtilities';

export function formatPackagePartCode(item: IPackage)
{
  const partCodeData: IPartCode =
  {
    outlet: item.outletCode,
    productGroup: item.productGroupCode,
    partType: item.partTypeCode,
    evidenceYear: item.evidenceYearCode,
    location: item.locationCode,
    uniqueIdentifier: item.packageIdentifier,
    gateLevel: item.gateLevelCode,
    numberOfPrototypes: item.initialCount,
  };

  return formatPartCode(partCodeData);
}

export const generatePackagePrintLabel = (packageData: IPackage) =>
{
  return {
    customer: packageData.customer,
    productGroup: packageData.productGroupTitle,
    partType: packageData.partTypeTitle,
    description: packageData.comment,
    partCode: formatPackagePartCode(packageData),
    outletTitle: packageData.outletTitle,
    locationTitle: packageData.locationTitle,
    projectNumber: packageData.projectNumber,
    gateLevelTitle: packageData.gateLevelTitle,
  };
};
