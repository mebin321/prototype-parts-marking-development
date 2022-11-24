import _ from 'lodash';
import React, { useCallback, useEffect, useMemo } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { RouteComponentProps, withRouter } from 'react-router-dom';

import { ItemType } from '../../../../models/api/items/part/itemType';
import { IPrototypeExtended } from '../../../../models/api/items/part/prototypeExtended';
import { ISortParameters, NoSort } from '../../../../models/api/sort/sortParameters';
import { IApplicationState, filterPrototypes } from '../../../../store';
import { setPrototypesFilterVisibility } from '../../../../store/filtering/actions';
import { debounceInputChangeEventHandler, tableFilterTimeout } from '../../../../utilities/events';
import PartsExtendedTable from '../PartsExtendedTable';

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    filterVisible: state.filtering.prototypes.visible,
    filter: state.filtering.prototypes.filter,
    sort: state.filtering.prototypes.sort,
    results: state.filtering.prototypes.results,
    error: state.filtering.prototypes.error,
    loading: state.filtering.prototypes.loading,
  };
};

const mapDispatchToProps =
{
  setFilterVisibility: setPrototypesFilterVisibility,
  filterPrototypes: filterPrototypes,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

interface IPrototypesExtendedListProps extends RouteComponentProps, ConnectedProps<typeof connector>
{
}

const PrototypesExtendedList: React.FC<IPrototypesExtendedListProps> = ({
  history,
  filter,
  sort,
  results,
  error,
  loading,
  filterVisible,
  setFilterVisibility,
  filterPrototypes,
}) =>
{
  // initiate loading prototypes on mount if response is not already loaded and stored
  useEffect(() =>
  {
    if (results || error) return;
    filterPrototypes(filter, sort);
  }, [results, error, filter, sort, filterPrototypes]);

  const filterClearHandler = useCallback(() =>
  {
    filterPrototypes({ type: ItemType.Prototype }, NoSort);
  }, [filterPrototypes]);

  const filterChangeHandler = useMemo(() =>
  {
    return debounceInputChangeEventHandler(filter => filterPrototypes(filter, sort), tableFilterTimeout);
  }, [filterPrototypes, sort]);

  const sortChangeHandler = useCallback((sort: ISortParameters) =>
  {
    filterPrototypes(filter, sort);
  }, [filter, filterPrototypes]);

  const pageNumberChangeHandler = useCallback((pageNumber: number) =>
  {
    filterPrototypes(filter, sort, pageNumber);
  }, [filter, sort, filterPrototypes]);

  const prototypeRowClickHandler = useCallback((prototype: IPrototypeExtended) =>
  {
    history.push(`/prototype-sets/${prototype.prototypeSet.id}/prototypes/${prototype.id}`);
  }, [history]);

  return (
    <div style={{ padding: '2em' }}>
      <PartsExtendedTable
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
        onRowClick={prototypeRowClickHandler}
      />
    </div>
  );
};

export default withRouter(connector(PrototypesExtendedList));
