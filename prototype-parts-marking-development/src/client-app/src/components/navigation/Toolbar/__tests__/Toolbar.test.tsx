import { render } from '@testing-library/react';
import { createMemoryHistory } from 'history';
import React from 'react';
import { MemoryRouter } from 'react-router-dom';

import { EmptyUserData } from '../../../../models/api/users/userData';
import Toolbar from '../Toolbar';

const dummyDispatch = jest.fn(action => action);
const sampleUser = { id: 1, username: 'me', email: 'nobody@nowhere.com', name: 'Me Here' };

const renderToolbar = (props: object = {}) =>
{
  const defaultProps =
  {
    isMobileView: false,
    items: [],
    user: EmptyUserData,
    toggleSideDrawer: () => {},
    dispatch: dummyDispatch,
    history: createMemoryHistory(),
    location: { pathname: '', search: '', state: undefined, hash: '' },
    match: { params: {}, isExact: false, path: '', url: '' },
  };

  return render(<Toolbar {...defaultProps} {...props} />, { wrapper: MemoryRouter });
};

describe('<Toolbar />', () =>
{
  it('should contain a login link if unauthenticated', () =>
  {
    const wrapper = renderToolbar({ user: EmptyUserData });
    expect(wrapper.container.querySelector('a[class*="item"][title="Log In"]')).toBeTruthy();
  });

  it('should not contain a login link if authenticated', () =>
  {
    const wrapper = renderToolbar({ user: sampleUser });
    expect(wrapper.container.querySelector('a[class*="item"][title="Log In"]')).not.toBeTruthy();
  });
});
