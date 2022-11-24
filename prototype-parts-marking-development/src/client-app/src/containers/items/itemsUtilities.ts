import { SyntheticEvent } from 'react';
import { toast } from 'react-toastify';
import { SearchProps, SearchResultData } from 'semantic-ui-react';

import agent from '../../api/agent';
import { IEnumerationItemsSearchEndpoint } from '../../api/enumerations';
import { extractErrorDetails, listAll } from '../../api/utilities';
import { IEnumItem } from '../../models/api/enumerations';
import { IEvidenceYear, createFakeEvidenceYear } from '../../models/api/enumerations/evidenceYears';
import { IGateLevel, createFakeGateLevel } from '../../models/api/enumerations/gateLevels';
import { ILocation, createFakeLocation } from '../../models/api/enumerations/locations';
import { IOutlet, createFakeOutlet } from '../../models/api/enumerations/outlets';
import { IPartType, createFakePartType } from '../../models/api/enumerations/parts';
import { IProductGroup, createFakeProductGroup } from '../../models/api/enumerations/productGroups';
import { IPrintItemCreateData } from '../../models/api/print/printItemCreateData';
import { IViewableCustomer } from '../../models/api/projects/viewableCustomer';
import { IViewableProject } from '../../models/api/projects/viewableProject';
import { IUserData } from '../../models/api/users/userData';
import { IViewableUser } from '../../models/api/users/viewableUser';
import IPartCode from '../../models/partCode';
import { limitTextSize as limitTextLength } from '../../utilities/text';
import { toastDistinctError } from '../../utilities/toast';

export const materialNumberInputPattern = /^[a-z0-9]{0,13}$/i;
export const revisionCodeInputPattern = /^[0-9]{0,2}$/;

export enum ItemFormMode
{
  Create,
  View,
  Edit,
}

// LINE_LENGTH * ROWS = 23 * 3 (from containers/formatedTextArea/formatStrategy/formatStrategy.ts)
const ITEM_COMMENT_LIMIT = 69;

export function formatPrototypeNumber(partIndex?: number)
{
  if (!partIndex) return '000';

  return partIndex.toString().padStart(3, '0');
}

export function formatPartCode(partCode: Partial<IPartCode>)
{
  const year = partCode.evidenceYear ?? new Date().getFullYear().toString().substr(-2);
  const uniqueIdentifier = partCode.uniqueIdentifier ?? 'XXXX';
  const codeWithoutPrototypeNumber = [
    partCode.outlet,
    partCode.productGroup,
    partCode.partType,
    year,
    partCode.location,
    uniqueIdentifier,
    partCode.gateLevel].join('.');
  return `${codeWithoutPrototypeNumber}_${formatPrototypeNumber(partCode.numberOfPrototypes)}`;
}

export function prepareItemObjectData(obj: any)
{
  // remove properties required only by Semantic UI Search component to show results
  delete obj.owner.title;
  delete obj.owner.description;

  obj.evidenceYearValue = new Date().getFullYear().toString().substr(-2);
}

export function formatUser(user: IUserData | undefined | null)
{
  if (!user) return '';
  if (!user.id && !user.username && !user.name && !user.email) return '';

  return `${user.name} (${user.username})`;
}

export function convertUserForSearchInput(user: IUserData | undefined): IViewableUser | undefined
{
  if (!user) return undefined;

  return { ...user, title: user.username, description: user.email };
}

export function generateUserSearchHandler(setValues: (value: IViewableUser[]) => void)
{
  return async (_event: SyntheticEvent, data: SearchProps) =>
  {
    if (data.value === undefined)
    {
      return;
    }

    try
    {
      const response = await agent.Users.list({ search: data.value, isActive: true });
      const responseUsers = response?.items ?? [];
      const newUsers: IViewableUser[] = [];
      for (const user of responseUsers)
      {
        if (user)
        {
          newUsers.push(convertUserForSearchInput(user)!);
        }
      }

      setValues(newUsers);
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t get users:', extractErrorDetails(error));
      setValues([]);
    }
  };
}

export function generateEnumItemSearchHandler<T extends IEnumItem>(
  endpoint: IEnumerationItemsSearchEndpoint<T>,
  setValues: (value: T[]) => void
)
{
  return async (_event: SyntheticEvent, data: SearchProps) =>
  {
    try
    {
      const items = await listAll((page: number) => endpoint.searchItems(data.value, page));
      setValues(items ?? []);
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t get items of enumeration:', extractErrorDetails(error));
      setValues([]);
    }
  };
}

export function generateProductGroupSearchHandler(
  outletMoniker: string,
  setValues: (value: IProductGroup[]) => void
)
{
  return async (_event: SyntheticEvent, data: SearchProps) =>
  {
    try
    {
      if (!outletMoniker)
      {
        setValues([]);
        return;
      }

      const permittedItems =
          await agent.Enumerations.Outlets.listPermittedProductGroups(outletMoniker, data.value);
      setValues(permittedItems ?? []);
    }
    catch (error)
    {
      toastDistinctError(`Couldn't get permitted parts for outlet ${outletMoniker}:`, extractErrorDetails(error));
      setValues([]);
    }
  };
}

export function generatePartsSearchHandler(
  productGroupMoniker: string,
  setValues: (value: IPartType[]) => void
)
{
  return async (_event: SyntheticEvent, data: SearchProps) =>
  {
    try
    {
      if (!productGroupMoniker)
      {
        setValues([]);
        return;
      }

      const permittedItems =
          await agent.Enumerations.ProductGroups.listPermittedParts(productGroupMoniker, data.value);
      setValues(permittedItems ?? []);
    }
    catch (error)
    {
      toastDistinctError(
        `Couldn't get permitted parts for product group ${productGroupMoniker}:`, extractErrorDetails(error));
      setValues([]);
    }
  };
}

