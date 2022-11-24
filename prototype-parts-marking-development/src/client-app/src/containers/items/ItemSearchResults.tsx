import React, { ReactNode, SyntheticEvent, useCallback, useEffect, useMemo } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { Accordion, AccordionPanelProps, AccordionTitleProps, Header, Loader, Message } from 'semantic-ui-react';

import { IPackage } from '../../models/api/items/package/package';
import { IPackageFilter } from '../../models/api/items/package/packageFilter';
import { ItemType } from '../../models/api/items/part/itemType';
import { IPrototypeExtended } from '../../models/api/items/part/prototypeExtended';
import { IPrototypeFilter } from '../../models/api/items/part/prototypeFilter';
import { IPrototypeVariantExtended } from '../../models/api/items/part/variant/prototypeVariantExtended';
import { IPrototypeVariantFilter } from '../../models/api/items/part/variant/prototypeVariantFilter';
import { ISortParameters } from '../../models/api/sort/sortParameters';
import
{
  IApplicationState,
  foldResultsTarget,
  searchPackages,
  searchPrototypes,
  searchVariants,
  unfoldResultsTarget,
} from '../../store';
import { selectIsSearching } from '../../store/search/selectors';
import { SearchTarget } from '../../store/search/types';
import { prettifyPropertyName } from '../../utilities/objects';
import { formatPrototypeNumber } from './itemsUtilities';
import PackagesTable from './package/PackagesTable';
import PartsExtendedTable from './part/PartsExtendedTable';
import VariantsExtendedTable from './part/variant/VariantsExtendedTable';

// format chunk of part code according to the value of one filter property
const formatPartCodeChunk = (filter: string[] | undefined, defaultValue: string) =>
{
  if (filter && filter.length > 0)
  {
    const value = filter[0];
    if (value) return value;
  }

  return defaultValue;
};

// format part code representing the filter created by search
// all code filters (e.g. outletsCodes, locationCodes ...) are taken
// and where value is not set question marks are used - e.g. 13.??.00.??.ZV.????.60_001
const formatPartCode = (filter: IPackageFilter & IPrototypeFilter & IPrototypeVariantFilter) =>
{
  const outlet = formatPartCodeChunk(filter.outletCodes, '??');
  const productGroup = formatPartCodeChunk(filter.productGroupCodes, '??');
  const partType = formatPartCodeChunk(filter.partTypeCodes, '??');
  const evidenceYear = formatPartCodeChunk(filter.evidenceYearCodes, '??');
  const location = formatPartCodeChunk(filter.locationCodes, '??');
  let uniqueIdentifier = '????';
  if (filter.packageIdentifiers)
  {
    uniqueIdentifier = formatPartCodeChunk(filter.packageIdentifiers, '????');
  }
  else if (filter.setIdentifiers)
  {
    uniqueIdentifier = formatPartCodeChunk(filter.setIdentifiers, '????');
  }

  const gateLevel = formatPartCodeChunk(filter.gateLevelCodes, '??');
  let numberOfPrototypes = '???';
  if (filter.initialCounts && filter.initialCounts.length > 0 && filter.initialCounts[0])
  {
    numberOfPrototypes = formatPrototypeNumber(filter.initialCounts[0]);
  }
  else if (filter.indexes && filter.indexes.length > 0 && filter.indexes[0])
  {
    numberOfPrototypes = formatPrototypeNumber(filter.indexes[0]);
  }

  return `${outlet}.${productGroup}.${partType}.${evidenceYear}.` +
    `${location}.${uniqueIdentifier}.${gateLevel}_${numberOfPrototypes}`;
};

// convert filter created by search to textual representation
// includes information what was searched (comment / part code)
const formatFilter = (filter: (IPackageFilter & IPrototypeFilter & IPrototypeVariantFilter) | undefined) =>
{
  if (!filter) return '';

  return filter.search
    ? `comment containing ${filter.search}`
    : `part code ${formatPartCode(filter)}`;
};

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    packages: state.search.packages,
    prototypes: state.search.prototypes,
    variants: state.search.variants,
    isSearching: selectIsSearching(state),
  };
};

