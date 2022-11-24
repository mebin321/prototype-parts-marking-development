import { generateMoniker } from '../../../utilities/objects';
import { codeValidator, descriptionValidator, titleValidator } from './validators';

import { IEnumItem, IEnumItemDescriptor, IEnumItemDescriptorGeneric } from '.';

export interface IGateLevelUpdateData
{
  description: string;
}

export interface IGateLevelCreateData extends IEnumItem, IGateLevelUpdateData
{
  title: string;
}

export interface IGateLevel extends IGateLevelCreateData
{
  moniker: string;
}

export const descriptor: IEnumItemDescriptorGeneric<IGateLevel, IGateLevelCreateData, IGateLevelUpdateData> =
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

export const GateLevelDescriptor: IEnumItemDescriptor = descriptor;

export function createFakeGateLevel(title: string | undefined, code: string | undefined): IGateLevel | undefined
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
