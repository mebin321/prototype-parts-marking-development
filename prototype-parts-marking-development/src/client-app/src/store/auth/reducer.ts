import { EmptyAuthToken } from '../../models/api/auth/authToken';
import { EmptyUserData } from '../../models/api/users/userData';
import { updateObject } from '../utilities';
import
{
  AUTH_FAILURE,
  AUTH_START,
  AUTH_SUCCESS,
  AuthActionTypes,
  IAuthState,
  LOGOUT,
  SET_ROLES_CONFIGURATION,
} from './types';

const initialState: IAuthState =
{
  user: EmptyUserData,
  accessToken: EmptyAuthToken,
  refreshToken: EmptyAuthToken,
  roles: [],
  rolesConfiguration: { permissionMap: [] },
  loading: false,
};

// eslint-disable-next-line default-param-last
function reducer(state = initialState, action: AuthActionTypes)
{
  switch (action.type)
  {
    case AUTH_START:
      return updateObject(state,
        {
          error: undefined,
          loading: true,
        });
    case AUTH_SUCCESS:
      return updateObject(state,
        {
          user: action.data.user,
          accessToken: action.data.accessToken,
          refreshToken: action.data.refreshToken,
          roles: action.roles,
          loading: false,
        });
    case AUTH_FAILURE:
      return updateObject(state,
        {
          error: action.error,
          loading: false,
        });
    case LOGOUT:
      return updateObject(state,
        {
          user: initialState.user,
          accessToken: initialState.accessToken,
          refreshToken: initialState.refreshToken,
          roles: initialState.roles,
        });
    case SET_ROLES_CONFIGURATION:
      return updateObject(state,
        {
          rolesConfiguration: action.configuration,
        }
      );
    default:
      return state;
  }
}

export default reducer;
