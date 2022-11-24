import { fireEvent, render } from '@testing-library/react';
import React from 'react';
import { MemoryRouter } from 'react-router-dom';

import PaginationControls from '../PaginationControls';

const renderPaginationControls = (props: object = {}) =>
{
  const defaultProps =
  {
    totalPages: 0,
    pageNumber: 0,
    onPageNumberChange()
    {},
  };

  return render(<PaginationControls {...defaultProps} {...props} />, { wrapper: MemoryRouter });
};

describe('<PaginationControls />', () =>
{
  it('should call onPageNumberChange with respective number instead of string', async () =>
  {
    const onPageNumberChangeHandler = jest.fn();

    const wrapper = renderPaginationControls({
      onPageNumberChange: onPageNumberChangeHandler,
      totalPages: 10,
      pageNumber: '5',
    });
    const paginationDecreaseButton = await wrapper.findByText('\u27E8');

    fireEvent.click(paginationDecreaseButton);

    expect(onPageNumberChangeHandler).toHaveBeenCalledWith(4);
  });

  it('should call onPageNumberChange with decrease number', async () =>
  {
    const onPageNumberChangeHandler = jest.fn();

    const wrapper = renderPaginationControls({
      onPageNumberChange: onPageNumberChangeHandler,
      totalPages: 10,
      pageNumber: 5,
    });
    const paginationDecreaseButton = await wrapper.findByText('\u27E8');

    fireEvent.click(paginationDecreaseButton);

    expect(onPageNumberChangeHandler).toHaveBeenCalledWith(4);
  });
});
