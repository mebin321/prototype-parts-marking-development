import React, { FormEvent, Fragment, SyntheticEvent, useCallback, useEffect, useState } from 'react';
import { FieldRenderProps } from 'react-final-form';
import { Form, FormFieldProps, InputOnChangeData, Label, Popup } from 'semantic-ui-react';

import { evaluateTextInsertion } from '../../../utilities/text';

interface ITextFieldProps
  extends FieldRenderProps<string>, FormFieldProps
{
  readOnly?: boolean;
  autoFocus?: boolean;
  pattern?: RegExp;
  errorStyle?: 'label' | 'popup';
  uppercase?: boolean;
  onChange?: (event: SyntheticEvent, data: InputOnChangeData) => void;
}

const TextField: React.FC<ITextFieldProps> = ({
  input,
  readOnly,
  disabled,
  pattern,
  errorStyle = 'label',
  uppercase,
  autoFocus,
  width,
  type,
  placeholder,
  label,
  meta: { error, touched },
  onChange,
}) =>
{
  const [isErrorAllowed, setErrorAllowed] = useState(false);

  // re-display error popup when input is re-enabled
  useEffect(() =>
  {
    if (!disabled)
    {
      setErrorAllowed(true);
    }
  }, [disabled]);

  const beforeInputHandler = useCallback((event: FormEvent<HTMLInputElement>) =>
  {
    if (!pattern) return;

    const target = event.target as HTMLInputElement;
    const key = (event as any).data;

    if (!target || key === undefined || key === null) return;

    const newValue = evaluateTextInsertion(target.value, target.selectionStart, target.selectionEnd, key);
    if (!pattern.test(newValue))
    {
      event.preventDefault();
    }
  }, [pattern]);

  const textBoxInputHandler = useCallback((event: FormEvent<HTMLInputElement>) =>
  {
    if (uppercase)
    {
      const target = event.target as HTMLInputElement;
      const carretPosition = target.selectionStart;
      target.value = target.value.toUpperCase();
      target.setSelectionRange(carretPosition, carretPosition);
    }
  }, [uppercase]);

  const textBoxChangeHandler = useCallback((event: SyntheticEvent, data: InputOnChangeData) =>
  {
    input.onChange(data.value);
    if (onChange) onChange(event, data);
  }, [input, onChange]);

  const formInput = (
    <Form.Input
    {...input}
    readOnly={readOnly}
    disabled={disabled}
    label={label}
    input={{ autoFocus: autoFocus, style: uppercase ? { textTransform: 'none' } : undefined }}
    placeholder={placeholder}
    onBeforeInput={pattern && beforeInputHandler}
    onInput={textBoxInputHandler}
    onChange={textBoxChangeHandler}
    onBlur={(e: React.FocusEvent<HTMLElement>) =>
    {
      setErrorAllowed(true); // re-display error popup when input focus is lost
      if (input.onBlur) input.onBlur(e);
    }}
  />);

  const errorLabel = touched && !disabled && error && (
    <Label
      basic
      color='red'
      onClick={errorStyle === 'popup' ? () => setErrorAllowed(false) : undefined}
      onRemove={errorStyle === 'popup' ? () => setErrorAllowed(false) : undefined}
      content={error}
    />);

  const popup = (
    <Popup
      basic
      flowing
      pinned={false}
      position='bottom left'
      style={{ border: '0 none transparent', padding: 0 }}
      open={touched && !disabled && !!error && isErrorAllowed}
      openOnTriggerClick={false}
      openOnTriggerFocus={false}
      openOnTriggerMouseEnter={false}
      closeOnDocumentClick={false}
      closeOnEscape={false}
      closeOnPortalMouseLeave={false}
      closeOnTriggerBlur={false}
      closeOnTriggerClick={false}
      closeOnTriggerMouseLeave={false}
      trigger={formInput}>
      {errorLabel}
    </Popup>);

  return (
    <Form.Field error={touched && error} type={type} width={width}>
      {errorStyle === 'label' && (
        <Fragment>
          {formInput}
          {errorLabel}
        </Fragment>
      )}
      {errorStyle === 'popup' && (
        <Fragment>
          {popup}
        </Fragment>
      )}
    </Form.Field>
  );
};

export default TextField;
