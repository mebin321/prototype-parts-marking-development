import { FormApi } from 'final-form';
import React, { CSSProperties } from 'react';
import { Field, Form as FinalForm } from 'react-final-form';
import { combineValidators, isRequired } from 'revalidate';
import { Button, Form, Header, Icon, Segment } from 'semantic-ui-react';

import TextAreaField from '../../../common/form/TextAreaField';

interface IAddVariantFormProps
{
  version: number;
  disabled?: boolean;
  submitting?: boolean;
  style?: CSSProperties;
  onCancel?: () => void;
  onSubmit?: (comment: string, resetForm: () => void) => void;
}

const validate = combineValidators({
  comment: isRequired('Comment'),
});

const AddVariantForm: React.FC<IAddVariantFormProps> = ({
  version,
  disabled,
  submitting,
  style,
  onCancel,
  onSubmit,
}) =>
{
  const finalFormSubmitHandler = async (values: any, form: FormApi) =>
  {
    if (onSubmit) onSubmit(values.comment, () => resetForm(form));
  };

  const resetForm = (form: FormApi) =>
  {
    form.reset({ comment: '' });
    for (const fieldName of form.getRegisteredFields())
    {
      form.resetFieldState(fieldName);
    }
  };

  return (
    <FinalForm
      onSubmit={finalFormSubmitHandler}
      validate={validate}
      render={({ handleSubmit, invalid, pristine }) => (
        <Segment clearing style={{ position: 'relative', ...style }}>
          <Header size='medium' style={{ display: 'inline' }}>v{version}&nbsp;&nbsp;</Header>
          <Icon
            link
            fitted
            name='close'
            onClick={onCancel}
            style={{ position: 'absolute', top: '0.7em', right: '1em' }}
          />
          <Form onSubmit={handleSubmit}>
            <Field
              name='comment'
              placeholder='Variant comment'
              disabled={disabled}
              component={TextAreaField}
            />
            <Button
              content='Create Variant'
              type='submit'
              floated='right'
              color='green'
              disabled={disabled || pristine || invalid}
              loading={submitting}
            />
          </Form>
        </Segment>
      )}
    />
  );
};

export default AddVariantForm;
