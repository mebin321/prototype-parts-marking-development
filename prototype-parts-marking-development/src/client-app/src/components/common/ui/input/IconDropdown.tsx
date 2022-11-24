import React, { SyntheticEvent, useCallback, useState } from 'react';
import { Dropdown, DropdownItemProps, DropdownProps, Icon, SemanticICONS } from 'semantic-ui-react';

export interface IIconDropdownItemProps extends Omit<DropdownItemProps, 'icon'>
{
  icon: SemanticICONS; // force icon to be of type SemanticICONS
}

export interface IIconDropdownProps extends Omit<DropdownProps, 'options' | 'icon'>
{
  options: IIconDropdownItemProps[]; // force options to be of type IIconDropdownItemProps
  defaultValue?: string | number | boolean;
  value?: string | number | boolean;
}

const IconDropdown: React.FC<IIconDropdownProps> = ({
  defaultValue,
  value,
  options,
  style,
  onChange,
  ...props
}) =>
{
  const defaultSelection = defaultValue !== undefined ? defaultValue : value;
  const [selection, setSelection] = useState(defaultSelection);

  const getSelectedItemIcon = () =>
  {
    const selectedOption = options.find(option => option.value === selection);
    if (!selectedOption) return;

    return selectedOption.icon;
  };

  const changeHandler = useCallback((event: SyntheticEvent<HTMLElement>, data: DropdownProps) =>
  {
    // multi-value dropdown is not supported
    if (Array.isArray(data.value)) return;

    setSelection(data.value);
    if (onChange) onChange(event, data);
  }, [onChange]);

  // use Icon.Group to show multiple icons but not span to full parent height
  // wrap each Icon to div to prevent Icon.Group stacking behavior
  const selectionIcon = (
    <Icon.Group>
      <div style={{ display: 'inline' }}>
        <Icon fitted name={getSelectedItemIcon()} />
      </div>
      <div style={{ display: 'inline-flex', marginLeft: '1em', fontSize: '0.85714286em' }}>
        <Icon fitted name='dropdown' />
      </div>
    </Icon.Group>
  );

  return (
    <Dropdown
      {...props}
      icon={selectionIcon}
      text='&#x200B;' // force no text even if dropdown item has text property defined
      options={options}
      style={{ width: 'max-content', ...style }} // prevent dropdown icon wrapping
      onChange={changeHandler}
    />
  );
};

export default IconDropdown;
