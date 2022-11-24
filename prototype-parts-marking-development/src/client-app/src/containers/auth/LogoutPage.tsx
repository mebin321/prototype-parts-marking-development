import React, { useEffect } from 'react';
import { ConnectedProps, connect } from 'react-redux';
import { Redirect } from 'react-router-dom';

import { logout } from '../../store';

const mapDispatchToProps =
{
  onLogOut: logout,
};

const connector = connect(undefined, mapDispatchToProps);

interface ILogoutPageProps extends ConnectedProps<typeof connector>
{
}

const LogoutPage: React.FC<ILogoutPageProps> = ({
  onLogOut,
}) =>
{
  useEffect(() =>
  {
    onLogOut();
  }, [onLogOut]);

  return <Redirect to="/" />;
};

export default connector(LogoutPage);
