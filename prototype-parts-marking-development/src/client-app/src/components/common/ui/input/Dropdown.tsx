import _ from 'lodash';
import React, { AllHTMLAttributes, SyntheticEvent, useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { ConfiguredValidator } from 'revalidate';
import
{
  Dropdown,
  DropdownItemProps,
  DropdownProps,
  Icon,
  Label,
  LabelProps,
  StrictDropdownProps,
} from 'semantic-ui-react';

import styles from './Dropdown.module.css';

export type DropdownValue = boolean | number | string | (boolean | number | string)[];

export interface IDropdownComponentProps extends
  Omit<StrictDropdownProps, 'options'>,
  Omit<AllHTMLAttributes<HTMLElement>, keyof StrictDropdownProps>
{
  allowChangeEventOnlyWhenClosed?: boolean;
  options?: DropdownItemProps[] | ((value: string) => DropdownItemProps[] | Promise<DropdownItemProps[]>);
  validator?: ConfiguredValidator;
}

const createErrorPseudoItem = (error: string) =>
{
  return {
    disabled: true, // prevent selection of this pseudo-item
    text: `Invalid search query: ${error}`,
    icon: { name: 'exclamation triangle' },
    // style: { opacity: 1, color: 'red', pointerEvents: 'none' }, // prevent graying out this pseudo-item
    className: styles.ErrorItem,
  };
};

const renderDropdownActiveItemLabel = (item: DropdownItemProps, _index: number, defaultLabelProps: LabelProps) =>
{
  const labelProps = { ...defaultLabelProps };
  labelProps.content = (
    <div style={{ display: 'flex', alignItems: 'center', minHeight: '1em' }}>
      <div style={{ float: 'left', height: '100%' }}>{item.text}</div>
      <div style={{ float: 'right', fontSize: '0.92857143em', padding: '0 0 0 0.5em', height: '100%' }}>
        <Icon
          link
          fitted
          name='delete'
          style={{ opacity: '0.5' }}
          onClick={(e: React.MouseEvent<HTMLElement, MouseEvent>) =>
          {
            if (defaultLabelProps.onRemove) defaultLabelProps.onRemove(e, labelProps);
          }
          }
        />
      </div>
    </div>
  );
  labelProps.style = { textDecoration: 'none', fontSize: '0.85em' };
  labelProps.removeIcon = null;
  // remove selection also if clicked on label (not only on remove icon)
  labelProps.onClick = defaultLabelProps.onRemove;

  return <Label {...labelProps} />;
};

const DropdownComponent: React.FC<IDropdownComponentProps> = ({
  allowChangeEventOnlyWhenClosed,
  open,
  allowAdditions,
  multiple,
  options,
  value,
  validator,
  onSearchChange,
  onChange,
  onOpen,
  onClose,
  ...dropdownProps
}) =>
{
  const isOpenedRef = useRef(!!open);

  const [internalOptions, setInternalOptions] = useState(Array.isArray(options) ? options : []);
  const [selectedOptions, setSelectedOptions] = useState<DropdownItemProps[]>([]);
  const [selection, setSelection] = useState<DropdownValue>();
  const [selectionOnOpen, setSelectionOnOpen] = useState<DropdownValue>();

  // options can be either undefined or statically defined via property
  // or it can be a function returning list of available options
  const searchProps: StrictDropdownProps = useMemo(() =>
  {
    if (typeof options !== 'function') return { onSearchChange: onSearchChange };

    return {
      // override default search function to return all options
      // (already pre-filtered by search function provided in options property)
      search: (options: DropdownItemProps[]) => options,
      // set options as result of function (provided in options property) that fetches options (e.g. from server)
      onSearchChange: (event, data) =>
      {
        const issue = validator && validator(data.searchQuery);
        if (issue)
        {
          setInternalOptions([createErrorPseudoItem(issue)]);
        }
        else
        {
          Promise.resolve(options(data.searchQuery)).then(options => setInternalOptions(options));
        }

        if (onSearchChange) onSearchChange(event, data);
      },
    };
  }, [options, validator, onSearchChange]);

  // whole options need to be stored and added to current internalOptions
  // otherwise Dropdown would not render labels correctly
  // because it will contain values in selection for which there is no option in internalOptions
  // e.g. search query has changed and results of search query doesn't contain some of selected values anymore
  const updateSelectedOptions = useCallback((selection: (string | number | boolean)[] | undefined) =>
  {
    setSelectedOptions(prevSelectedOptions =>
    {
      // remove old stored options which are not selected anymore
      const oldSelectedOptions = prevSelectedOptions.filter(option =>
        option.value && selection?.includes(option.value));

      // store whole option for each selected value
      const newSelectedOptions = internalOptions.filter(option =>
        option.value && selection?.includes(option.value));

      // generate options with same value and label to be used for selections without corresponding option available
      const generatedSelectedOptions = selection?.map(value => { return { text: value, value: value }; });

      // use union to not create duplicate options
      return _.unionBy(oldSelectedOptions, newSelectedOptions, generatedSelectedOptions, option => option.value);
    });
  }, [internalOptions]);

  // update internal options from property
  useEffect(() =>
  {
    if (typeof options === 'function')
    {
      Promise.resolve(options('')).then(options => setInternalOptions(options));
    }
    else
    {
      const newOptions = options ?? []; // must not be undefined when selection property is set
      setInternalOptions(prevOptions => _.isEqual(prevOptions, newOptions) ? prevOptions : newOptions);
    }
  }, [options]);

  // update selection from value property
  useEffect(() =>
  {
    // value must be an array when property multiple is set to true
    const newValue = (multiple && !value) ? [] : value;

    // update internal selection from property
    setSelection(prevSelection => _.isEqual(prevSelection, newValue) ? prevSelection : newValue);

    // store whole options for selected values (see comment for function updateSelectedOptions)
    // this will stop working when all options are not listed for empty search query (e.g. when mounted)
    const selectionArray = Array.isArray(newValue) || newValue === undefined ? newValue : [newValue];
    updateSelectedOptions(selectionArray);

    // updateSelectedOptions is omitted from dependencies array to not run this effect on internalOptions change
    // otherwise the selection will be reset to property value when options are dynamically retrieved on search change
    // that would effectively remove all items added since last onChange call
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [multiple, value]);

  const changeHandler = useMemo(() =>
  {
    if (!onChange) return undefined;
    if (!allowChangeEventOnlyWhenClosed) return onChange;

    return (event: SyntheticEvent<HTMLElement>, data: DropdownProps) =>
    {
      setSelection(data.value);

      // store whole options for selected values (see comment for function updateSelectedOptions)
      const selectionArray = Array.isArray(data.value) || data.value === undefined ? data.value : [data.value];
      updateSelectedOptions(selectionArray);

      if (!isOpenedRef.current)
      {
        onChange(event, data);
      }
    };
  }, [allowChangeEventOnlyWhenClosed, isOpenedRef, updateSelectedOptions, onChange]);

  const openHandler = (event: SyntheticEvent<HTMLElement>, data: DropdownProps) =>
  {
    isOpenedRef.current = true;
    if (Array.isArray(selection))
    {
      setSelectionOnOpen([...selection]);
    }
    else
    {
      setSelectionOnOpen(selection);
    }

    if (onOpen) onOpen(event, data);
  };

  const closeHandler = (event: SyntheticEvent<HTMLElement>, data: DropdownProps) =>
  {
    if (onClose) onClose(event, data);

    isOpenedRef.current = false;
    if (allowChangeEventOnlyWhenClosed)
    {
      if (!_.isEqual(selection, selectionOnOpen) && changeHandler)
      {
        changeHandler(event, { ...data, value: selection });
      }

      // search query is cleared on close
      // therefore function which fetches filtered options have to be called with empty search query
      // to fetch options corresponding to empty search query value
      if (searchProps.onSearchChange) searchProps.onSearchChange(event, { ...data, searchQuery: '' });
    }
  };

  const additionHandler = useCallback((event: SyntheticEvent<HTMLElement>, data: DropdownProps) =>
  {
    // added value doesn't need to be added to options here
    // because it's added in updateSelectedOptions called from changeHandler

    // search query is cleared on user addition to the list of options
    // therefore function which fetches filtered options have to be called with empty search query
    // to fetch options corresponding to empty search query value
    if (searchProps.onSearchChange) searchProps.onSearchChange(event, { ...data, searchQuery: '' });
  }, [searchProps]);

  return (
    <Dropdown
      basic
      clearable
      allowAdditions={allowAdditions}
      selection
      multiple={multiple}
      scrolling
      wrapSelection
      // use union to remove duplicates when selected value is also found in options provided from outside
      // because it matches current search query
      options={_.unionBy(internalOptions, selectedOptions, option => option.value)}
      value={selection}
      search
      onAddItem={additionHandler}
      onOpen={openHandler}
      onClose={closeHandler}
      onChange={changeHandler}
      renderLabel={renderDropdownActiveItemLabel}
      {...searchProps}
      {...dropdownProps}
    />
  );
};

export default DropdownComponent;
