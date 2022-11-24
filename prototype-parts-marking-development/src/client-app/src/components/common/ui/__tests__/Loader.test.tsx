import { render } from '@testing-library/react';
import React from 'react';

import Loader from '../Loader';

const renderLoader = (props: object = {}) =>
{
  const defaultProps =
  {
    inverted: true,
  };

  return render(<Loader {...defaultProps} {...props} />);
};

describe('<Loader />', () =>
{
  it('should show text content', () =>
  {
    const expectedContent = 'Hello World!';
    const wrapper = renderLoader({ content: expectedContent });
    expect(wrapper.getAllByText(expectedContent)).toHaveLength(1);
  });

  it('should show spinner', () =>
  {
    const wrapper = renderLoader();
    expect(wrapper.container.querySelectorAll('div[class*="loader"]')).toHaveLength(1);
  });
});
