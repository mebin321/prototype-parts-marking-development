import { SortDirection } from '../../../../../models/api/sort/sortDirection';
import { toggleSortDirection } from '../dataTableSort';

describe('dataTableSort', () =>
{
  describe('toggleSortDirection', () =>
  {
    it('should change ascending to descending', () =>
    {
      const toggledSortDirection = toggleSortDirection(SortDirection.Ascending);
      expect(toggledSortDirection).toBe(SortDirection.Descending);
    });

    it('should change descending to none', () =>
    {
      const toggledSortDirection = toggleSortDirection(SortDirection.Descending);
      expect(toggledSortDirection).toBe(SortDirection.None);
    });

    it('should change none to ascending', () =>
    {
      const toggledSortDirection = toggleSortDirection(SortDirection.None);
      expect(toggledSortDirection).toBe(SortDirection.Ascending);
    });
  });
});
