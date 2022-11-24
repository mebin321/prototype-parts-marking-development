import { IUserData } from '../../../users/userData';

export interface IPrototypeVariant
{
  id: number;
  prototypeId: number;
  version: number;
  comment: string;
  createdAt: Date;
  createdBy: IUserData;
  modifiedAt: Date;
  modifiedBy: IUserData;
  deletedAt: Date;
  deletedBy: IUserData;
}
