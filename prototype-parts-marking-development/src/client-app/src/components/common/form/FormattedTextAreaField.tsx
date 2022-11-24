import React, { SyntheticEvent } from 'react';
import { FieldRenderProps } from 'react-final-form';
import { Form, FormFieldProps, Label, TextAreaProps } from 'semantic-ui-react';

import FormattedTextArea from '../../../containers/formatedTextArea/FormattedTextArea';

interface ITextAreaFieldProps
  extends FieldRenderProps<string>, FormFieldProps
{
  readOnly?: boolean;
  onChange?: (event: SyntheticEvent, data: TextAreaProps) => void;
}

const FormattedTextAreaField: React.FC<ITextAreaFieldProps> = ({
  input,
  readOnly,
  width,
  placeholder,
  label,
  meta: { touched, error },
}) =>
{
  return (
    <Form.Field error={touched && error} width={width} style={{ fontWeight: 'bold' }}>
      <div style={{ marginBottom: '0.3em' }}>{label}</div>
      <FormattedTextArea
        input={input}
        readOnly={readOnly}
        placeholder={placeholder}
      />
      {touched && error && (
        <Label basic color='red'>
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default FormattedTextAreaField;
