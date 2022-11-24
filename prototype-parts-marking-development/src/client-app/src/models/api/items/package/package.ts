import { IUserData } from '../../users/userData';

export interface IPackage
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
  partTypeCode: string;
  partTypeTitle: string;
  packageIdentifier: string;
  initialCount: number;
  actualCount: number;
  customer: string;
  project: string;
  projectNumber: string;
  owner: IUserData;
  comment: string;
  createdAt: Date;
  createdBy: IUserData;
  modifiedAt: Date;
  modifiedBy: IUserData;
  deletedAt: Date;
  deletedBy: IUserData;
}
