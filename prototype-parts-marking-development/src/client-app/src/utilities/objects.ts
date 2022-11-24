import { capitalizeAllWords } from './text';

export type DataGetterFunction<TObject extends object, TReturn = any> = (obj: TObject) => TReturn;
export type DataSetterFunction<TObject extends object> = (obj: TObject, value: any) => void;
export type DataGetter<TObject extends object, TReturn = any> = keyof TObject | DataGetterFunction<TObject, TReturn>;
export type DataSetter<TObject extends object> = keyof TObject | DataSetterFunction<TObject>;

export function getPropertyValue<T extends object>(obj: T | undefined, property: DataGetter<T> | undefined)
{
  if (obj === undefined) return undefined;
  if (property === undefined) return undefined;

  return typeof property === 'function' ? property(obj) : obj[property];
}

export function setPropertyValue<T extends object>(obj: T | undefined, property: DataSetter<T> | undefined, value: any)
{
  if (obj === undefined) return;
  if (property === undefined) return;

  if (typeof property === 'function')
  {
    property(obj, value);
  }
  else
  {
    obj[property] = value;
  }
}

export function isValueEmpty(value: any)
{
  if (value === undefined || value === null) return true;

  switch (typeof value)
  {
    case 'boolean':
      return false;
    case 'number':
    case 'bigint':
    {
      return Number.isNaN(value);
    }
    case 'object':
    {
      if (Array.isArray(value)) return value.length < 1;
      if (value instanceof Date) return !value;

      // consider object falsy when it contains no properties or all properties are falsy
      for (const key of Object.getOwnPropertyNames(value))
      {
        const propertyValue = value[key];
        if (!isValueEmpty(propertyValue)) return false;
      }

      return true;
    }
    default:
    {
      return !value;
    }
  }
}

export function prettifyPropertyName(name: string)
{
  name = name.replace(/_/g, ' ');
  name = name.replace(/([A-Z]+)/g, ' $1');
  name = name.trimStart();
  name = capitalizeAllWords(name);

  return name;
}

export function generateMoniker(name: string)
{
  return name.replace(/\s/g, '-').toLowerCase();
}

export function parseLocalStorageJSON<T = any>(key: string, defaultValue: T): T
{
  const value = localStorage.getItem(key);
  if (!value) return defaultValue;

  return JSON.parse(value);
}