export function generateCustomersSearchHandler(
  setValues: (value: IViewableCustomer[]) => void
)
{
  return async (_event: SyntheticEvent, data: SearchProps) =>
  {
    try
    {
      const searchText = data.value ?? '';
      const items = await agent.GlobalProjects.ListCustomers();
      const sortedItems = items.sort();
      const customers = sortedItems.filter(cus => cus.toLowerCase().includes(searchText.toLowerCase()));
      const customersToShow = customers.map(cus => ({ title: cus }));
      setValues(customersToShow ?? []);
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t get customers:', extractErrorDetails(error));
      setValues([]);
    }
  };
}

export function generateProjectsSearchHandler(
  setValues: (value: IViewableProject[]) => void,
  customer: string | undefined
)
{
  return async (_event: SyntheticEvent, data: SearchProps) =>
  {
    try
    {
      const projects = await agent.GlobalProjects.ListProjects(undefined, customer, data.value);
      const projectsToShow = projects.map(proj => ({
        title: proj.projectNumber,
        price: proj.customer,
        description: proj.description,
      }));
      setValues(projectsToShow ?? []);
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t get projects:', extractErrorDetails(error));
      setValues([]);
    }
  };
}

export function generateEnumItemResultSelectHandler<T extends IEnumItem>(
  setCurrentValue: (value: string) => void,
  setLocalStorageValue: (value: T) => void
)
{
  return (_event: SyntheticEvent, data: SearchResultData) =>
  {
    const item: T = data.result;
    setCurrentValue(item?.code ?? '');
    setLocalStorageValue(item);
  };
}

export function generateOutletResultSelectHandler(
  setCurrentValue: (value: string) => void,
  setLocalStorageValue: (value: IOutlet) => void,
  setPermittedProductGroups: (value: IOutlet[]) => void
)
{
  return (_event: SyntheticEvent, data: SearchResultData) =>
  {
    const item: IOutlet = data.result;
    setCurrentValue(item?.code ?? '');
    setLocalStorageValue(item);

    if (!item)
    {
      setPermittedProductGroups([]);
      return;
    }

    agent.Enumerations.Outlets.listPermittedProductGroups(item.moniker)
      .then(permittedProductGroups => setPermittedProductGroups(permittedProductGroups))
      .catch(error =>
      {
        toastDistinctError(
          `Couldn't get permitted product groups for outlet ${item.moniker}:`, extractErrorDetails(error));
        setPermittedProductGroups([]);
      });
  };
}

export function generateProductGroupResultSelectHandler(
  setCurrentValue: (value: string) => void,
  setLocalStorageValue: (value: IProductGroup) => void,
  setPermittedParts: (value: IProductGroup[]) => void
)
{
  return (_event: SyntheticEvent, data: SearchResultData) =>
  {
    const item: IProductGroup = data.result;
    setCurrentValue(item?.code ?? '');
    setLocalStorageValue(item);

    if (!item)
    {
      setPermittedParts([]);
      return;
    }

    agent.Enumerations.ProductGroups.listPermittedParts(item.moniker)
      .then(permittedParts => setPermittedParts(permittedParts))
      .catch(error =>
      {
        toastDistinctError(
          `Couldn't get permitted parts for product group ${item.moniker}:`, extractErrorDetails(error));
        setPermittedParts([]);
      });
  };
}

export function getInitialEvidenceYear(mode: ItemFormMode, year?: number, code?: string, defaultValue?: IEvidenceYear)
{
  return mode === ItemFormMode.Create ? defaultValue : createFakeEvidenceYear(year, code);
}

export function getInitialGateLevel(mode: ItemFormMode, title?: string, code?: string, defaultValue?: IGateLevel)
{
  return mode === ItemFormMode.Create ? defaultValue : createFakeGateLevel(title, code);
}

export function getInitialLocation(mode: ItemFormMode, title?: string, code?: string, defaultValue?: ILocation)
{
  return mode === ItemFormMode.Create ? defaultValue : createFakeLocation(title, code);
}

export function getInitialOutlet(mode: ItemFormMode, title?: string, code?: string, defaultValue?: IOutlet)
{
  return mode === ItemFormMode.Create ? defaultValue : createFakeOutlet(title, code);
}

export function getInitialPartType(mode: ItemFormMode, title?: string, code?: string, defaultValue?: IPartType)
{
  return mode === ItemFormMode.Create ? defaultValue : createFakePartType(title, code);
}

export function getInitialProductGroup(mode: ItemFormMode, title?: string, code?: string, defaultValue?: IProductGroup)
{
  return mode === ItemFormMode.Create ? defaultValue : createFakeProductGroup(title, code);
}

export async function addToPrintList(printLabels: IPrintItemCreateData[], toastClickHandler: () => void)
{
  try
  {
    await agent.PrintingLabels.create(printLabels);
    toast.success(`Successfully added ${printLabels.length}
       item${printLabels.length > 1 ? 's' : ''} to print list`,
    { autoClose: 5000, closeOnClick: true, onClick: toastClickHandler });
  }
  catch (error)
  {
    toast.error(`Couldn't add items to print list: ${extractErrorDetails(error)}`);
  }
}

export function getItemCommentForTableListing(item: { comment: string })
{
  return limitTextLength(item.comment, ITEM_COMMENT_LIMIT);
}
