import { Config, createValidator, matchesPattern } from 'revalidate';
import validator from 'validator';

import { isValueEmpty } from '../objects';

export const itemUniqueIdentifierPattern = /^[a-z0-9]{4}$/i;
export const materialNumberPatter = /^[a-z0-9]{13}$/i;
export const revisionCodePattern = /^[0-9]{2}$/;
export const projectSearchQueryPattern = /^[\w\- ]*$/;

export function isGreaterThan(min: number)
{
  return createValidator(
    (message: any) => (value: any) => (isNaN(value) || value >= min) ? undefined : message,
    (field: any) => `${field} must be greater than ${min}`);
}

export function hasLength(length: number)
{
  return createValidator(
    (message: any) => (value: any) => value?.length === length ? undefined : message,
    (field: any) => `${field} must be ${length} characters long`);
}

export const isEmail = createValidator(
  (message: any) => (value: any) => (typeof value === 'string' && validator.isEmail(value)) ? undefined : message,
  (field: any) => `${field} must be valid e-mail address (e.g. something like user@domain.com)`
);

export const isItemUniqueIdentifier = createValidator(
  (message) => (value: any) => !value || itemUniqueIdentifierPattern.test(value) ? undefined : message,
  (field) => `${field} must be 4 alphanumeric characters`
);

export const isMaterialNumber = createValidator(
  (message) => (value: any) => !value || materialNumberPatter.test(value) ? undefined : message,
  (field) => `${field} must be 13 alphanumeric characters`
);

export const isRevisionCode = createValidator(
  (message) => (value: any) => !value || revisionCodePattern.test(value) ? undefined : message,
  (field) => `${field} must be 2 digits`
);

export const isProjectSearchQuery = (config?: string | Config) =>
  matchesPattern(projectSearchQueryPattern)(config);

export const isProjectSearchQueryValidator =
  isProjectSearchQuery({ message: 'only alphanumeric characters, space, underscore and dash are allowed' });

export const isNotEmpty = createValidator(
  (message: any) => (value: any) => isValueEmpty(value) ? message : undefined,
  (field: any) => `${field} must not be empty`
);
