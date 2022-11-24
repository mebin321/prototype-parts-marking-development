import { Action } from 'redux';

import { IAuthData } from '../../models/api/auth/authData';
import { IAuthToken } from '../../models/api/auth/authToken';
import { IUserRole } from '../../models/api/roles/userRole';
import { IUserRolesConfiguration } from '../../models/api/roles/userRolesConfiguration';
import { IUserData } from '../../models/api/users/userData';

export interface IAuthState
{
  user: IUserData;
  accessToken: IAuthToken;
  refreshToken: IAuthToken;
  roles: IUserRole[];
  rolesConfiguration: IUserRolesConfiguration;
  error?: string;
  loading: boolean;
}

export const AUTH_START = 'AUTH_START';
export const AUTH_SUCCESS = 'AUTH_SUCCESS';
export const AUTH_FAILURE = 'AUTH_FAILURE';
export const LOGOUT = 'AUTH_LOGOUT';
export const SET_ROLES_CONFIGURATION = 'AUTH_SET_ROLES_CONFIGURATION';

export interface IAuthStartAction extends Action<typeof AUTH_START>
{
}

export interface IAuthSuccessAction extends Action<typeof AUTH_SUCCESS>
{
  data: IAuthData;
  roles: IUserRole[];
}

export interface IAuthFailureAction extends Action<typeof AUTH_FAILURE>
{
  error: string;
}

export interface ILogoutAction extends Action<typeof LOGOUT>
{
}

export interface ISetRolesConfigurationAction extends Action<typeof SET_ROLES_CONFIGURATION>
{
  configuration: IUserRolesConfiguration;
}

export type AuthActionTypes = IAuthStartAction | IAuthSuccessAction | IAuthFailureAction | ILogoutAction
  | ISetRolesConfigurationAction;
