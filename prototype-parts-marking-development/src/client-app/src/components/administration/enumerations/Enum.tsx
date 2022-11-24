import React from 'react';
import { Button, Grid } from 'semantic-ui-react';

import { EnumItem, IEnumItemDescriptor } from '../../../models/api/enumerations';
import { prettifyPropertyName } from '../../../utilities/objects';
import PaginationControls from '../../common/ui/PaginationControls';
import DataTable from '../../common/ui/table/DataTable';

interface IEnumProps
{
  name: string;
  items: EnumItem[];
  itemDescriptor: IEnumItemDescriptor;
  pageNumber: number;
  totalPages: number;
  loading?: boolean;
  showAddButton?: boolean;
  onPageNumberChange: (pageNumber: number) => void;
  onAddButtonClick: () => void;
}

const Enum: React.FC<IEnumProps> = ({
  name,
  items,
  itemDescriptor,
  pageNumber,
  totalPages,
  loading,
  showAddButton,
  onPageNumberChange,
  onAddButtonClick,
}) =>
{
  return (
    <Grid container>
      <Grid.Row>
        <DataTable
          columns={itemDescriptor.visibleProperties.map(property =>
          {
            return { name: prettifyPropertyName(property), value: property as keyof EnumItem };
          })}
          data={items.map((item: any) =>
          {
            return { id: item.year ?? item.moniker, ...item };
          })}
          loading={loading}
        />
      </Grid.Row>

      <Grid.Row>
        {showAddButton &&
          <Button
            onClick={onAddButtonClick}
            basic
            size='large'
            icon='plus'
            content={`Add ${name}`}
          />
        }

      </Grid.Row>

      <Grid.Row>
        <PaginationControls
          totalPages={totalPages}
          pageNumber={pageNumber}
          onPageNumberChange={onPageNumberChange}
        />
      </Grid.Row>
    </Grid>
  );
};

export default Enum;
