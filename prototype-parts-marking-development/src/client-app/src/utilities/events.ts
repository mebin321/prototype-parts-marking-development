import _, { DebouncedFunc } from 'lodash';

// timeouts in milliseconds
export const textSearchTimeout = 500;
export const tableFilterTimeout = 1000;

export function debounceInputChangeEventHandler<T extends (...args: any) => any>(
  func: T,
  timeout: number
): DebouncedFunc<T>
{
  return _.debounce(func, timeout, { leading: false, trailing: true });
}
