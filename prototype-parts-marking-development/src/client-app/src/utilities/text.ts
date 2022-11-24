const wordLeadingCharacterPattern = /\b\w(?=\B)/g;

export function capitalizeFirstWord(s: string)
{
  if (s.length < 1) return s;

  return s.charAt(0).toUpperCase() + s.slice(1);
}

export function capitalizeAllWords(s: string)
{
  return s.replace(wordLeadingCharacterPattern, c => c.toUpperCase());
}

export function limitTextSize(s: string | undefined | null, limit: number, strict = false)
{
  const length = s?.length ?? 0;
  if (length <= limit) return s;
  if (strict) limit -= 3;

  return `${s?.substring(0, limit)}...`;
}

export function evaluateTextInsertion(
  currentValue: string | null | undefined,
  selectionStart: number | null,
  selectionEnd: number | null,
  insertion: string)
{
  currentValue = currentValue ?? '';
  selectionStart = selectionStart !== null ? selectionStart : currentValue.length;
  selectionEnd = selectionEnd !== null ? selectionEnd : currentValue.length;

  const selectionRange = selectionStart < selectionEnd
    ? [selectionStart, selectionEnd]
    : [selectionEnd, selectionStart];
  const head = currentValue.substring(0, selectionRange[0]);
  const tail = currentValue.substring(selectionRange[1]);
  return `${head}${insertion}${tail}`;
}
