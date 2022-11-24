import _ from 'lodash';
import { Dispatch, SetStateAction } from 'react';
import { DropdownItemProps } from 'semantic-ui-react/dist/commonjs/modules/Dropdown/DropdownItem';

import agent from '../../api/agent';
import { listAll } from '../../api/utilities';
import { IDataTableColumn } from '../../components/common/ui/table/dataTableColumn';
import
{
  DataTableDateTimeFilterType,
  DataTableNumberFilterType,
  DataTableSelectFilterType,
  DataTableTextFilterType,
  DropdownValue,
  IDataTableDateTimeFilterSpecification,
  IDataTableNumberFilterSpecification,
  IDataTableSelectDynamicFilterSpecification,
  IDataTableSelectStaticFilterSpecification,
  IDataTableTextFilterSpecification,
} from '../../components/common/ui/table/dataTableFilterSpecification';
import
{
  isFilterValueEmpty,
  updateFilterMultiValueProperty,
  updateFilterRangeProperties,
  updateFilterSingleValueProperty,
} from '../../components/common/ui/table/dataTableFilterUtilities';
import { IPaginatedResponse } from '../../models/api/paginatedResponse';
import
{
  isItemUniqueIdentifier,
  isMaterialNumber,
  isProjectSearchQueryValidator,
  isRevisionCode,
} from '../../utilities/validation/validators';
import { materialNumberInputPattern, revisionCodeInputPattern } from './itemsUtilities';

const itemUniqueIdentifierInputPattern = /^[a-z0-9]{0,4}$/i;

type ItemActiveStateType =
{
  readonly Yes: string;
  readonly No: string;
};

export const ItemActiveState: ItemActiveStateType =
{
  Yes: 'Yes',
  No: 'No',
};

interface ITitledItem
{
  title: string;
}

export function createTextFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableTextFilterSpecification<TFilter>
{
  return {
    type: DataTableTextFilterType,
    getter: filter => filter[property] as any,
    setter: (value: string) => setFilter(prevFilter =>
      updateFilterSingleValueProperty(prevFilter, property, value)),
  };
}

export function createItemUniqueIdentifierFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableTextFilterSpecification<TFilter>
{
  const filter = createTextFilter(property, setFilter);
  filter.pattern = itemUniqueIdentifierInputPattern;
  filter.validator = isItemUniqueIdentifier({ message: 'Invalid identifier (no results will be listed)' });

  return filter;
}

export function createMaterialNumberFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableTextFilterSpecification<TFilter>
{
  const filter = createTextFilter(property, setFilter);
  filter.pattern = materialNumberInputPattern;
  filter.validator = isMaterialNumber({ message: 'Invalid material number (no results will be listed)' });
  filter.setter = (value: string) => setFilter(prevFilter =>
    updateFilterMultiValueProperty(prevFilter, property, value));

  return filter;
}

export function createRevisionCodeFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableTextFilterSpecification<TFilter>
{
  const filter = createTextFilter(property, setFilter);
  filter.pattern = revisionCodeInputPattern;
  filter.validator = isRevisionCode({ message: 'Invalid revision code (no results will be listed)' });
  filter.setter = (value: string) => setFilter(prevFilter =>
    updateFilterMultiValueProperty(prevFilter, property, value));

  return filter;
}

