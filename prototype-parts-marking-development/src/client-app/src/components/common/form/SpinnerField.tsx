import React, { SyntheticEvent, useCallback } from 'react';
import { FieldRenderProps } from 'react-final-form';
import { Form, FormFieldProps, InputOnChangeData, Label } from 'semantic-ui-react';

interface ISpinnerFieldProps
  extends FieldRenderProps<number>, FormFieldProps
{
  min?: number;
  max?: number;
  step?: number;
  readOnly?: boolean;
  autoFocus?: boolean;
  onChange?: (event: SyntheticEvent, data: InputOnChangeData) => void;
}

const SpinnerField: React.FC<ISpinnerFieldProps> = ({
  input,
  min = 1,
  max = 999,
  step = 1,
  readOnly,
  disabled,
  autoFocus,
  width,
  type,
  label,
  placeholder,
  meta: { touched, error },
  onChange,
}) =>
{
  const ensureValueIsInRange = useCallback((data: InputOnChangeData) =>
  {
    const value = +data.value;

    if (min !== undefined && value < min)
    {
      data.value = min.toString();
    }

    if (max !== undefined && value > max)
    {
      data.value = max.toString();
    }
  }, [min, max]);

  const spinnerChangeHandler = useCallback((event: SyntheticEvent, data: InputOnChangeData) =>
  {
    ensureValueIsInRange(data);
    input.onChange(data.value);
    if (onChange) onChange(event, data);
  }, [input, ensureValueIsInRange, onChange]);

  return (
    <Form.Field error={touched && error} type={type} width={width} style={{ fontWeight: 'bold' }}>
      <Form.Input
        {...input}
        readOnly={readOnly}
        disabled={disabled}
        label={label}
        input={{ autoFocus: autoFocus }}
        placeholder={placeholder}
        type='number'
        min={min}
        max={max}
        step={step}
        onChange={spinnerChangeHandler}
      />
      {touched && error && (
        <Label basic color='red'>
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default SpinnerField;