const mapDispatchToProps =
{
  searchPackages: searchPackages,
  searchPrototypes: searchPrototypes,
  searchVariants: searchVariants,
  foldResultsTarget: foldResultsTarget,
  unfoldResultsTarget: unfoldResultsTarget,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

interface IItemSearchResultsProps extends RouteComponentProps, ConnectedProps<typeof connector>
{
}

const ItemSearchResults: React.FC<IItemSearchResultsProps> = ({
  history,
  packages,
  prototypes,
  variants,
  isSearching,
  searchPackages,
  searchPrototypes,
  searchVariants,
  foldResultsTarget,
  unfoldResultsTarget,
}) =>
{
  // prepare elements containing error messages from each item type search (if error occurred)
  const errors = useMemo(() =>
  {
    const errors: JSX.Element[] = [];
    if (packages.error)
    {
      errors.push(<p key={'packages'}>{packages.error}</p>);
    }

    if (prototypes.error)
    {
      errors.push(<p key={'prototypes'}>{prototypes.error}</p>);
    }

    if (variants.error)
    {
      errors.push(<p key={'variants'}>{variants.error}</p>);
    }

    return errors;
  }, [packages.error, prototypes.error, variants.error]);

  // format search criteria to be shown in header
  // filters created by search are same for all item types
  // therefore return first non-empty filter
  const formatSearch = () =>
  {
    const packageFilter = formatFilter(packages.filter);
    const prototypeFilter = formatFilter(prototypes.filter);
    const variantsFilter = formatFilter(variants.filter);
    if (packageFilter) return packageFilter;
    if (prototypeFilter) return prototypeFilter;
    if (variantsFilter) return variantsFilter;
    return '';
  };

  // get search data (filter, sort, results, error ...) for given item type
  // this is just a function to map correct property to given item type
  const getTargetSearchData = useCallback((target: SearchTarget) =>
  {
    switch (target)
    {
      case 'packages':
        return packages;
      case 'prototypes':
        return prototypes;
      case 'variants':
        return variants;
    }
  }, [prototypes, packages, variants]);

  // initially (when search is started) all results are folded
  // when search is finished results shall be unfolded if there are results only for one item type
  // otherwise results shall remain folded to give an overview how many items were found for each item type
  const initializeResultsFolding = useCallback(() =>
  {
    const packagesCount = packages.results?.pagination?.totalCount ?? 0;
    const prototypesCount = prototypes.results?.pagination?.totalCount ?? 0;
    const variantsCount = variants.results?.pagination?.totalCount ?? 0;

    if (packagesCount > 0 && prototypesCount <= 0 && variantsCount <= 0)
    {
      unfoldResultsTarget('packages');
    }
    else if (prototypesCount > 0 && packagesCount <= 0 && variantsCount <= 0)
    {
      unfoldResultsTarget('prototypes');
    }
    else if (variantsCount > 0 && packagesCount <= 0 && prototypesCount <= 0)
    {
      unfoldResultsTarget('variants');
    }
  }, [packages.results, prototypes.results, variants.results, unfoldResultsTarget]);

  // initialize search results folding when search is finished
  useEffect(() =>
  {
    // search has not finished yet
    if (packages.loading || prototypes.loading || variants.loading) return;

    // expanded state is set to undefined when user triggers search
    if (packages.expanded !== undefined || prototypes.expanded !== undefined || variants.expanded !== undefined) return;

    initializeResultsFolding();
  },
  [
    packages.loading,
    packages.expanded,
    prototypes.loading,
    prototypes.expanded,
    variants.loading,
    variants.expanded,
    initializeResultsFolding,
  ]);

  // #region result tables event handlers
  const packagesPageChangeHandler = useCallback((pageNumber: number) =>
  {
    if (!packages.filter || !packages.results?.pagination) return;
    searchPackages(packages.filter, packages.sort, pageNumber, packages.results?.pagination.pageSize);
  }, [packages.filter, packages.sort, packages.results, searchPackages]);

  const packagesSortChangeHandler = useCallback((sort: ISortParameters) =>
  {
    searchPackages(packages.filter!, sort);
  }, [packages.filter, searchPackages]);

  const packageRowClickHandler = useCallback((pkg: IPackage) =>
  {
    history.push(`/packages/${pkg.id}`);
  }, [history]);

  const prototypesPageChangeHandler = useCallback((pageNumber: number) =>
  {
    if (!prototypes.filter || !prototypes.results?.pagination) return;
    searchPrototypes(prototypes.filter, prototypes.sort, pageNumber, prototypes.results?.pagination.pageSize);
  }, [prototypes.filter, prototypes.sort, prototypes.results, searchPrototypes]);

  const prototypesSortChangeHandler = useCallback((sort: ISortParameters) =>
  {
    searchPrototypes(prototypes.filter!, sort);
  }, [prototypes.filter, searchPrototypes]);

  const prototypeRowClickHandler = useCallback((prototype: IPrototypeExtended) =>
  {
    switch (prototype.type)
    {
      case ItemType.Prototype:
        history.push(`/prototype-sets/${prototype.prototypeSet.id}/prototypes/${prototype.id}`);
        break;
      case ItemType.Component:
        history.push(`/prototype-sets/${prototype.prototypeSet.id}/components/${prototype.id}`);
        break;
    }
  }, [history]);

  const variantsPageChangeHandler = useCallback((pageNumber: number) =>
  {
    if (!variants.filter || !variants.results?.pagination) return;
    searchVariants(variants.filter, pageNumber, variants.results?.pagination.pageSize);
  }, [variants.filter, variants.results, searchVariants]);

  const variantRowClickHandler = useCallback((variant: IPrototypeVariantExtended) =>
  {
    switch (variant.prototype.type)
    {
      case ItemType.Prototype:
        history.push(`/prototype-sets/${variant.prototype.prototypeSet.id}/prototypes/${variant.prototype.id}`);
        break;
      case ItemType.Component:
        history.push(`/prototype-sets/${variant.prototype.prototypeSet.id}/components/${variant.prototype.id}`);
        break;
    }
  }, [history]);
  // #endregion result tables event handlers

  const createSearchTargetResultsTable = useCallback((target: SearchTarget): ReactNode =>
  {
    switch (target)
    {
      case 'packages':
        return packages.results?.pagination.totalCount
          ? <PackagesTable
              data={packages.results}
              sort={packages.sort}
              loading={packages.loading}
              onPageNumberChange={packagesPageChangeHandler}
              onSortChange={packagesSortChangeHandler}
              onRowClick={packageRowClickHandler}
            />
          : <p style={{ fontSize: '1.2em', paddingLeft: '1em' }}>The search yielded no results.</p>;
      case 'prototypes':
        return prototypes.results?.pagination.totalCount
          ? <PartsExtendedTable
              data={prototypes.results}
              sort={prototypes.sort}
              loading={prototypes.loading}
              onPageNumberChange={prototypesPageChangeHandler}
              onSortChange={prototypesSortChangeHandler}
              onRowClick={prototypeRowClickHandler}
            />
          : <p style={{ fontSize: '1.2em', paddingLeft: '1em' }}>The search yielded no results.</p>;
      case 'variants':
        return variants.results?.pagination.totalCount
          ? <VariantsExtendedTable
              data={variants.results}
              loading={variants.loading}
              onPageNumberChange={variantsPageChangeHandler}
              onRowClick={variantRowClickHandler}
            />
          : <p style={{ fontSize: '1.2em', paddingLeft: '1em' }}>The search yielded no results.</p>;
    }
  },
  [
    packages.sort,
    packages.results,
    packages.loading,
    prototypes.sort,
    prototypes.results,
    prototypes.loading,
    variants.results,
    variants.loading,
    packagesPageChangeHandler,
    packagesSortChangeHandler,
    packageRowClickHandler,
    prototypesPageChangeHandler,
    prototypesSortChangeHandler,
    prototypeRowClickHandler,
    variantsPageChangeHandler,
    variantRowClickHandler,
  ]);

  // create accordion panel (can be expanded/collapsed to show/hide content) for given item type
  // the panel just wraps the results table and adds header which on click expands/collapses the panel
  const createSearchTargetResultsPanel = useCallback((target: SearchTarget): AccordionPanelProps | undefined =>
  {
    const data = getTargetSearchData(target);
    if (!data.filter) return undefined;

    return {
      key: target,
      active: data.expanded,
      title:
      {
        content:
        (
          <Header size='small' style={{ display: 'inline' }}>
            {data.results?.pagination.totalCount ?? 0} {prettifyPropertyName(target)}
          </Header>
        ),
        onClick: (_event: SyntheticEvent, data: AccordionTitleProps) =>
        {
          // toggle search results panel expanded state on click
          if (data.active)
          {
            foldResultsTarget(target);
          }
          else
          {
            unfoldResultsTarget(target);
          }
        },
      },
      content:
      {
        content: createSearchTargetResultsTable(target),
      },
    };
  }, [getTargetSearchData, createSearchTargetResultsTable, foldResultsTarget, unfoldResultsTarget]);

  // prepare search results content based on state
  const content = useMemo(() =>
  {
    // search is still ongoing
    if (isSearching) return <Loader active={isSearching} size='massive' />;

    // error occurred on search of one or more item types
    if (errors.length > 0) return <Message error content={errors} />;

    if (!packages.filter && !prototypes.filter && !variants.filter)
    {
      // not redirected from search input but address was entered directly
      return <Message warning content='Enter something into search box at the top of the page!' />;
    }
    else if (packages.filter && !prototypes.filter && !variants.filter)
    {
      // only packages were searched - don't create panel for each item type results
      return createSearchTargetResultsTable('packages');
    }
    else if (!packages.filter && prototypes.filter && !variants.filter)
    {
      // only prototypes were searched - don't create panel for each item type results
      return createSearchTargetResultsTable('prototypes');
    }
    else if (!packages.filter && !prototypes.filter && variants.filter)
    {
      // only variants were searched - don't create panel for each item type results
      return createSearchTargetResultsTable('variants');
    }
    else
    {
      // multiple item types were searched - create panel for each item type results
      const resultPanels =
      [
        createSearchTargetResultsPanel('packages'),
        createSearchTargetResultsPanel('prototypes'),
        createSearchTargetResultsPanel('variants'),
      ].filter(panel => !!panel).map(panel => panel!);
      return <Accordion fluid panels={resultPanels} />;
    }
  },
  [
    errors,
    packages.filter,
    prototypes.filter,
    variants.filter,
    isSearching,
    createSearchTargetResultsPanel,
    createSearchTargetResultsTable,
  ]);

  return (
    <div style={{ padding: '2em' }}>
      <Header as='h2'>{isSearching ? 'Searching' : 'Search results'} for {formatSearch()}</Header>
      {content}
    </div>
  );
};

export default withRouter(connector(ItemSearchResults));
