import { updateObject, updateObjectProperty } from '../utilities';
import
{
  ISearchState,
  SEARCH_PACKAGES_CLEAR,
  SEARCH_PACKAGES_FAILURE,
  SEARCH_PACKAGES_START,
  SEARCH_PACKAGES_SUCCESS,
  SEARCH_PROTOTYPES_CLEAR,
  SEARCH_PROTOTYPES_FAILURE,
  SEARCH_PROTOTYPES_START,
  SEARCH_PROTOTYPES_SUCCESS,
  SEARCH_RESULTS_FOLD,
  SEARCH_RESULTS_UNFOLD,
  SEARCH_VARIANTS_CLEAR,
  SEARCH_VARIANTS_FAILURE,
  SEARCH_VARIANTS_START,
  SEARCH_VARIANTS_SUCCESS,
  SearchActionTypes,
} from './types';

const initialState: ISearchState =
{
  packages: { loading: false },
  prototypes: { loading: false },
  variants: { loading: false },
};

// eslint-disable-next-line default-param-last
function reducer(state = initialState, action: SearchActionTypes)
{
  switch (action.type)
  {
    case SEARCH_PACKAGES_CLEAR:
      return updateObject(state,
        {
          packages: initialState.packages,
        });
    case SEARCH_PACKAGES_START:
      return updateObjectProperty(state, 'packages',
        {
          filter: action.filter,
          sort: action.sort,
          error: undefined,
          loading: true,
        });
    case SEARCH_PACKAGES_SUCCESS:
      return updateObjectProperty(state, 'packages',
        {
          results: action.data,
          loading: false,
        });
    case SEARCH_PACKAGES_FAILURE:
      return updateObjectProperty(state, 'packages',
        {
          error: action.error,
          loading: false,
        });
    case SEARCH_PROTOTYPES_CLEAR:
      return updateObject(state,
        {
          prototypes: initialState.prototypes,
        });
    case SEARCH_PROTOTYPES_START:
      return updateObjectProperty(state, 'prototypes',
        {
          filter: action.filter,
          sort: action.sort,
          error: undefined,
          loading: true,
        });
    case SEARCH_PROTOTYPES_SUCCESS:
      return updateObjectProperty(state, 'prototypes',
        {
          results: action.data,
          loading: false,
        });
    case SEARCH_PROTOTYPES_FAILURE:
      return updateObjectProperty(state, 'prototypes',
        {
          error: action.error,
          loading: false,
        });
    case SEARCH_VARIANTS_CLEAR:
      return updateObject(state,
        {
          variants: initialState.variants,
        });
    case SEARCH_VARIANTS_START:
      return updateObjectProperty(state, 'variants',
        {
          filter: action.filter,
          sort: action.sort,
          error: undefined,
          loading: true,
        });
    case SEARCH_VARIANTS_SUCCESS:
      return updateObjectProperty(state, 'variants',
        {
          results: action.data,
          loading: false,
        });
    case SEARCH_VARIANTS_FAILURE:
      return updateObjectProperty(state, 'variants',
        {
          error: action.error,
          loading: false,
        });
    case SEARCH_RESULTS_FOLD:
      return updateObject(state,
        {
          [action.target]: updateObject(state[action.target],
            {
              expanded: false,
            }),
        });
    case SEARCH_RESULTS_UNFOLD:
      return updateObject(state,
        {
          [action.target]: updateObject(state[action.target],
            {
              expanded: true,
            }
          ),
        });
    default:
      return state;
  }
}

export default reducer;
