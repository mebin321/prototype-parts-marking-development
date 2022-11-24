import { codeValidator, yearValidator } from './validators';

import { IEnumItem, IEnumItemDescriptor, IEnumItemDescriptorGeneric } from '.';

export interface IEvidenceYearCreateData extends IEnumItem
{
  year: number;
}

export interface IEvidenceYear extends IEvidenceYearCreateData
{
}

const descriptor: IEnumItemDescriptorGeneric<IEvidenceYear, IEvidenceYearCreateData, {}> =
{
  properties:
  {
    year: { type: 'number', validator: yearValidator('Year') },
    code: { type: 'text', validator: codeValidator('Code') },
  },
  initialProperties: ['year', 'code'],
  visibleProperties: ['year', 'code'],
  editableProperties: [],
};

export const EvidenceYearDescriptor: IEnumItemDescriptor = descriptor;

export function createFakeEvidenceYear(year: number | undefined, code: string | undefined): IEvidenceYear | undefined
{
  if (year !== undefined && code)
  {
    return {
      year: year,
      code: code,
    };
  }

  return undefined;
}
