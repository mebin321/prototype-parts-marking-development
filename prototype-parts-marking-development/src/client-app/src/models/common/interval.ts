import { formatDateTimeToLocalCultureInvariant } from '../../utilities/datetime';
import { parseReactNumber } from '../../utilities/react';

export interface IInterval<T>
{
  lowerBound?: T;
  upperBound?: T;
}

export type NumberInterval = IInterval<number>;
export type DateInterval = IInterval<Date>;

export const NumberIntervalSeparator = '-';
export const DateIntervalSeparator = ' ~ ';

export function parseInterval<T>(
  text: string | undefined,
  separator: string,
  parseValue: (value: string) => T | undefined
): IInterval<T>
{
  if (!text) return {};

  const chunks = text.split(separator, 2);
  if (chunks.length < 1) return {};

  if (chunks.length < 2)
  {
    const value = parseValue(chunks[0]);
    if (text?.startsWith(separator)) return { upperBound: value };
    if (text?.endsWith(separator)) return { lowerBound: value };
    return { lowerBound: value, upperBound: value };
  }

  return { lowerBound: parseValue(chunks[0]), upperBound: parseValue(chunks[1]) };
}

export function parseNumberInterval(text: string | undefined): NumberInterval
{
  return parseInterval(text, NumberIntervalSeparator, parseReactNumber);
}

function defaultValueFormatter(value: any)
{
  if (value === undefined || value === null) return '';

  return String(value);
}

export function formatInterval<T>(
  interval: IInterval<T> | undefined,
  separator: string,
  formatter?: (value: T | undefined) => string
)
{
  const lowerBound = interval?.lowerBound;
  const upperBound = interval?.upperBound;
  const lowerBoundFormatted = formatter ? formatter(lowerBound) : defaultValueFormatter(lowerBound);
  const upperBoundFormatted = formatter ? formatter(upperBound) : defaultValueFormatter(upperBound);

  if (lowerBoundFormatted === upperBoundFormatted) return lowerBoundFormatted;

  return `${lowerBoundFormatted}${separator}${upperBoundFormatted}`;
}

export function formatNumberInterval(interval: NumberInterval | undefined)
{
  return formatInterval(interval, NumberIntervalSeparator);
}

export function formatDateInterval(interval: DateInterval | undefined)
{
  return formatInterval(interval, DateIntervalSeparator, formatDateTimeToLocalCultureInvariant);
}
