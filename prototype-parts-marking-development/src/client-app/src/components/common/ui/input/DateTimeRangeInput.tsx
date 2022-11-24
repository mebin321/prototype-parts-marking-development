import React, { InputHTMLAttributes, SyntheticEvent } from 'react';
import { Input, InputOnChangeData, InputProps, StrictInputProps } from 'semantic-ui-react';

import { DateInterval } from '../../../../models/common/interval';
import { formatDateTimeToInput } from '../../../../utilities/datetime';
import { parseReactDate } from '../../../../utilities/react';

const determineLowerBoundMax = (max: Date | undefined, intervalUpperBound: Date | undefined) =>
{
  if (max !== undefined && intervalUpperBound !== undefined)
  {
    return new Date(Math.min(max.getTime(), intervalUpperBound.getTime()));
  }

  if (max === undefined && intervalUpperBound !== undefined)
  {
    return intervalUpperBound;
  }

  if (max !== undefined && intervalUpperBound === undefined)
  {
    return max;
  }

  return undefined;
};

const determineUpperBoundMin = (min: Date | undefined, intervalLowerBound: Date | undefined) =>
{
  if (min !== undefined && intervalLowerBound !== undefined)
  {
    return new Date(Math.max(min.getTime(), intervalLowerBound.getTime()));
  }

  if (min === undefined && intervalLowerBound !== undefined)
  {
    return intervalLowerBound;
  }

  if (min !== undefined && intervalLowerBound === undefined)
  {
    return min;
  }

  return undefined;
};

interface IDateTimeRangeInputProps extends Omit<InputProps, 'onChange'>,
  Omit<InputHTMLAttributes<HTMLInputElement>, keyof StrictInputProps | 'value'>
{
  value?: DateInterval;
  onChange?: (value: DateInterval) => void;
}

const DateTimeRangeInput: React.FC<IDateTimeRangeInputProps> = ({
  value,
  min,
  max,
  onChange,
  ...props
}) =>
{
  const lowerBoundChangeHandler = (_event: SyntheticEvent, data: InputOnChangeData) =>
  {
    if (!onChange) return;

    const newValue = { ...value };
    const lowerBound = parseReactDate(data.value);
    if (!lowerBound)
    {
      delete newValue.lowerBound;
    }
    else
    {
      newValue.lowerBound = lowerBound;
    }

    onChange(newValue);
  };

  const upperBoundChangeHandler = (_event: SyntheticEvent, data: InputOnChangeData) =>
  {
    if (!onChange) return;

    const newValue = { ...value };
    const upperBound = parseReactDate(data.value);
    if (!upperBound)
    {
      delete newValue.upperBound;
    }
    else
    {
      newValue.upperBound = upperBound;
    }

    onChange(newValue);
  };

  const minLimit = parseReactDate(min);
  const maxLimit = parseReactDate(max);

  return (
    <div>
      <div style={{ margin: '0 0 0.15em 0' }}>From</div>
      <Input
        type='datetime-local'
        name='lowerBound'
        title={'Enter lower bound value or leave empty if lowest value shouldn\'t be limited'}
        min={minLimit}
        max={determineLowerBoundMax(maxLimit, value?.upperBound)}
        value={formatDateTimeToInput(value?.lowerBound)}
        onChange={lowerBoundChangeHandler}
        {...props}
      />

      <div style={{ margin: '0.6em 0 0.15em 0' }}>To</div>
      <Input
        type='datetime-local'
        name='upperBound'
        title={'Enter upper bound value or leave empty if highest value shouldn\'t be limited'}
        min={determineUpperBoundMin(minLimit, value?.lowerBound)}
        max={maxLimit}
        value={formatDateTimeToInput(value?.upperBound)}
        onChange={upperBoundChangeHandler}
        {...props}
      />
    </div>
  );
};

export default DateTimeRangeInput;
