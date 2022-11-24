export interface IPrintItem
{
  id: number;
  ownerId: number;
  customer: string;
  productGroup: string;
  partType: string;
  description: string;
  partCode: string;
  createdAt: Date;
  outlet: string;
  location: string;
  projectNumber: string;
  gateLevel: string;
  materialNumber?: string;
  revisionCode?: string;
}
