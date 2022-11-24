import React, { Fragment, SyntheticEvent, useEffect, useRef, useState } from 'react';
import { Field, Form as FinalForm } from 'react-final-form';
import { combineValidators, composeValidators, isRequired } from 'revalidate';
import { CheckboxProps, InputOnChangeData } from 'semantic-ui-react';

import CheckBoxField from '../../../../components/common/form/CheckBoxField';
import TextField from '../../../../components/common/form/TextField';
import { IPartType } from '../../../../models/api/enumerations/parts';
import { isMaterialNumber, isRevisionCode } from '../../../../utilities/validation/validators';
import { materialNumberInputPattern, revisionCodeInputPattern } from '../../itemsUtilities';

const validate = combineValidators({
  materialNumber: composeValidators(isRequired, isMaterialNumber)('Material Number'),
  revisionCode: composeValidators(isRequired, isRevisionCode)('Revision Code'),
});

export interface IComponentData
{
  partType: IPartType;
  materialNumber: string;
  revisionCode: string;
}

interface IComponentDefinition
{
  partType: IPartType;
  selected: boolean;
  disabled: boolean;
  onChange?: (isSelected: boolean, data: IComponentData) => void;
}

const ComponentDefinition: React.FC<IComponentDefinition> = ({
  partType,
  selected,
  disabled,
  onChange,
}) =>
{
  const allowOnChangeRef = useRef(false);

  const [isSelected, setSelected] = useState(selected);
  const [materialNumber, setMaterialNumber] = useState('');
  const [revisionCode, setRevisionCode] = useState('');

  useEffect(() =>
  {
    setSelected(selected);
  }, [selected]);

  useEffect(() =>
  {
    // prevent calling onChange when selected property is set on mount
    if (!allowOnChangeRef.current)
    {
      allowOnChangeRef.current = true;
      return;
    }

    if (onChange) onChange(isSelected, { partType, materialNumber, revisionCode });
    // onChange is omitted in dependencies to prevent firing onChange when onChange handler is changed
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isSelected, materialNumber, revisionCode]);

  return (
    <FinalForm
      validate={validate}
      onSubmit={() => {}}
      render={() => (
        <Fragment>
          <div>
            <Field
              label={partType.title}
              name={partType.moniker}
              checked={selected}
              disabled={disabled}
              onChange={(_e: SyntheticEvent, d: CheckboxProps) => setSelected(d.checked ?? false)}
              component={CheckBoxField}
            />
          </div>
          <label style={{ paddingLeft: '1em', fontWeight: 700, opacity: isSelected ? 1 : 0.2 }}>
            Material Number
          </label>
          <div style={{ display: 'inline', width: '16em', minWidth: '16em' }}>
            <Field
              name='materialNumber'
              pattern={materialNumberInputPattern}
              uppercase
              disabled={!isSelected}
              value={materialNumber}
              errorStyle='popup'
              onChange={(_e: SyntheticEvent, data: InputOnChangeData) => setMaterialNumber(data.value?.toUpperCase())}
              component={TextField}
            />
          </div>
          <label style={{ paddingLeft: '1em', fontWeight: 700, opacity: isSelected ? 1 : 0.2 }}>
            Revision Code
          </label>
          <div style={{ display: 'inline', width: '4em', minWidth: '4em' }}>
            <Field
              name='revisionCode'
              pattern={revisionCodeInputPattern}
              disabled={!isSelected}
              value={revisionCode}
              errorStyle='popup'
              onChange={(_e: SyntheticEvent, data: InputOnChangeData) => setRevisionCode(data.value)}
              component={TextField}
            />
          </div>
        </Fragment>
      )}
    />
  );
};

export default ComponentDefinition;
