import { useSelector } from 'react-redux';

import
{
  selectCanCreatePrototypePackages,
  selectCanCreatePrototypeSet,
  selectCanCreatePrototypes,
  selectCanDisableUsers,
  selectCanEnableUsers,
  selectCanListAdUsers,
  selectCanModifyEntityRelation,
  selectCanModifyEvidenceYear,
  selectCanModifyGateLevel,
  selectCanModifyLocation,
  selectCanModifyOutlet,
  selectCanModifyPart,
  selectCanModifyPrintingLabels,
  selectCanModifyProductGroup,
  selectCanModifyPrototypePackages,
  selectCanModifyPrototypeVariants,
  selectCanModifyPrototypes,
  selectCanModifyRole,
  selectCanModifyUserRole,
  selectCanModifyUsers,
  selectCanPerformMaintenance,
  selectCanReactivatePrototypeSets,
  selectCanReactivatePrototypes,
  selectCanReactivatePrototypesPackages,
  selectCanReadEntityRelations,
  selectCanScrapPrototypeSets,
  selectCanScrapPrototypes,
  selectCanScrapPrototypesPackages,
} from '../store/auth/selectors';

export default function usePermissions()
{
  const canCreatePrototypePackages = useSelector(selectCanCreatePrototypePackages);
  const canCreatePrototypes = useSelector(selectCanCreatePrototypes);
  const canCreatePrototypeSet = useSelector(selectCanCreatePrototypeSet);
  const canScrapPrototypeSets = useSelector(selectCanScrapPrototypeSets);
  const canDisableUsers = useSelector(selectCanDisableUsers);
  const canEnableUsers = useSelector(selectCanEnableUsers);
  const canListAdUsers = useSelector(selectCanListAdUsers);
  const canModifyEntityRelation = useSelector(selectCanModifyEntityRelation);
  const canModifyEvidenceYear = useSelector(selectCanModifyEvidenceYear);
  const canModifyGateLevel = useSelector(selectCanModifyGateLevel);
  const canModifyLocation = useSelector(selectCanModifyLocation);
  const canModifyOutlet = useSelector(selectCanModifyOutlet);
  const canModifyPart = useSelector(selectCanModifyPart);
  const canModifyPrintingLabels = useSelector(selectCanModifyPrintingLabels);
  const canModifyProductGroup = useSelector(selectCanModifyProductGroup);
  const canModifyPrototypePackages = useSelector(selectCanModifyPrototypePackages);
  const canModifyPrototypes = useSelector(selectCanModifyPrototypes);
  const canModifyPrototypeVariants = useSelector(selectCanModifyPrototypeVariants);
  const canModifyRole = useSelector(selectCanModifyRole);
  const canModifyUserRole = useSelector(selectCanModifyUserRole);
  const canModifyUsers = useSelector(selectCanModifyUsers);
  const canPerformMaintenance = useSelector(selectCanPerformMaintenance);
  const canReactivatePrototypes = useSelector(selectCanReactivatePrototypes);
  const canReactivatePrototypeSets = useSelector(selectCanReactivatePrototypeSets);
  const canReadEntityRelations = useSelector(selectCanReadEntityRelations);
  const canReactivatePrototypesPackages = useSelector(selectCanReactivatePrototypesPackages);
  const canScrapPrototypes = useSelector(selectCanScrapPrototypes);
  const canScrapPrototypesPackages = useSelector(selectCanScrapPrototypesPackages);

  return {
    canCreatePrototypePackages,
    canCreatePrototypes,
    canCreatePrototypeSet,
    canScrapPrototypeSets,
    canDisableUsers,
    canEnableUsers,
    canListAdUsers,
    canModifyEntityRelation,
    canModifyEvidenceYear,
    canModifyGateLevel,
    canModifyLocation,
    canModifyOutlet,
    canModifyPart,
    canModifyPrintingLabels,
    canModifyProductGroup,
    canModifyPrototypePackages,
    canModifyPrototypes,
    canModifyPrototypeVariants,
    canModifyRole,
    canModifyUserRole,
    canModifyUsers,
    canPerformMaintenance,
    canReactivatePrototypes,
    canReactivatePrototypeSets,
    canReadEntityRelations,
    canReactivatePrototypesPackages,
    canScrapPrototypes,
    canScrapPrototypesPackages,
  };
}
