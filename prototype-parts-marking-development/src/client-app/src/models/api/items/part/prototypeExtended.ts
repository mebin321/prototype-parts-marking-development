import { IPrototype } from './prototype';
import { IPrototypeSet } from './set/prototypeSet';

export interface IPrototypeExtended extends Omit<IPrototype, 'prototypeSetId'>
{
  prototypeSet: IPrototypeSet;
}
