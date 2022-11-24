import _ from 'lodash';
import React, { useCallback, useEffect, useMemo } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { RouteComponentProps, withRouter } from 'react-router-dom';

import { ItemType } from '../../../../models/api/items/part/itemType';
import { IPrototypeExtended } from '../../../../models/api/items/part/prototypeExtended';
import { ISortParameters, NoSort } from '../../../../models/api/sort/sortParameters';
import { IApplicationState, filterComponents } from '../../../../store';
import { setComponentsFilterVisibility } from '../../../../store/filtering/actions';
import { debounceInputChangeEventHandler, tableFilterTimeout } from '../../../../utilities/events';
import PartsExtendedTable from '../PartsExtendedTable';

const mapStateToProps = (state: IApplicationState) =>
{
  return {
    filterVisible: state.filtering.components.visible,
    filter: state.filtering.components.filter,
    sort: state.filtering.components.sort,
    results: state.filtering.components.results,
    error: state.filtering.components.error,
    loading: state.filtering.components.loading,
  };
};

const mapDispatchToProps =
{
  setFilterVisibility: setComponentsFilterVisibility,
  filterComponents: filterComponents,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

interface IComponentsExtendedListProps extends RouteComponentProps, ConnectedProps<typeof connector>
{
}

const ComponentsExtendedList: React.FC<IComponentsExtendedListProps> = ({
  history,
  filter,
  sort,
  results,
  error,
  loading,
  filterVisible,
  setFilterVisibility,
  filterComponents,
}) =>
{
  // initiate loading components on mount if response is not already loaded and stored
  useEffect(() =>
  {
    if (results || error) return;
    filterComponents(filter, sort);
  }, [results, error, filter, sort, filterComponents]);

  const filterClearHandler = useCallback(() =>
  {
    filterComponents({ type: ItemType.Component }, NoSort);
  }, [filterComponents]);

  const filterChangeHandler = useMemo(() =>
  {
    return debounceInputChangeEventHandler(filter => filterComponents(filter, sort), tableFilterTimeout);
  }, [filterComponents, sort]);

  const sortChangeHandler = useCallback((sort: ISortParameters) =>
  {
    filterComponents(filter, sort);
  }, [filter, filterComponents]);

  const pageNumberChangeHandler = useCallback((pageNumber: number) =>
  {
    filterComponents(filter, sort, pageNumber);
  }, [filter, sort, filterComponents]);

  const componentRowClickHandler = useCallback((component: IPrototypeExtended) =>
  {
    history.push(`/prototype-sets/${component.prototypeSet.id}/components/${component.id}`);
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
        onRowClick={componentRowClickHandler}
      />
    </div>
  );
};

export default withRouter(connector(ComponentsExtendedList));
