import { generateMoniker } from '../../../utilities/objects';
import { codeValidator, descriptionValidator, titleValidator } from './validators';

import { IEnumItem, IEnumItemDescriptor, IEnumItemDescriptorGeneric } from '.';

export interface IOutletUpdateData
{
  description: string;
}

export interface IOutletCreateData extends IEnumItem, IOutletUpdateData
{
  title: string;
}

export interface IOutlet extends IOutletCreateData
{
  moniker: string;
}

export const descriptor: IEnumItemDescriptorGeneric<IOutlet, IOutletCreateData, IOutletUpdateData> =
{
  properties:
  {
    moniker: { type: 'text' },
    title: { type: 'text', validator: titleValidator('Title') },
    code: { type: 'text', validator: codeValidator('Code') },
    description: { type: 'text', validator: descriptionValidator('Description') },
  },
  initialProperties: ['title', 'code', 'description'],
  visibleProperties: ['title', 'code', 'description'],
  editableProperties: ['description'],
};

export const OutletDescriptor: IEnumItemDescriptor = descriptor;

export function createFakeOutlet(title: string | undefined, code: string | undefined): IOutlet | undefined
{
  if (title && code)
  {
    return {
      moniker: generateMoniker(title),
      title: title,
      code: code,
      description: '',
    };
  }

  return undefined;
}
