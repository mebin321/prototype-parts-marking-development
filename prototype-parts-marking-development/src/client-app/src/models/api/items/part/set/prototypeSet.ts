import { IUserData } from '../../../users/userData';

export interface IPrototypeSet
{
  id: number;
  outletCode: string;
  outletTitle: string;
  productGroupCode: string;
  productGroupTitle: string;
  gateLevelCode: string;
  gateLevelTitle: string;
  evidenceYearCode: string;
  evidenceYearTitle: number;
  locationCode: string;
  locationTitle: string;
  setIdentifier: string;
  customer: string;
  project: string;
  projectNumber: string;
  createdAt: Date;
  createdBy: IUserData;
  modifiedAt: Date;
  modifiedBy: IUserData;
  deletedAt: Date;
  deletedBy: IUserData;
}
