import { createSelector } from 'reselect';

import
{
  CanCreatePrototypePackages,
  CanCreatePrototypeSet,
  CanCreatePrototypes,
  CanDisableUsers,
  CanEnableUsers,
  CanListAdUsers,
  CanModifyEntityRelation,
  CanModifyEvidenceYear,
  CanModifyGateLevel,
  CanModifyLocation,
  CanModifyOutlet,
  CanModifyPart,
  CanModifyPrintingLabels,
  CanModifyProductGroup,
  CanModifyPrototypePackages,
  CanModifyPrototypeVariants,
  CanModifyPrototypes,
  CanModifyRole,
  CanModifyUserRole,
  CanModifyUsers,
  CanPerformMaintenance,
  CanReactivatePrototypeSets,
  CanReactivatePrototypes,
  CanReactivatePrototypesPackages,
  CanReadEntityRelations,
  CanScrapPrototypeSets,
  CanScrapPrototypes,
  CanScrapPrototypesPackages,
} from '../../models/api/roles/permissions';
import { IUserRole } from '../../models/api/roles/userRole';
import { IUserRolesPermissionMapEntry } from '../../models/api/roles/userRolesPermissionMapEntry';
import { IApplicationState } from '../index';

function canDispatchAction(action: string, userRoles: IUserRole[], permissionMap: IUserRolesPermissionMapEntry[])
{
  for (const userRole of userRoles)
  {
    const roleConfiguration = permissionMap.find(role => role.moniker === userRole.moniker);
    if (roleConfiguration && roleConfiguration.permissions.includes(action))
    {
      return true;
    }
  }

  return false;
}

function createPermissionCheckSelector(action: string)
{
  return createSelector(
    (state: IApplicationState) => state.auth.roles,
    (state: IApplicationState) => state.auth.rolesConfiguration.permissionMap,
    (roles, permissionMap) => canDispatchAction(action, roles, permissionMap)
  );
}

export const selectCanCreatePrototypePackages = createPermissionCheckSelector(CanCreatePrototypePackages);
export const selectCanCreatePrototypes = createPermissionCheckSelector(CanCreatePrototypes);
export const selectCanCreatePrototypeSet = createPermissionCheckSelector(CanCreatePrototypeSet);
export const selectCanScrapPrototypeSets = createPermissionCheckSelector(CanScrapPrototypeSets);
export const selectCanDisableUsers = createPermissionCheckSelector(CanDisableUsers);
export const selectCanEnableUsers = createPermissionCheckSelector(CanEnableUsers);
export const selectCanListAdUsers = createPermissionCheckSelector(CanListAdUsers);
export const selectCanModifyEntityRelation = createPermissionCheckSelector(CanModifyEntityRelation);
export const selectCanModifyEvidenceYear = createPermissionCheckSelector(CanModifyEvidenceYear);
export const selectCanModifyGateLevel = createPermissionCheckSelector(CanModifyGateLevel);
export const selectCanModifyLocation = createPermissionCheckSelector(CanModifyLocation);
export const selectCanModifyOutlet = createPermissionCheckSelector(CanModifyOutlet);
export const selectCanModifyPart = createPermissionCheckSelector(CanModifyPart);
export const selectCanModifyPrintingLabels = createPermissionCheckSelector(CanModifyPrintingLabels);
export const selectCanModifyProductGroup = createPermissionCheckSelector(CanModifyProductGroup);
export const selectCanModifyPrototypePackages = createPermissionCheckSelector(CanModifyPrototypePackages);
export const selectCanModifyPrototypes = createPermissionCheckSelector(CanModifyPrototypes);
export const selectCanModifyPrototypeVariants = createPermissionCheckSelector(CanModifyPrototypeVariants);
export const selectCanModifyRole = createPermissionCheckSelector(CanModifyRole);
export const selectCanModifyUserRole = createPermissionCheckSelector(CanModifyUserRole);
export const selectCanModifyUsers = createPermissionCheckSelector(CanModifyUsers);
export const selectCanPerformMaintenance = createPermissionCheckSelector(CanPerformMaintenance);
export const selectCanReactivatePrototypes = createPermissionCheckSelector(CanReactivatePrototypes);
export const selectCanReactivatePrototypeSets = createPermissionCheckSelector(CanReactivatePrototypeSets);
export const selectCanReadEntityRelations = createPermissionCheckSelector(CanReadEntityRelations);
export const selectCanReactivatePrototypesPackages = createPermissionCheckSelector(CanReactivatePrototypesPackages);
export const selectCanScrapPrototypes = createPermissionCheckSelector(CanScrapPrototypes);
export const selectCanScrapPrototypesPackages = createPermissionCheckSelector(CanScrapPrototypesPackages);
