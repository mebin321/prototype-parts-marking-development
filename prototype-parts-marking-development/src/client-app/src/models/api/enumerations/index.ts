import { ConfiguredValidator } from 'revalidate';

import { IEvidenceYear } from './evidenceYears';
import { IGateLevel } from './gateLevels';
import { ILocation } from './locations';
import { IOutlet } from './outlets';
import { IPartType } from './parts';
import { IProductGroup } from './productGroups';

export interface IEnumItem
{
  code: string;
}

type inputType = 'button' | 'checkbox' | 'color' | 'date' | 'datetime-local' | 'email' | 'file' | 'hidden' | 'image'
  | 'month' | 'number' | 'password' | 'radio' | 'range' | 'reset' | 'search' | 'submit' | 'tel' | 'text' | 'time'
  | 'url' | 'week';

export interface IEnumItemPropertyDescriptor
{
  readonly type: inputType;
  readonly validator?: ConfiguredValidator;
}

export interface IEnumItemDescriptorGeneric<
  TData extends IEnumItem,
  TCreateData extends IEnumItem,
  TUpdateData extends object>
{
  readonly properties: Record<Extract<keyof TData, string>, IEnumItemPropertyDescriptor>;
  readonly initialProperties: Extract<keyof TCreateData, string>[];
  readonly visibleProperties: Extract<keyof TData, string>[];
  readonly editableProperties: Extract<keyof TUpdateData, string>[];
}

export interface IEnumItemDescriptor
{
  readonly properties: Record<string, IEnumItemPropertyDescriptor>;
  readonly initialProperties: string[];
  readonly visibleProperties: string[];
  readonly editableProperties: string[];
}

export type EnumItem =
| IGateLevel
| ILocation
| IOutlet
| IPartType
| IProductGroup
| IEvidenceYear;

export const EmptyTextualEnumItem =
{
  title: '',
  code: '',
  description: '',
  moniker: '',
};
