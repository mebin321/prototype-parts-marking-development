import { IUserData } from '../../users/userData';

export interface IPrototype
{
  id: number;
  prototypeSetId: number;
  type: string;
  partTypeCode: string;
  partTypeTitle: string;
  index: number;
  materialNumber: string;
  revisionCode: string;
  owner: IUserData;
  comment: string;
  createdAt: Date;
  createdBy: IUserData;
  modifiedAt: Date;
  modifiedBy: IUserData;
  deletedAt: Date;
  deletedBy: IUserData;
}
