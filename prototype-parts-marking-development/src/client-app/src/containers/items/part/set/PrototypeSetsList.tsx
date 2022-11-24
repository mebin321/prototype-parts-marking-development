import _ from 'lodash';
import React, { useCallback, useEffect, useMemo } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { RouteComponentProps, withRouter } from 'react-router-dom';

import { IPrototypeSet } from '../../../../models/api/items/part/set/prototypeSet';
import { ISortParameters, NoSort } from '../../../../models/api/sort/sortParameters';
import { IApplicationState, filterPrototypeSets } from '../../../../store';
import { setPrototypeSetsFilterVisibility } from '../../../../store/filtering/actions';
import { debounceInputChangeEventHandler, tableFilterTimeout } from '../../../../utilities/events';
import PrototypeSetsTable from './PrototypeSetsTable';

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    filterVisible: state.filtering.prototypeSets.visible,
    filter: state.filtering.prototypeSets.filter,
    sort: state.filtering.prototypeSets.sort,
    results: state.filtering.prototypeSets.results,
    error: state.filtering.prototypeSets.error,
    loading: state.filtering.prototypeSets.loading,
  };
};

const mapDispatchToProps =
{
  setFilterVisibility: setPrototypeSetsFilterVisibility,
  filterPrototypeSets: filterPrototypeSets,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

interface IPrototypeSetsListProps extends RouteComponentProps, ConnectedProps<typeof connector>
{
}

const PrototypeSetsList: React.FC<IPrototypeSetsListProps> = ({
  history,
  filter,
  sort,
  results,
  error,
  loading,
  filterVisible,
  setFilterVisibility,
  filterPrototypeSets,
}) =>
{
  // initiate loading prototype sets on mount if response is not already loaded and stored
  useEffect(() =>
  {
    if (results || error) return;
    filterPrototypeSets(filter, sort);
  }, [results, error, filter, sort, filterPrototypeSets]);

  const filterClearHandler = useCallback(() =>
  {
    filterPrototypeSets({}, NoSort);
  }, [filterPrototypeSets]);

  const filterChangeHandler = useMemo(() =>
  {
    return debounceInputChangeEventHandler(filter => filterPrototypeSets(filter, sort), tableFilterTimeout);
  }, [filterPrototypeSets, sort]);

  const sortChangeHandler = useCallback((sort: ISortParameters) =>
  {
    filterPrototypeSets(filter, sort);
  }, [filter, filterPrototypeSets]);

  const pageNumberChangeHandler = useCallback((pageNumber: number) =>
  {
    filterPrototypeSets(filter, sort, pageNumber);
  }, [filter, sort, filterPrototypeSets]);

  const prototypeSetRowClickHandler = useCallback((prototypeSet: IPrototypeSet) =>
  {
    history.push(`/prototype-sets/${prototypeSet.id}`);
  }, [history]);

  return (
    <div style={{ padding: '2em' }}>
      <PrototypeSetsTable
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
        onRowClick={prototypeSetRowClickHandler}
      />
    </div>
  );
};

export default withRouter(connector(PrototypeSetsList));
