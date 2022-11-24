import { getPropertyValue } from '../objects';

describe('dataSpecifier', () =>
{
  describe('getPropertyValue', () =>
  {
    it('should return undefined on empty object and empty property', () =>
    {
      const actualValue = getPropertyValue({}, '');
      expect(actualValue).toBeUndefined();
    });

    it('should return undefined on empty object and non-empty property', () =>
    {
      const actualValue = getPropertyValue({}, 'test');
      expect(actualValue).toBeUndefined();
    });

    it('should return value on existent property', () =>
    {
      const expectedValue = { boo: 'foo' };
      const actualValue = getPropertyValue({ test: expectedValue }, 'test');
      expect(actualValue).toBe(expectedValue);
    });

    it('should return undefined on empty object and given getter function', () =>
    {
      const actualValue = getPropertyValue({}, obj => (obj as any).test);
      expect(actualValue).toBeUndefined();
    });

    it('should return value on existent property and given getter function', () =>
    {
      const expectedValue = { boo: 'foo' };
      const actualValue = getPropertyValue({ test: expectedValue }, obj => obj.test);
      expect(actualValue).toBe(expectedValue);
    });
  });
});
