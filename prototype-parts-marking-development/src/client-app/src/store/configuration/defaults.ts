import { ITableDisplaySettings } from './types';

export const DefaultPackagesTableSettings: ITableDisplaySettings =
{
  visibleColumns:
  [
    'Part Code',
    'Outlet',
    'Product Group',
    'Part Type',
    'Evidence Year',
    'Location',
    'Identifier',
    'Gate Level',
    'Initial Count',
    'Actual Count',
    'Is Active',
  ],
};

export const DefaultPrototypeSetsTableSettings: ITableDisplaySettings =
{
  visibleColumns:
  [
    'Part Code',
    'Outlet',
    'Product Group',
    'Evidence Year',
    'Location',
    'Identifier',
    'Gate Level',
    'Is Active',
  ],
};

export const DefaultPrototypesTableSettings: ITableDisplaySettings =
{
  visibleColumns:
  [
    'Part Code',
    'Outlet',
    'Product Group',
    'Part Type',
    'Evidence Year',
    'Location',
    'Identifier',
    'Gate Level',
    'Part Number',
    'Is Active',
  ],
};

export const DefaultVariantsTableSettings: ITableDisplaySettings =
{
  visibleColumns:
  [
    'Part Code',
    'Version',
    'Comment',
    'Is Active',
  ],
};
