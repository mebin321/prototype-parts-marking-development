import React, { InputHTMLAttributes, SyntheticEvent } from 'react';
import { Input, InputOnChangeData, InputProps, StrictInputProps } from 'semantic-ui-react';

import { NumberInterval, parseNumberInterval } from '../../../../models/common/interval';
import { parseReactNumber } from '../../../../utilities/react';

const determineLowerBoundMax = (max: number | undefined, intervalUpperBound: number | undefined) =>
{
  if (max !== undefined && intervalUpperBound !== undefined)
  {
    return Math.min(max, intervalUpperBound);
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

const determineUpperBoundMin = (min: number | undefined, intervalLowerBound: number | undefined) =>
{
  if (min !== undefined && intervalLowerBound !== undefined)
  {
    return Math.max(min, intervalLowerBound);
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

interface INumberRangeInputProps extends Omit<InputProps, 'onChange'>,
  Omit<InputHTMLAttributes<HTMLInputElement>, keyof StrictInputProps>
{
  value?: string;
  onChange?: (value: NumberInterval) => void;
}

const NumberRangeInput: React.FC<INumberRangeInputProps> = ({
  value,
  min,
  max,
  onChange,
  ...props
}) =>
{
  const interval = parseNumberInterval(value);

  const lowerBoundChangeHandler = (_event: SyntheticEvent, data: InputOnChangeData) =>
  {
    if (!onChange) return;

    const newInterval = { ...interval };
    const lowerBound = Number.parseInt(data.value);
    if (isNaN(lowerBound))
    {
      delete newInterval.lowerBound;
    }
    else
    {
      newInterval.lowerBound = lowerBound;
    }

    onChange(newInterval);
  };

  const upperBoundChangeHandler = (_event: SyntheticEvent, data: InputOnChangeData) =>
  {
    if (!onChange) return;

    const newInterval = { ...interval };
    const upperBound = Number.parseInt(data.value);
    if (isNaN(upperBound))
    {
      delete newInterval.upperBound;
    }
    else
    {
      newInterval.upperBound = upperBound;
    }

    onChange(newInterval);
  };

  const minLimit = parseReactNumber(min);
  const maxLimit = parseReactNumber(max);

  return (
    <div>
      <div style={{ margin: '0 0 0.15em 0' }}>From</div>
      <Input
        fluid
        type='number'
        name='lowerBound'
        title={'Enter lower bound value or leave empty if lowest value shouldn\'t be limited'}
        min={minLimit}
        max={determineLowerBoundMax(maxLimit, interval.upperBound)}
        value={interval.lowerBound ?? ''} // don't pass undefined to not confuse React that input is uncontrolled
        onChange={lowerBoundChangeHandler}
        {...props}
      />

      <div style={{ margin: '0.6em 0 0.15em 0' }}>To</div>
      <Input
        fluid
        type='number'
        name='upperBound'
        title={'Enter upper bound value or leave empty if highest value shouldn\'t be limited'}
        min={determineUpperBoundMin(minLimit, interval.lowerBound)}
        max={maxLimit}
        value={interval.upperBound ?? ''} // don't pass undefined to not confuse React that input is uncontrolled
        onChange={upperBoundChangeHandler}
        {...props}
      />
    </div>
  );
};

export default NumberRangeInput;
