import React, { FormEvent, useCallback, useEffect, useState } from 'react';
import { ConfiguredValidator } from 'revalidate';
import { Input, InputProps, Label } from 'semantic-ui-react';

import { evaluateTextInsertion } from '../../../../utilities/text';

interface ITextValidatingInputProps extends InputProps
{
  pattern?: RegExp;
  validator?: ConfiguredValidator;
}

const TextValidatingInput: React.FC<ITextValidatingInputProps> = ({
  pattern,
  validator,
  value,
  onBeforeInput,
  ...props
}) =>
{
  const [validationMessage, setValidationMessage] = useState(true);

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

    if (onBeforeInput) onBeforeInput(event);
  }, [pattern, onBeforeInput]);

  useEffect(() =>
  {
    setValidationMessage(value ? validator?.(value) : undefined);
  }, [value, validator]);

  return (
    <div>
      <Input
        type='text'
        value={value}
        onBeforeInput={beforeInputHandler}
        {...props}
      />
      {validationMessage &&
        <Label
          basic
          style={{ margin: 0 }}
          color='orange'
          icon='warning sign'
          content={validationMessage}
        />
      }
    </div>
  );
};

export default TextValidatingInput;
