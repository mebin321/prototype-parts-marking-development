import { Dispatch } from 'redux';

import { AuthDataLocalStorageKey, UserRolesLocalStorageKey } from '..';
import agent from '../../api/agent';
import { extractErrorDetails } from '../../api/utilities';
import { IAuthData } from '../../models/api/auth/authData';
import { IUserRole } from '../../models/api/roles/userRole';
import { IUserRolesConfiguration } from '../../models/api/roles/userRolesConfiguration';
import
{
  AUTH_FAILURE,
  AUTH_START,
  AUTH_SUCCESS,
  IAuthFailureAction,
  IAuthStartAction,
  IAuthSuccessAction,
  ILogoutAction,
  LOGOUT,
  SET_ROLES_CONFIGURATION,
} from './types';

function authStart(): IAuthStartAction
{
  return {
    type: AUTH_START,
  };
}

export function authSuccess(data: IAuthData, roles: IUserRole[]): IAuthSuccessAction
{
  localStorage.setItem(AuthDataLocalStorageKey, JSON.stringify(data));
  localStorage.setItem(UserRolesLocalStorageKey, JSON.stringify(roles));

  return {
    type: AUTH_SUCCESS,
    data: data,
    roles: roles,
  };
}

function authFailure(error: string): IAuthFailureAction
{
  return {
    type: AUTH_FAILURE,
    error: error,
  };
}

export function logout(): ILogoutAction
{
  localStorage.removeItem(AuthDataLocalStorageKey);
  localStorage.removeItem(UserRolesLocalStorageKey);

  return {
    type: LOGOUT,
  };
}

function setRolesConfiguration(configuration: IUserRolesConfiguration)
{
  return {
    type: SET_ROLES_CONFIGURATION,
    configuration: configuration,
  };
}

export function login(username: string, password: string)
{
  return async (dispatch: Dispatch) =>
  {
    dispatch(authStart());
    try
    {
      const authData = await agent.Auth.login(username, password);
      const userRoles = await agent.Users.listRoles(authData.user.id);
      dispatch(authSuccess(authData, userRoles));
    }
    catch (error)
    {
      dispatch(authFailure(extractErrorDetails(error)));
    }
  };
}

export function loadAuthConfiguration()
{
  return async (dispatch: Dispatch) =>
  {
    const rolesConfiguration = await agent.Roles.configuration();
    dispatch(setRolesConfiguration(rolesConfiguration));
  };
}

export function initializeAuthFromLocalStorage()
{
  return (dispatch: Dispatch) =>
  {
    const authDataJson = localStorage.getItem(AuthDataLocalStorageKey);
    const userRolesJson = localStorage.getItem(UserRolesLocalStorageKey);
    if (!authDataJson || !userRolesJson)
    {
      dispatch(logout());
      return;
    }

    const currentTime = Date.now();
    const authData = JSON.parse(authDataJson);
    const accessTokenExpiration = Date.parse(authData.accessToken.expiresAt);
    const refreshTokenExpiration = Date.parse(authData.refreshToken.expiresAt);

    // if access token and refresh token is expired then user has to enter credentials
    // therefore the user is technically not logged in -> authentication information can be deleted
    if (accessTokenExpiration < currentTime && refreshTokenExpiration < currentTime)
    {
      dispatch(logout());
      return;
    }

    dispatch(authSuccess(authData, JSON.parse(userRolesJson)));
  };
}
