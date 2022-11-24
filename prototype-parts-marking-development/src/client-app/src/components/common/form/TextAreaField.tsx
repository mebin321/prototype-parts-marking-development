import React, { SyntheticEvent, useCallback } from 'react';
import { FieldRenderProps } from 'react-final-form';
import { Form, FormFieldProps, Label, TextArea, TextAreaProps } from 'semantic-ui-react';

interface ITextAreaFieldProps
  extends FieldRenderProps<string>, FormFieldProps
{
  readOnly?: boolean;
  onChange?: (event: SyntheticEvent, data: TextAreaProps) => void;
}

const TextAreaField: React.FC<ITextAreaFieldProps> = ({
  input,
  readOnly,
  disabled,
  width,
  rows,
  placeholder,
  label,
  meta: { touched, error },
  onChange,
}) =>
{
  const textAreaChangeHandler = useCallback((event: SyntheticEvent, data: TextAreaProps) =>
  {
    input.onChange(data.value);
    if (onChange) onChange(event, data);
  }, [input, onChange]);

  return (
    <Form.Field error={touched && error} width={width} style={{ fontWeight: 'bold' }}>
      <div style={{ marginBottom: '0.3em' }}>{label}</div>
      <TextArea
        rows={rows}
        {...input}
        readOnly={readOnly}
        disabled={disabled}
        placeholder={placeholder}
        style={{ fontWeight: 'normal' }}
        onChange={textAreaChangeHandler}
      />
      {touched && error && (
        <Label basic color='red'>
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default TextAreaField;