export function createNumberRangeFilter<TFilter extends object>(
  lowerBoundProperty: keyof TFilter,
  upperBoundProperty: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableNumberFilterSpecification<TFilter>
{
  return {
    type: DataTableNumberFilterType,
    range: true,
    getter: filter =>
    {
      return { lowerBound: filter[lowerBoundProperty] as any, upperBound: filter[upperBoundProperty] as any };
    },
    setter: value => setFilter(prevFilter =>
      updateFilterRangeProperties(prevFilter, lowerBoundProperty, upperBoundProperty, value)),
  };
}

export function createYearsRangeFilter<TFilter extends object>(
  lowerBoundProperty: keyof TFilter,
  upperBoundProperty: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableNumberFilterSpecification<TFilter>
{
  const filter = createNumberRangeFilter(lowerBoundProperty, upperBoundProperty, setFilter);
  filter.min = 1870;
  filter.placeholder = 'YYYY';

  return filter;
}

export function createDateTimeRangeFilter<TFilter extends object>(
  lowerBoundProperty: keyof TFilter,
  upperBoundProperty: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableDateTimeFilterSpecification<TFilter>
{
  return {
    type: DataTableDateTimeFilterType,
    getter: filter =>
    {
      return { lowerBound: filter[lowerBoundProperty] as any, upperBound: filter[upperBoundProperty] as any };
    },
    setter: value => setFilter(prevFilter =>
      updateFilterRangeProperties(prevFilter, lowerBoundProperty, upperBoundProperty, value)),
  };
}

export function createUserFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableSelectDynamicFilterSpecification<TFilter>
{
  return {
    type: DataTableSelectFilterType,
    multiple: true,
    onSearch: value =>
      listAll((page: number) => agent.Users.list({ search: value }, page))
        .then(users => users.map(user => { return { text: user.name, value: user.id }; })),
    getter: filter => filter[property] as any,
    setter: (value: DropdownValue) => setFilter(prevFilter =>
      updateFilterMultiValueProperty(prevFilter, property, value)),
  };
}

export function createEnumFilter<TFilter extends object, TEnumItem extends ITitledItem>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>,
  fetchItems: (text: string, page: number) => Promise<IPaginatedResponse<TEnumItem>>
) : IDataTableSelectDynamicFilterSpecification<TFilter>
{
  return {
    type: DataTableSelectFilterType,
    multiple: true,
    onSearch: value =>
      listAll((page: number) => fetchItems(value, page))
        .then(items => items.map(item => { return { text: item.title, value: item.title }; })),
    getter: filter => filter[property] as any,
    setter: (value: DropdownValue) => setFilter(prevFilter =>
      updateFilterMultiValueProperty(prevFilter, property, value)),
  };
}

export function createGateLevelsEnumFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableSelectDynamicFilterSpecification<TFilter>
{
  return createEnumFilter(property, setFilter, (text, page) => agent.Enumerations.GateLevels.searchItems(text, page));
}

export function createLocationsEnumFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableSelectDynamicFilterSpecification<TFilter>
{
  return createEnumFilter(property, setFilter, (text, page) => agent.Enumerations.Locations.searchItems(text, page));
}

export function createOutletsEnumFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableSelectDynamicFilterSpecification<TFilter>
{
  return createEnumFilter(property, setFilter, (text, page) => agent.Enumerations.Outlets.searchItems(text, page));
}

export function createPartsEnumFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableSelectDynamicFilterSpecification<TFilter>
{
  return createEnumFilter(property, setFilter, (text, page) => agent.Enumerations.Parts.searchItems(text, page));
}

export function createProductGroupsEnumFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableSelectDynamicFilterSpecification<TFilter>
{
  return createEnumFilter(property, setFilter, (text, page) =>
    agent.Enumerations.ProductGroups.searchItems(text, page));
}

export function createScrappedItemFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableSelectStaticFilterSpecification<TFilter>
{
  return {
    options:
    [
      { text: ItemActiveState.Yes, value: ItemActiveState.Yes },
      { text: ItemActiveState.No, value: ItemActiveState.No },
    ],
    type: DataTableSelectFilterType,
    getter: filter => convertTristateBooleanToYesNo(filter[property] as any),
    setter: (value: DropdownValue) => setFilter(prevFilter =>
      updateFilterSingleValueProperty(prevFilter, property, convertYesNoToTristateBoolean(value))),
  };
}

export function createCustomerFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>,
  customers: DropdownItemProps[]
) : IDataTableSelectStaticFilterSpecification<TFilter>
{
  return {
    type: DataTableSelectFilterType,
    multiple: true,
    options: customers,
    getter: filter => filter[property] as any,
    setter: (value: DropdownValue) => setFilter(prevFilter =>
      updateFilterMultiValueProperty(prevFilter, property, value)),
  };
}

export function createProjectNumberFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableSelectDynamicFilterSpecification<TFilter>
{
  return {
    type: DataTableSelectFilterType,
    multiple: true,
    onSearch: value =>
      agent.GlobalProjects.ListProjects(undefined, undefined, value).then(projects =>
        projects.map(project => { return { text: project.projectNumber, value: project.projectNumber }; })),
    getter: filter => filter[property] as any,
    setter: (value: DropdownValue) => setFilter(prevFilter =>
      updateFilterMultiValueProperty(prevFilter, property, value)),
  };
}

export function createProjectNameFilter<TFilter extends object>(
  property: keyof TFilter,
  setFilter: Dispatch<SetStateAction<TFilter>>
) : IDataTableSelectDynamicFilterSpecification<TFilter>
{
  return {
    type: DataTableSelectFilterType,
    multiple: true,
    allowAdditions: true,
    validator: isProjectSearchQueryValidator,
    onSearch: value =>
      agent.GlobalProjects.ListProjects(undefined, undefined, value).then(projects =>
        projects.map(project => { return { text: project.description, value: project.description }; })),
    getter: filter => filter[property] as any,
    setter: (value: DropdownValue) => setFilter(prevFilter =>
      updateFilterMultiValueProperty(prevFilter, property, value)),
  };
}

export async function fetchCustomersAsDropdownOptions()
{
  return await agent.GlobalProjects.ListCustomers()
    .then(items => items.map(item => { return { text: item, value: item }; }));
}

function convertYesNoToTristateBoolean(value: DropdownValue)
{
  switch (value)
  {
    case ItemActiveState.Yes:
      return true;
    case ItemActiveState.No:
      return false;
    default:
      return undefined;
  }
}

function convertTristateBooleanToYesNo(value: boolean | undefined)
{
  switch (value)
  {
    case true:
      return ItemActiveState.Yes;
    case false:
      return ItemActiveState.No;
    default:
      return '';
  }
}

export function clearHiddenColumnsFilter<TData extends object, TFilter extends object>(
  filter: TFilter,
  allColumns: IDataTableColumn<TData, TFilter>[],
  oldVisibleColumns: string[],
  newVisibleColumns: string[]
)
{
  const hiddenColumns = _.difference(oldVisibleColumns, newVisibleColumns);
  for (const columnName of hiddenColumns)
  {
    const column = allColumns.find(column => column.name === columnName);
    if (!column) continue;

    // clear filter on column that was hidden
    if (!isFilterValueEmpty(filter, column.filter?.getter))
    {
      column.filter?.setter(undefined as any);
    }
  }
}
