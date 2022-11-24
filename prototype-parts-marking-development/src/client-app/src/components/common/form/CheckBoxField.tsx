import React, { SyntheticEvent, useCallback } from 'react';
import { FieldRenderProps } from 'react-final-form';
import { Checkbox, CheckboxProps, FormFieldProps } from 'semantic-ui-react';

interface ICheckBoxFieldProps
  extends FieldRenderProps<boolean>, FormFieldProps
{
  onChange?: (event: SyntheticEvent, data: CheckboxProps) => void;
}

const CheckBoxField: React.FC<ICheckBoxFieldProps> = ({
  input,
  label,
  disabled,
  checked,
  toggle,
  onChange,
}) =>
{
  const checkboxChangeHandler = useCallback((event: SyntheticEvent, data: CheckboxProps) =>
  {
    input.onChange(data.value);
    if (onChange) onChange(event, data);
  }, [input, onChange]);

  return (
      <Checkbox
        toggle={toggle}
        label={label}
        onChange={checkboxChangeHandler}
        disabled={disabled}
        checked={checked}
      />
  );
};

export default CheckBoxField;
