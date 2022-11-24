import { fireEvent, render } from '@testing-library/react';
import React from 'react';

import { OutletDescriptor } from '../../../../models/api/enumerations/outlets';
import AddEnumItemForm from '../AddEnumItemForm';

const renderAddEnumItemForm = (props: object = {}) =>
{
  const defaultProps =
  {
    enumerationName: '',
    itemDescriptor: OutletDescriptor,
    onSubmit: () => {},
    onCancel: () => {},
  };

  return render(<AddEnumItemForm {...defaultProps} {...props} />);
};

describe('<AddEnumItemForm />', () =>
{
  it('should show title by enumeration name', () =>
  {
    const expectedContent = 'TEST';
    const wrapper = renderAddEnumItemForm({ enumerationName: expectedContent });
    expect(wrapper.getAllByText(`New ${expectedContent} item`)).toHaveLength(1);
  });

  it('should call onCancel when cancel button is selected', async () =>
  {
    const onCancel = jest.fn();
    const wrapper = renderAddEnumItemForm({ onCancel });
    const switchViewButton = await wrapper.findByRole('button', { name: /cancel/i });
    fireEvent.click(switchViewButton);
    expect(onCancel).toHaveBeenCalled();
  });

  it('should submit form with correct values', async () =>
  {
    const titleValue = 'test';
    const codeValue = 'TE';
    const descriptionValue = 'test description';
    const onSubmit = jest.fn();
    const wrapper = renderAddEnumItemForm({ onSubmit });

    const submitForm = wrapper.container.querySelector('form');
    const title = wrapper.container.querySelector('input[name="title"]');
    const code = await wrapper.container.querySelector('input[name="code"]');
    const description = await wrapper.container.querySelector('input[name="description"]');

    expect(submitForm).not.toBeNull();
    expect(title).not.toBeNull();
    expect(code).not.toBeNull();
    expect(description).not.toBeNull();

    fireEvent.change(title!, { target: { value: titleValue } });
    fireEvent.change(code!, { target: { value: codeValue } });
    fireEvent.change(description!, { target: { value: descriptionValue } });

    fireEvent.submit(submitForm!);
    expect(onSubmit).toHaveBeenCalledWith({ code: codeValue, description: descriptionValue, title: titleValue });
  });
});
