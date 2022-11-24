import React, { useEffect, useState } from 'react';

import agent from '../../../../api/agent';
import { extractErrorDetails } from '../../../../api/utilities';
import { ItemType } from '../../../../models/api/items/part/itemType';
import { IPrototype } from '../../../../models/api/items/part/prototype';
import { IPrototypeSet } from '../../../../models/api/items/part/set/prototypeSet';
import { IPrototypeSetItemsFilter } from '../../../../models/api/items/part/set/prototypeSetItemsFilter';
import { IPrototypesResponse } from '../../../../models/api/responses';
import { SortDirection } from '../../../../models/api/sort/sortDirection';
import { ISortParameters } from '../../../../models/api/sort/sortParameters';
import { toastDistinctError } from '../../../../utilities/toast';
import PartsTable from '../PartsTable';

interface IComponentsListProps
{
  prototypeSet: IPrototypeSet;
  partIndex: number;
  onComponentRowClick?: (component: IPrototype) => void;
  onComponentsSelectionChange?: (selection: IPrototype[]) => void;
}

const ComponentsList: React.FC<IComponentsListProps> = ({
  prototypeSet,
  partIndex,
  onComponentRowClick,
  onComponentsSelectionChange,
}) =>
{
  const [components, setComponents] = useState<IPrototypesResponse>();
  const [sort, setSort] = useState<ISortParameters>({ sortBy: 'index', sortDirection: SortDirection.Ascending });
  const [pageNumber, setPageNumber] = useState(1);

  useEffect(() =>
  {
    const filter: IPrototypeSetItemsFilter = { type: ItemType.Component, index: partIndex };
    agent.PrototypeSets.listItems(prototypeSet.id, filter, sort, pageNumber)
      .then(response => setComponents(response))
      .catch(error => toastDistinctError('Couldn\'t list components:', extractErrorDetails(error)));
  }, [prototypeSet, partIndex, sort, pageNumber]);

  return (
    <div style={{ padding: '2em' }}>
      {components &&
      <PartsTable
        prototypeSet={prototypeSet}
        data={components}
        sort={sort}
        onSortChange={setSort}
        onPageNumberChange={setPageNumber}
        onRowsSelectionChange={onComponentsSelectionChange}
        onPrototypeRowClick={onComponentRowClick}
      />}
    </div>
  );
};

export default ComponentsList;
