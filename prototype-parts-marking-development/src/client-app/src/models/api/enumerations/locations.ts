import { generateMoniker } from '../../../utilities/objects';
import { codeValidator, descriptionValidator, titleValidator } from './validators';

import { IEnumItem, IEnumItemDescriptor, IEnumItemDescriptorGeneric } from '.';

export interface ILocationUpdateData
{
  description: string;
}

export interface ILocationCreateData extends IEnumItem, ILocationUpdateData
{
  title: string;
}

export interface ILocation extends ILocationCreateData
{
  moniker: string;
}

export const descriptor: IEnumItemDescriptorGeneric<ILocation, ILocationCreateData, ILocationUpdateData> =
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

export const LocationDescriptor: IEnumItemDescriptor = descriptor;

export function createFakeLocation(title: string | undefined, code: string | undefined): ILocation | undefined
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
