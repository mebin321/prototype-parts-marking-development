import { generateMoniker } from '../../../utilities/objects';
import { codeValidator, descriptionValidator, titleValidator } from './validators';

import { IEnumItem, IEnumItemDescriptor, IEnumItemDescriptorGeneric } from '.';

export interface IPartTypeUpdateData
{
  description: string;
}

export interface IPartTypeCreateData extends IEnumItem, IPartTypeUpdateData
{
  title: string;
}

export interface IPartType extends IPartTypeCreateData
{
  moniker: string;
}

export const descriptor: IEnumItemDescriptorGeneric<IPartType, IPartTypeCreateData, IPartTypeUpdateData> =
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

export const PartTypeDescriptor: IEnumItemDescriptor = descriptor;

export function createFakePartType(title: string | undefined, code: string | undefined): IPartType | undefined
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
