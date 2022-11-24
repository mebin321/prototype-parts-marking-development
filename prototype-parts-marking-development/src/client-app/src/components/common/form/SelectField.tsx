import React, { SyntheticEvent, useCallback } from 'react';
import { FieldRenderProps } from 'react-final-form';
import { DropdownProps, Form, FormFieldProps, Label, Select } from 'semantic-ui-react';

export interface IDropdownItemData
{
  key: string;
  text: string;
  value: string;
}

interface ISelectFieldProps
  extends FieldRenderProps<string>, FormFieldProps
{
  readOnly?: boolean;
  onChange?: (event: SyntheticEvent, data: DropdownProps) => void;
}

const SelectField: React.FC<ISelectFieldProps> = ({
  input,
  readOnly,
  disabled,
  width,
  options,
  placeholder,
  label,
  meta: { error, touched },
  onChange,
}) =>
{
  const dropdownChangeHandler = useCallback((event: SyntheticEvent, data: DropdownProps) =>
  {
    input.onChange(data.value);
    if (onChange) onChange(event, data);
  }, [input, onChange]);

  return (
    <Form.Field
      width={width}
      error={touched && error}
      style={{ fontWeight: 'bold' }}
      onBlur={input.onBlur}
      onFocus={input.onFocus}
    >
      <div style={{ marginBottom: '0.3em' }}>{label}</div>
      <Select
        disabled={readOnly}
        // because property readOnly does nothing on Select component
        // it has to be disabled to prevent changing selected value
        // but to prevent the grayed out look opacity is set back to 1
        style={!disabled && { fontWeight: 'normal', opacity: 1 }}
        placeholder={placeholder}
        options={options}
        value={input.value}
        onChange={dropdownChangeHandler}
        />
      {touched && error && (
        <Label basic color='red'>
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default SelectField;
