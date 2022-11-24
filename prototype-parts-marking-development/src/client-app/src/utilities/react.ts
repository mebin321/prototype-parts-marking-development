import { ReactText } from 'react';

export function parseReactNumber(value: ReactText | undefined)
{
  switch (typeof value)
  {
    case 'number':
      return value;
    case 'string':
      if (!value) return undefined;
      return Number.parseInt(value);
    default:
      return undefined;
  }
}

export function parseReactDate(value: ReactText | undefined)
{
  return (value === undefined || value === '') ? undefined : new Date(value);
}
