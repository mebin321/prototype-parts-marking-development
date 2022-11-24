import { Action } from 'redux';

import { IItemOptions } from '../../models/api/items/itemOptions';

export interface ITableDisplaySettings
{
  visibleColumns: string[];
}

export interface IConfigurationSubstate
{
  tableSettings: ITableDisplaySettings;
  itemOptions: IItemOptions;
}

export interface IConfigurationState
{
  packages: IConfigurationSubstate;
  prototypeSets: IConfigurationSubstate;
  prototypes: IConfigurationSubstate;
  variants: IConfigurationSubstate;
}

export type ConfigurationItem = keyof IConfigurationState;

export const SET_TABLE_SETTINGS = 'SET_TABLE_SETTINGS';
export const SET_ITEMS_CONFIGURATION = 'SET_ITEMS_CONFIGURATION';

export interface ISetTableSettingsAction extends Action<typeof SET_TABLE_SETTINGS>
{
  itemType: ConfigurationItem;
  configuration: ITableDisplaySettings;
}

export interface ISetItemsConfigurationAction extends Action<typeof SET_ITEMS_CONFIGURATION>
{
  itemType: ConfigurationItem;
  configuration: IItemOptions;
}

export type ConfigurationActionTypes = ISetTableSettingsAction | ISetItemsConfigurationAction;
