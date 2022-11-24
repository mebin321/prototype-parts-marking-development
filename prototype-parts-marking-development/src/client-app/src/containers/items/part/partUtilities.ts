
import { IPrototype } from '../../../models/api/items/part/prototype';
import { IPrototypeCreateData } from '../../../models/api/items/part/prototypeCreateData';
import { IPrototypeSet } from '../../../models/api/items/part/set/prototypeSet';
import IPartCode from '../../../models/partCode';
import { formatPartCode } from '../itemsUtilities';

type NumberingType =
{
  readonly Even: string;
  readonly Odd: string;
  readonly Consecutive: string;
};

export const NumberingStyle: NumberingType =
{
  Consecutive: 'Consecutive',
  Odd: 'Odd',
  Even: 'Even',
};

export const NumberingStyleOptions = [
  { key: '1', text: NumberingStyle.Consecutive, value: NumberingStyle.Consecutive },
  { key: '2', text: NumberingStyle.Odd, value: NumberingStyle.Odd },
  { key: '3', text: NumberingStyle.Even, value: NumberingStyle.Even },
];

function calculateStartIndex(
  initialNumberOfPrototypes: number,
  partEvenness: string
)
{
  const isInitialNumberEven = (initialNumberOfPrototypes % 2) === 0;
  if (isInitialNumberEven)
  {
    return partEvenness === NumberingStyle.Even ? initialNumberOfPrototypes + 2 : initialNumberOfPrototypes + 1;
  }
  else
  {
    return partEvenness === NumberingStyle.Odd ? initialNumberOfPrototypes + 2 : initialNumberOfPrototypes + 1;
  }
}

export function generatePartIndices(
  initialNumberOfPrototypes: number,
  numberOfPrototypes: number,
  partEvenness: string
)
{
  const prototypeCodesIndexes: number[] = [];

  const startIndex = calculateStartIndex(initialNumberOfPrototypes, partEvenness);
  const step = partEvenness === NumberingStyle.Consecutive ? 1 : 2;

  for (let index = 0; index < numberOfPrototypes; index++)
  {
    prototypeCodesIndexes.push(startIndex + index * step);
  }

  return prototypeCodesIndexes;
}

export function generatePrototypesCreateData(
  partCodeIndexes: number[],
  partTypeMoniker: string,
  materialNumber: string,
  revisionCode: string,
  ownerId: number,
  comment: string
)
{
  const prototypes: IPrototypeCreateData[] = [];
  partCodeIndexes.forEach(index =>
  {
    prototypes.push(
      {
        index: index,
        partMoniker: partTypeMoniker,
        materialNumber: materialNumber.toUpperCase(),
        revisionCode: revisionCode,
        ownerId: ownerId,
        comment: comment,
      }
    );
  });
  return prototypes;
}

export function formatPrototypePartCode(item: IPrototypeSet, partType: string, partIndex: number)
{
  const partCodeData: IPartCode =
  {
    outlet: item.outletCode,
    productGroup: item.productGroupCode,
    partType: partType,
    evidenceYear: item.evidenceYearCode,
    location: item.locationCode,
    uniqueIdentifier: item.setIdentifier,
    gateLevel: item.gateLevelCode,
    numberOfPrototypes: partIndex,
  };

  return formatPartCode(partCodeData);
}

export const generatePartPrintLabel = (prototypeSetData: IPrototypeSet, prototypeData: IPrototype) =>
{
  return {
    customer: prototypeSetData?.customer,
    productGroup: prototypeSetData.productGroupTitle,
    description: prototypeData.comment,
    partType: prototypeData.partTypeTitle,
    partCode: formatPrototypePartCode(prototypeSetData, prototypeData.partTypeCode, prototypeData.index),
    materialNumber: prototypeData.materialNumber,
    revisionCode: prototypeData.revisionCode,
    outlet: prototypeSetData.outletTitle,
    location: prototypeSetData.locationTitle,
    projectNumber: prototypeSetData.projectNumber,
    gateLevel: prototypeSetData.gateLevelTitle,
  };
};
