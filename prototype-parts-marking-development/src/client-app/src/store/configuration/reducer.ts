import { updateObjectProperty } from '../utilities';
import
{
  ConfigurationActionTypes,
  IConfigurationState,
  SET_ITEMS_CONFIGURATION,
  SET_TABLE_SETTINGS,
} from './types';

const initialState: IConfigurationState =
{
  packages: { tableSettings: { visibleColumns: [] }, itemOptions: {} },
  prototypeSets: { tableSettings: { visibleColumns: [] }, itemOptions: {} },
  prototypes: { tableSettings: { visibleColumns: [] }, itemOptions: {} },
  variants: { tableSettings: { visibleColumns: [] }, itemOptions: {} },
};

// eslint-disable-next-line default-param-last
function reducer(state = initialState, action: ConfigurationActionTypes)
{
  switch (action.type)
  {
    case SET_TABLE_SETTINGS:
      return updateObjectProperty(state, action.itemType,
        {
          tableSettings: action.configuration,
        }
      );
    case SET_ITEMS_CONFIGURATION:
      return updateObjectProperty(state, action.itemType,
        {
          itemOptions: action.configuration,
        }
      );
    default:
      return state;
  }
}

export default reducer;
