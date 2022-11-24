import _ from 'lodash';
import React, { HTMLAttributes, ReactNode, useCallback, useEffect, useRef, useState } from 'react';
import { Button, Header, Segment } from 'semantic-ui-react';

import styles from './DualListBox.module.css';

interface IListBoxItem
{
  readonly value: string;
  readonly label: string;
}

interface IDualListBoxProps extends Omit<HTMLAttributes<HTMLElement>, 'onChange'>
{
  optionsLabel?: string | ReactNode;
  selectionLabel?: string | ReactNode;
  options: IListBoxItem[];
  selection?: IListBoxItem[];
  onChange?: (selection: IListBoxItem[]) => void;
}

const retrieveSelectedOptions = (select: HTMLSelectElement | undefined | null) =>
{
  if (!select) return [];

  const selection: string[] = [];

  for (let i = 0; i < select.options.length; i++)
  {
    const option = select.options.item(i)!;
    if (option.selected)
    {
      selection.push(option.value);
    }
  }

  return selection;
};

const prepareLabel = (label: string | ReactNode) =>
{
  if (label === undefined) return <div />;
  if (label === null) return <div />;
  if (typeof label === 'boolean' || typeof label === 'string')
  {
    return <Header size='tiny' style={{ margin: '1em 0' }}>{String(label)}</Header>;
  }

  return label;
};

const DualListBox: React.FC<IDualListBoxProps> = ({
  optionsLabel,
  selectionLabel,
  options,
  selection,
  onChange,
  ...props
}) =>
{
  const [internalSelection, setInternalSelection] = useState(selection ?? []);
  const optionsRef = useRef<HTMLSelectElement>(null);
  const selectionRef = useRef<HTMLSelectElement>(null);

  // update internal selection from property
  useEffect(() =>
  {
    const newSelection = selection ?? [];
    setInternalSelection(prevSelection => _.isEqual(prevSelection, newSelection) ? prevSelection : newSelection);
  }, [selection]);

  const isItemSelected = useCallback((itemToCheck: IListBoxItem) =>
  {
    return internalSelection.findIndex(item => item.value === itemToCheck.value) >= 0;
  }, [internalSelection]);

  const addItems = useCallback((values: string[]) =>
  {
    if (!values || values.length < 1) return;

    setInternalSelection(prevSelection =>
    {
      const newSelection = [...prevSelection];
      for (const addedValue of values)
      {
        const addedItem = options.find(item => item.value === addedValue);
        if (!addedItem)
        {
          continue;
        }

        newSelection.push(addedItem);
      }

      if (onChange) onChange(newSelection);
      return newSelection;
    });
  }, [options, onChange]);

  const removeItems = useCallback((values: string[]) =>
  {
    if (!values || values.length < 1) return;

    setInternalSelection(prevSelection =>
    {
      const newSelection = [...prevSelection];
      for (const removedValue of values)
      {
        const removedItemIndex = newSelection.findIndex(item => item.value === removedValue);
        if (removedItemIndex < 0)
        {
          continue;
        }

        newSelection.splice(removedItemIndex, 1);
      }

      if (onChange) onChange(newSelection);
      return newSelection;
    });
  }, [onChange]);

  const addItemsHandler = useCallback(() =>
  {
    const values = retrieveSelectedOptions(optionsRef.current);
    addItems(values);
  }, [addItems]);

  const addAllItemsHandler = useCallback(() =>
  {
    const values = options
      .filter(item => !isItemSelected(item))
      .map(item => item.value);
    addItems(values);
  }, [options, addItems, isItemSelected]);

  const removeItemsHandler = useCallback(() =>
  {
    const values = retrieveSelectedOptions(selectionRef.current);
    removeItems(values);
  }, [removeItems]);

  const removeAllItemsHandler = useCallback(() =>
  {
    const values = internalSelection.map(item => item.value);
    removeItems(values);
  }, [internalSelection, removeItems]);

  const moveItemsUp = useCallback(() =>
  {
    const values = retrieveSelectedOptions(selectionRef.current);
    if (values.length < 1) return;

    setInternalSelection(prevSelection =>
    {
      const newSelection = [...prevSelection];

      for (let i = 0; i < newSelection.length; i++)
      {
        const item = newSelection[i];
        if (!values.includes(item.value)) continue; // the item is not selected; don't move it

        if (i === 0) return prevSelection; // first selected item is at top; don't move any item

        newSelection.splice(i, 1); // remove the item from its current position
        newSelection.splice(i - 1, 0, item); // insert item to one position above
      }

      if (onChange) onChange(newSelection);
      return newSelection;
    });
  }, [onChange]);

  const moveItemsDown = useCallback(() =>
  {
    const values = retrieveSelectedOptions(selectionRef.current);
    if (values.length < 1) return;

    setInternalSelection(prevSelection =>
    {
      const newSelection = [...prevSelection];
      const lastIndex = newSelection.length - 1;

      for (let i = lastIndex; i >= 0; i--)
      {
        const item = newSelection[i];
        if (!values.includes(item.value)) continue; // the item is not selected; don't move it

        if (i === lastIndex) return prevSelection; // last selected item is at bottom; don't move any item

        newSelection.splice(i, 1); // remove the item from its current position
        newSelection.splice(i + 1, 0, item); // insert item to one position below
      }

      if (onChange) onChange(newSelection);
      return newSelection;
    });
  }, [onChange]);

  const optionsLabelNode = prepareLabel(optionsLabel);
  const selectionLabelNode = prepareLabel(selectionLabel);

  return (
    <div className={styles.RootContainer} {...props}>
      {optionsLabelNode}<div />{selectionLabelNode}<div />
      <Segment style={{ padding: '0', margin: '0' }}>
        <select multiple className={styles.Select} ref={optionsRef}>
          {options
            .filter(item => !isItemSelected(item))
            .map(item => (
              <option key={item.value} label={item.label} onDoubleClick={() => addItems([item.value])}>
                {item.value}
              </option>
            ))}
        </select>
      </Segment>

      <div className={styles.ButtonsContainer}>
        <Button
          basic
          name='add all'
          icon='angle double right'
          onClick={addAllItemsHandler}
        />
        <Button
          basic
          name='add'
          icon='angle right'
          onClick={addItemsHandler}
        />
        <Button
          basic
          name='remove'
          icon='angle left'
          onClick={removeItemsHandler}
        />
        <Button
          basic
          name='remove all'
          icon='angle double left'
          onClick={removeAllItemsHandler}
        />
      </div>

      <Segment style={{ padding: '0', margin: '0' }}>
        <select multiple className={styles.Select} ref={selectionRef}>
          {internalSelection
            .map(item => (
              <option key={item.value} label={item.label} onDoubleClick={() => removeItems([item.value])}>
                {item.value}
              </option>
            ))}
        </select>
      </Segment>

      <div className={styles.ButtonsContainer}>
        <Button basic name='move up' icon='angle up' onClick={moveItemsUp} />
        <Button basic name='move down' icon='angle down' onClick={moveItemsDown} />
      </div>
    </div>
  );
};

export default DualListBox;
