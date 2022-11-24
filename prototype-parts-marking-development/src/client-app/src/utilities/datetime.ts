export function formatDateToLocalIso(value: Date, separator = '-')
{
  const yearString = value.getFullYear().toString().padStart(4, '0');
  const monthString = (value.getMonth() + 1).toString().padStart(2, '0'); // +1 because month is zero-based
  const dayString = value.getDate().toString().padStart(2, '0');

  return [yearString, monthString, dayString].join(separator);
}

export function formatTimeToLocalIso(value: Date, normalize = false, timeSeparator = ':', decimalSeparator = '.')
{
  const hoursString = value.getHours().toString().padStart(2, '0');
  const minutesString = value.getMinutes().toString().padStart(2, '0');
  const secondsString = value.getSeconds().toString().padStart(2, '0');
  const millisecondsString = value.getMilliseconds().toString().padStart(3, '0');

  let result = `${hoursString}${timeSeparator}${minutesString}`;

  if (!normalize || value.getSeconds() || value.getMilliseconds())
  {
    result += `${timeSeparator}${secondsString}`;
  }

  if (!normalize || value.getMilliseconds())
  {
    result += `${decimalSeparator}${millisecondsString}`;
  }

  return result;
}

export function formatDateTimeToLocalIso(
  value: Date,
  normalize = false,
  options?: { dateSeparator?: string; timeSeparator?: string; decimalSeparator?: string })
{
  const dateString = formatDateToLocalIso(value, options?.dateSeparator);
  const timeString = formatTimeToLocalIso(value, normalize, options?.timeSeparator, options?.decimalSeparator);

  return `${dateString}T${timeString}`;
}

export function formatDateTimeToLocalCultureInvariant(value: Date | undefined | null, normalize = true)
{
  if (value === undefined || value === null) return '';

  return formatDateTimeToLocalIso(value, normalize)
    .replace('T', ' '); // replace date and time separator
}

export function formatDateTimeToInput(value: Date | undefined | null)
{
  if (value === undefined || value === null) return '';

  return formatDateTimeToLocalIso(value, false)
    .replace(/(:\d+(\.\d+)?)?Z$/, ''); // remove seconds, milliseconds and time-zone
}
