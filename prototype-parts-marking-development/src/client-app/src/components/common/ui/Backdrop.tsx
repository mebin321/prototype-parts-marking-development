import React from 'react';

import classes from './Backdrop.module.css';

interface IBackdropProps
{
  visible: boolean;
  onClick: () => void;
}

const Backdrop: React.FC<IBackdropProps> = ({
  visible,
  onClick,
}) =>
{
  return !visible
    ? null
    : <div className={classes.Backdrop} onClick={onClick}></div>;
};

export default Backdrop;
