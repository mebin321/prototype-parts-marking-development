import { IPrototypeExtended } from '../prototypeExtended';
import { IPrototypeVariant } from './prototypeVariant';

export interface IPrototypeVariantExtended extends Omit<IPrototypeVariant, 'prototypeId'>
{
  prototype: IPrototypeExtended;
}
