import { composeValidators, hasLengthGreaterThan, isRequired, matchesPattern } from 'revalidate';

import { hasLength } from '../../../utilities/validation/validators';

export const titleValidator = isRequired;

export const yearValidator = composeValidators(
  isRequired,
  matchesPattern(/-?\d+/)
);

export const codeValidator = composeValidators(
  isRequired,
  hasLength(2)
);

export const descriptionValidator = composeValidators(
  isRequired,
  hasLengthGreaterThan(7)
);
