import { Dispatch, useCallback, useState } from 'react';

export type SetLocalStorageAction<T> = T | ((value: T) => T);

export type LocalStorageValueSetter<T> = Dispatch<SetLocalStorageAction<T>>;

function getInitialValue<T>(value: T | (() => T))
{
  return value instanceof Function ? value() : value;
}

export function useLocalStorage<T = undefined>(key: string): [T | undefined, LocalStorageValueSetter<T | undefined>];
export function useLocalStorage<T>(key: string, initialValue: T | (() => T)): [T, LocalStorageValueSetter<T>];
export function useLocalStorage<T>(key: string, initialValue?: T | (() => T)): [T, LocalStorageValueSetter<T>]
{
  const [storedValue, setStoredValue] = useState<T>(() =>
  {
    try
    {
      const value = window.localStorage.getItem(key);
      return !value || value === 'undefined'
        ? getInitialValue(initialValue)
        : JSON.parse(value);
    }
    catch (error)
    {
      console.log(error);

      return getInitialValue(initialValue);
    }
  });

  const setValue = useCallback((value: T | ((val: T) => T)) =>
  {
    try
    {
      setStoredValue(prevStoredValue =>
      {
        const valueToStore = value instanceof Function ? value(prevStoredValue) : value;
        window.localStorage.setItem(key, JSON.stringify(valueToStore));
        return valueToStore;
      });
    }
    catch (error)
    {
      console.log(error);
    }
  }, [key]);

  return [storedValue, setValue];
}
