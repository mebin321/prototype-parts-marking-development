import React, { useCallback } from 'react';
import { Icon, InputProps } from 'semantic-ui-react';

import { DateInterval, formatDateInterval } from '../../../../models/common/interval';
import DateTimeRangeInput from './DateTimeRangeInput';
import InputWithEditor from './InputWithEditor';

interface IDateTimeInputProps extends Omit<InputProps, 'onChange'>
{
  value?: DateInterval;
  onChange?: (value: DateInterval) => void;
}

const DateTimeInput: React.FC<IDateTimeInputProps> = ({
  // fluid must be applied only to main input only, but not to inputs in DateTimeRangeInput
  // otherwise inputs in editor will adapt to the editor width
  // and will not force the editor (container element of DateTimeRangeInput) to stretch to fit whole inputs
  fluid,
  value,
  onChange,
  ...props
}) =>
{
  const clearInputHandler = useCallback(() =>
  {
    if (onChange) onChange({});
  }, [onChange]);

  const isValueFilled = value?.lowerBound || value?.upperBound;

  return (
    <InputWithEditor
      type='text'
      fluid={fluid}
      readOnly
      icon={isValueFilled && <Icon link name='remove' onClick={clearInputHandler} />}
      title={formatDateInterval(value)}
      value={formatDateInterval(value)}
      {...props}
    >
      <DateTimeRangeInput value={value} onChange={onChange} {...props} />
    </InputWithEditor>
  );
};

export default DateTimeInput;
