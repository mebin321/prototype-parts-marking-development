import { ItemType } from '../../models/api/items/part/itemType';
import { updateObjectProperty } from '../utilities';
import
{
  FILTER_COMPONENTS_FAILURE,
  FILTER_COMPONENTS_START,
  FILTER_COMPONENTS_SUCCESS,
  FILTER_PACKAGES_FAILURE,
  FILTER_PACKAGES_START,
  FILTER_PACKAGES_SUCCESS,
  FILTER_PROTOTYPES_FAILURE,
  FILTER_PROTOTYPES_START,
  FILTER_PROTOTYPES_SUCCESS,
  FILTER_PROTOTYPE_SETS_FAILURE,
  FILTER_PROTOTYPE_SETS_START,
  FILTER_PROTOTYPE_SETS_SUCCESS,
  FilteringActionTypes,
  IFilteringState,
  SET_COMPONENTS_FILTER_VISIBILITY,
  SET_PACKAGES_FILTER_VISIBILITY,
  SET_PROTOTYPES_FILTER_VISIBILITY,
  SET_PROTOTYPE_SETS_FILTER_VISIBILITY,
} from './types';

const initialState: IFilteringState =
{
  packages: { visible: false, loading: false, filter: {} },
  prototypeSets: { visible: false, loading: false, filter: {} },
  prototypes: { visible: false, loading: false, filter: { type: ItemType.Prototype } },
  components: { visible: false, loading: false, filter: { type: ItemType.Component } },
};

// eslint-disable-next-line default-param-last
function reducer(state = initialState, action: FilteringActionTypes)
{
  switch (action.type)
  {
    case SET_PACKAGES_FILTER_VISIBILITY:
      return updateObjectProperty(state, 'packages',
        {
          visible: action.visible,
        });
    case FILTER_PACKAGES_START:
      return updateObjectProperty(state, 'packages',
        {
          filter: action.filter,
          sort: action.sort,
          error: undefined,
          loading: true,
        });
    case FILTER_PACKAGES_SUCCESS:
      return updateObjectProperty(state, 'packages',
        {
          results: action.data,
          loading: false,
        });
    case FILTER_PACKAGES_FAILURE:
      return updateObjectProperty(state, 'packages',
        {
          results: undefined,
          error: action.error,
          loading: false,
        });
    case SET_PROTOTYPE_SETS_FILTER_VISIBILITY:
      return updateObjectProperty(state, 'prototypeSets',
        {
          visible: action.visible,
        });
    case FILTER_PROTOTYPE_SETS_START:
      return updateObjectProperty(state, 'prototypeSets',
        {
          filter: action.filter,
          sort: action.sort,
          error: undefined,
          loading: true,
        });
    case FILTER_PROTOTYPE_SETS_SUCCESS:
      return updateObjectProperty(state, 'prototypeSets',
        {
          results: action.data,
          loading: false,
        });
    case FILTER_PROTOTYPE_SETS_FAILURE:
      return updateObjectProperty(state, 'prototypeSets',
        {
          results: undefined,
          error: action.error,
          loading: false,
        });
    case SET_PROTOTYPES_FILTER_VISIBILITY:
      return updateObjectProperty(state, 'prototypes',
        {
          visible: action.visible,
        });
    case FILTER_PROTOTYPES_START:
      return updateObjectProperty(state, 'prototypes',
        {
          filter: action.filter,
          sort: action.sort,
          error: undefined,
          loading: true,
        });
    case FILTER_PROTOTYPES_SUCCESS:
      return updateObjectProperty(state, 'prototypes',
        {
          results: action.data,
          loading: false,
        });
    case FILTER_PROTOTYPES_FAILURE:
      return updateObjectProperty(state, 'prototypes',
        {
          results: undefined,
          error: action.error,
          loading: false,
        });
    case SET_COMPONENTS_FILTER_VISIBILITY:
      return updateObjectProperty(state, 'components',
        {
          visible: action.visible,
        });
    case FILTER_COMPONENTS_START:
      return updateObjectProperty(state, 'components',
        {
          filter: action.filter,
          sort: action.sort,
          error: undefined,
          loading: true,
        });
    case FILTER_COMPONENTS_SUCCESS:
      return updateObjectProperty(state, 'components',
        {
          results: action.data,
          loading: false,
        });
    case FILTER_COMPONENTS_FAILURE:
      return updateObjectProperty(state, 'components',
        {
          results: undefined,
          error: action.error,
          loading: false,
        });
    default:
      return state;
  }
}

export default reducer;
