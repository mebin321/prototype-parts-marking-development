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

interface IPrototypesListProps
{
  prototypeSet: IPrototypeSet;
  onPrototypeRowClick?: (prototype: IPrototype) => void;
  onPrototypesSelectionChange?: (selection: IPrototype[]) => void;
}

const PrototypesList: React.FC<IPrototypesListProps> = ({
  prototypeSet,
  onPrototypesSelectionChange,
  onPrototypeRowClick,
}) =>
{
  const [prototypes, setPrototypes] = useState<IPrototypesResponse>();
  const [sort, setSort] = useState<ISortParameters>({ sortBy: 'index', sortDirection: SortDirection.Ascending });
  const [pageNumber, setPageNumber] = useState(1);

  useEffect(() =>
  {
    const filter: IPrototypeSetItemsFilter = { type: ItemType.Prototype };
    agent.PrototypeSets.listItems(prototypeSet.id, filter, sort, pageNumber)
      .then(response => setPrototypes(response))
      .catch(error => toastDistinctError('Couldn\'t list prototypes:', extractErrorDetails(error)));
  }, [prototypeSet, sort, pageNumber]);

  return (
    <div style={{ padding: '2em' }}>
      {prototypes &&
      <PartsTable
        prototypeSet={prototypeSet}
        data={prototypes}
        sort={sort}
        onSortChange={setSort}
        onPageNumberChange={setPageNumber}
        onRowsSelectionChange={onPrototypesSelectionChange}
        onPrototypeRowClick={onPrototypeRowClick}
      />}
    </div>
  );
};

export default PrototypesList;
