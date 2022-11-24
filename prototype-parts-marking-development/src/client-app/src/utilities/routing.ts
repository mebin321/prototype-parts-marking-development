export function parseIdFromParameter(entity: string, id: string)
{
  if (!id)
  {
    throw Error(`Not found ${entity} ID in address`);
  }

  const result = parseInt(id);
  if (isNaN(result) || !isFinite(result))
  {
    throw Error(`Incorrect format of ${entity} ID - expected integer but got ${id}`);
  }

  return result;
}

export function parseChildRoute(completeUrl: string, matchUrl: string)
{
  if (!completeUrl || !matchUrl) return '';
  if (!completeUrl.startsWith(matchUrl)) return '';

  let childUrl = completeUrl.substr(matchUrl.length);
  if (childUrl.startsWith('/'))
  {
    childUrl = childUrl.substr(1);
  }

  const slashIndex = childUrl.indexOf('/');
  if (slashIndex >= 0)
  {
    childUrl = childUrl.substr(0, slashIndex);
  }

  return childUrl;
}
