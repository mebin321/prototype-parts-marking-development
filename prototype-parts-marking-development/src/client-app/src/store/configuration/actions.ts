import { Dispatch } from 'redux';

import agent from '../../api/agent';
import { IItemOptions } from '../../models/api/items/itemOptions';
import { ConfigurationItem, ITableDisplaySettings, SET_ITEMS_CONFIGURATION, SET_TABLE_SETTINGS } from './types';

export function setTableSettings(itemType: ConfigurationItem, configuration: ITableDisplaySettings)
{
  localStorage.setItem(`configuration.${itemType}.tableSettings`, JSON.stringify(configuration));

  return {
    type: SET_TABLE_SETTINGS,
    itemType: itemType,
    configuration: configuration,
  };
}

function setItemsConfiguration(itemType: ConfigurationItem, configuration: IItemOptions)
{
  return {
    type: SET_ITEMS_CONFIGURATION,
    itemType: itemType,
    configuration: configuration,
  };
}

async function loadPackagesConfiguration(dispatch: Dispatch)
{
  const configuration = await agent.Packages.configuration();
  dispatch(setItemsConfiguration('packages', configuration));
}

async function loadPrototypeSetsConfiguration(dispatch: Dispatch)
{
  const configuration = await agent.PrototypeSets.configuration();
  dispatch(setItemsConfiguration('prototypeSets', configuration));
}

async function loadPrototypesConfiguration(dispatch: Dispatch)
{
  const configuration = await agent.Prototypes.configuration();
  dispatch(setItemsConfiguration('prototypes', configuration));
}

export function loadConfiguration()
{
  return async (dispatch: Dispatch) =>
  {
    loadPackagesConfiguration(dispatch);
    loadPrototypeSetsConfiguration(dispatch);
    loadPrototypesConfiguration(dispatch);
  };
}
