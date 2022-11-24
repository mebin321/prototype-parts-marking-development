import { generateMoniker } from '../../../utilities/objects';
import { codeValidator, descriptionValidator, titleValidator } from './validators';

import { IEnumItem, IEnumItemDescriptor, IEnumItemDescriptorGeneric } from '.';

export interface IProductGroupUpdateData
{
  description: string;
}

export interface IProductGroupCreateData extends IEnumItem, IProductGroupUpdateData
{
  title: string;
}

export interface IProductGroup extends IProductGroupCreateData
{
  moniker: string;
}

export const descriptor: IEnumItemDescriptorGeneric<IProductGroup, IProductGroupCreateData, IProductGroupUpdateData> =
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

export const ProductGroupDescriptor: IEnumItemDescriptor = descriptor;

export function createFakeProductGroup(title: string | undefined, code: string | undefined): IProductGroup | undefined
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
