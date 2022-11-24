import _ from 'lodash';
import React, { useCallback, useEffect, useMemo } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { RouteComponentProps, withRouter } from 'react-router-dom';

import { IPackage } from '../../../models/api/items/package/package';
import { ISortParameters, NoSort } from '../../../models/api/sort/sortParameters';
import { IApplicationState, filterPackages } from '../../../store';
import { setPackagesFilterVisibility } from '../../../store/filtering/actions';
import { debounceInputChangeEventHandler, tableFilterTimeout } from '../../../utilities/events';
import PackagesTable from './PackagesTable';

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    filterVisible: state.filtering.packages.visible,
    filter: state.filtering.packages.filter,
    sort: state.filtering.packages.sort,
    results: state.filtering.packages.results,
    error: state.filtering.packages.error,
    loading: state.filtering.packages.loading,
  };
};

const mapDispatchToProps =
{
  setFilterVisibility: setPackagesFilterVisibility,
  filterPackages: filterPackages,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

interface IPackagesListProps extends RouteComponentProps, ConnectedProps<typeof connector>
{
}

const PackagesList: React.FC<IPackagesListProps> = ({
  history,
  filter,
  sort,
  results,
  error,
  loading,
  filterVisible,
  setFilterVisibility,
  filterPackages,
}) =>
{
  // initiate loading packages on mount if response is not already loaded and stored
  useEffect(() =>
  {
    if (results || error) return;
    filterPackages(filter, sort);
  }, [results, error, filter, sort, filterPackages]);

  const filterClearHandler = useCallback(() =>
  {
    filterPackages({}, NoSort);
  }, [filterPackages]);

  const filterChangeHandler = useMemo(() =>
  {
    return debounceInputChangeEventHandler(filter => filterPackages(filter, sort), tableFilterTimeout);
  }, [filterPackages, sort]);

  const sortChangeHandler = useCallback((sort: ISortParameters) =>
  {
    filterPackages(filter, sort);
  }, [filter, filterPackages]);

  const pageNumberChangeHandler = useCallback((pageNumber: number) =>
  {
    filterPackages(filter, sort, pageNumber);
  }, [filter, sort, filterPackages]);

  const packageRowClickHandler = useCallback((pkg: IPackage) =>
  {
    history.push(`/packages/${pkg.id}`);
  }, [history]);

  return (
    <div style={{ padding: '2em' }}>
      <PackagesTable
        filtered
        data={results}
        filter={filter}
        sort={sort}
        loading={loading}
        error={error}
        filterVisible={filterVisible}
        onFilterVisibilityChange={setFilterVisibility}
        onFilterClear={filterClearHandler}
        onPageNumberChange={pageNumberChangeHandler}
        onFilterChange={filterChangeHandler}
        onSortChange={sortChangeHandler}
        onRowClick={packageRowClickHandler}
      />
    </div>
  );
};

export default withRouter(connector(PackagesList));
