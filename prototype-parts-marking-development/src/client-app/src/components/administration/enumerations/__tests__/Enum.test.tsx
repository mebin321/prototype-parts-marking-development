import { fireEvent, render } from '@testing-library/react';
import React from 'react';

import { OutletDescriptor } from '../../../../models/api/enumerations/outlets';
import { dummyItems } from '../../../../utilities/test/dummyData';
import Enum from '../Enum';

const renderEnum = (props: object = {}) =>
{
  const defaultProps =
  {
    name: '',
    items: dummyItems,
    itemDescriptor: OutletDescriptor,
    pageNumber: 1,
    totalPages: 1,
    showAddButton: true,
    onPageNumberChange: () => {},
    onAddButtonClick: () => {},
  };

  return render(<Enum {...defaultProps} {...props} />);
};

describe('<Enum />', () =>
{
  it('should call method on click to button', async () =>
  {
    const enumName = 'TestEnum';
    const addButtonClickHandler = jest.fn();
    const wrapper = renderEnum({ name: enumName, onAddButtonClick: addButtonClickHandler });
    const switchViewButton = await wrapper.findByRole('button', { name: `Add ${enumName}` });
    fireEvent.click(switchViewButton);
    expect(addButtonClickHandler).toHaveBeenCalled();
  });
});
