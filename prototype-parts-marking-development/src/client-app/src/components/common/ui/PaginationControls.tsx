import React, { SyntheticEvent, useCallback } from 'react';
import { Pagination, PaginationProps } from 'semantic-ui-react';

interface IPaginationControlsProps
{
  totalPages?: number;
  pageNumber?: number;
  onPageNumberChange?: (pageNumber: number) => void;
}

const PaginationControls: React.FC<IPaginationControlsProps> = ({
  totalPages,
  pageNumber,
  onPageNumberChange,
}) =>
{
  const pageChangeHandler = useCallback((e: SyntheticEvent, pageInfo: PaginationProps) =>
  {
    let activePage: number;
    if (pageInfo.activePage === undefined)
    {
      activePage = -1;
    }
    else if (typeof pageInfo.activePage === 'string')
    {
      activePage = +pageInfo.activePage;
    }
    else
    {
      activePage = pageInfo.activePage;
    }

    if (onPageNumberChange) onPageNumberChange(activePage);
  }, [onPageNumberChange]);

  return (
    <Pagination
      activePage={pageNumber ?? 1}
      ellipsisItem={null}
      firstItem={'\u27EA'}
      prevItem={'\u27E8'}
      nextItem={'\u27E9'}
      lastItem={'\u27EB'}
      siblingRange={1}
      totalPages={totalPages ?? 0}
      onPageChange={pageChangeHandler}
    />
  );
};

export default PaginationControls;
