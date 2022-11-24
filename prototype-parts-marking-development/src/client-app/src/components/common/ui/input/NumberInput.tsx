import _ from 'lodash';
import React, { FormEvent, SyntheticEvent, useCallback, useEffect, useState } from 'react';
import { Checkbox, Input, InputOnChangeData, InputProps } from 'semantic-ui-react';

import { NumberInterval, formatNumberInterval, parseNumberInterval } from '../../../../models/common/interval';
import InputWithEditor from './InputWithEditor';
import NumberRangeInput from './NumberRangeInput';

const digitKeys = Object.freeze(['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']);

const isDigitKeyPressed = (key: string) =>
  digitKeys.includes(key);

interface INumberInputProps extends Omit<InputProps, 'onChange'>
{
  range?: boolean;
  placeholder?: string;
  editorPlaceholder?: string;
  min?: number;
  defaultValue?: string | number;
  value?: string | number;
  onBeforeInput?: (event: FormEvent<HTMLInputElement>) => void;
  onChange?: (value: NumberInterval) => void;
}

const NumberInput: React.FC<INumberInputProps> = ({
  // fluid must be applied only to main input only, but not to inputs in editor
  // otherwise inputs in editor will adapt to the editor width
  // and will not force the editor to stretch to fit whole inputs
  fluid,
  range,
  min,
  placeholder,
  editorPlaceholder,
  defaultValue,
  value,
  onBeforeInput,
  onChange,
  ...props
}) =>
{
  const [internalValue, setInternalValue] = useState(defaultValue?.toString() ?? value?.toString() ?? '');
  const [showRangeEditor, setShowRangeEditor] = useState(false);

  useEffect(() =>
  {
    setInternalValue(prevValue => _.isEqual(prevValue, value) ? prevValue : value?.toString() ?? '');
  }, [value]);

  const isRangeSeparatorEnteredAndValid = useCallback((event: FormEvent<HTMLInputElement>) =>
  {
    const target = event.target as HTMLInputElement;
    const key = (event as any).data;
    if (!target || key === undefined || key === null) return;

    const inputValue = target.value ?? '';

    // dash is range separator
    if (key !== '-') return false;

    // dash is allowed only when input can contain number range
    if (!range) return false;

    // if input can contain number range then allow dash only once
    return !inputValue?.includes('-');
  }, [range]);

  const beforeInputHandler = useCallback((event: FormEvent<HTMLInputElement>) =>
  {
    const key = (event as any).data;

    if (!isDigitKeyPressed(key) &&
      !isRangeSeparatorEnteredAndValid(event))
    {
      event.preventDefault();
    }

    if (onBeforeInput) onBeforeInput(event);
  }, [isRangeSeparatorEnteredAndValid, onBeforeInput]);

  const toggleShowRangeEditor = useCallback(() =>
  {
    setShowRangeEditor(prevShowRange => !prevShowRange);
  }, []);

  const changeHandler = useCallback((value: NumberInterval) =>
  {
    setInternalValue(formatNumberInterval(value));

    if (onChange) onChange(value);
  }, [onChange]);

  const inputChangeHandler = useCallback((_event: SyntheticEvent, data: InputOnChangeData) =>
  {
    const value = data.value?.trim();
    if (range)
    {
      // switch editor to range if value contains range separator and range editor is not shown already
      if (!showRangeEditor && value.includes('-')) setShowRangeEditor(true);

      // bypass changeHandler to not loose range separator
      if (value === '-')
      {
        setInternalValue(value);
        if (onChange) onChange({});
        return;
      }
    }

    changeHandler(parseNumberInterval(value));
  }, [range, showRangeEditor, onChange, changeHandler]);

  // if range is allowed value must not be negative, otherwise it couldn't tell minus sign apart range separator
  let minValue = min;
  if (range)
  {
    minValue = min === undefined ? 0 : Math.max(min, 0);
  }

  return (
      <InputWithEditor
        type='text'
        fluid={fluid}
        title={internalValue}
        placeholder={placeholder}
        editorDisabled={!range}
        onBeforeInput={beforeInputHandler}
        onChange={inputChangeHandler}
        {...props}
        // overwrite value property using locally remembered value
        // otherwise when range separator (dash) is entered it would be overwritten by empty string
        // because lower bound and upper bound is undefined when range separator is entered without values
        // and therefore the string representation is ''
        value={internalValue}
      >
        {range &&
          <Checkbox
            label='Range'
            style={{ padding: '1em 0' }}
            checked={showRangeEditor}
            onChange={toggleShowRangeEditor}
          />
        }

        {showRangeEditor
          ? <NumberRangeInput
              min={minValue}
              value={internalValue}
              placeholder={editorPlaceholder || placeholder}
              style={{ width: '6em' }}
              onChange={changeHandler}
              {...props}
            />
          : <Input
              fluid
              type='number'
              min={minValue}
              value={internalValue}
              placeholder={editorPlaceholder || placeholder}
              style={{ width: '6em' }}
              onChange={inputChangeHandler}
              {...props}
            />

        }
      </InputWithEditor>
  );
};

export default NumberInput;
