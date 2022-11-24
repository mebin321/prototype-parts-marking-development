import React, { useCallback, useEffect, useRef } from 'react';

import styles from './BorderlessInput.module.css';

interface IBorderlessInputProps extends React.InputHTMLAttributes<HTMLInputElement>
{
  onSubmit?: () => void;
}

const BorderlessInput: React.FC<IBorderlessInputProps> = ({
  onSubmit,
  ...props
}) =>
{
  const inputRef = useRef<HTMLInputElement>(null);

  const keyDownHandler = useCallback((event: KeyboardEvent) =>
  {
    if (event.key !== 'Enter') return;
    if (onSubmit) onSubmit();
  }, [onSubmit]);

  useEffect(() =>
  {
    const input = inputRef?.current;
    input?.addEventListener('keydown', keyDownHandler);

    return () => input?.removeEventListener('keydown', keyDownHandler);
  }, [keyDownHandler]);

  return (
    <input
      className={styles.Borderless}
      ref={inputRef}
      {...props}
    />
  );
};

export default BorderlessInput;
