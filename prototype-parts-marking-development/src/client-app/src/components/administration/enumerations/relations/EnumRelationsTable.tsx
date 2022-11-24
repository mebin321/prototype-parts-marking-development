import _ from 'lodash';
import React, { useCallback, useEffect, useRef, useState } from 'react';
import { Checkbox, Header, Ref, Table } from 'semantic-ui-react';

import classes from './EnumRelationsTable.module.css';

interface ITextualEnumItem
{
  moniker: string;
  title: string;
}

interface IEnumRelationsTableProps
{
  inputOptions: ITextualEnumItem[];
  outputOptions: ITextualEnumItem[];
  relations: Map<string, string[]>;
  disabledRelations?: Map<string, string[]>;
  inputOptionsCaption?: string;
  outputOptionsCaption?: string;
  onChange?: (relations: Map<string, string[]>) => void;
}

const EnumRelationsTable: React.FC<IEnumRelationsTableProps> = ({
  inputOptions,
  outputOptions,
  relations,
  disabledRelations,
  inputOptionsCaption,
  outputOptionsCaption,
  onChange,
}) =>
{
  const allowOnRelationsChangeRef = useRef(false);
  const tableBodyRef = useRef<HTMLElement>(null);

  const [internalRelations, setInternalRelations] = useState(new Map<string, string[]>());

  // update internal relations from property
  useEffect(() =>
  {
    setInternalRelations(prevSelection =>
      _.isEqual(prevSelection, relations) ? prevSelection : new Map(relations));
  }, [relations]);

  useEffect(() =>
  {
    // prevent calling onChange when initial relations value is set on mount
    if (!allowOnRelationsChangeRef.current)
    {
      allowOnRelationsChangeRef.current = true;
      return;
    }

    if (onChange) onChange(new Map(internalRelations));
  }, [internalRelations, onChange]);

  const tableCellMouseOverHandler = useCallback((event: React.MouseEvent<HTMLTableCellElement>) =>
  {
    const currentCell = event.currentTarget;
    const currentRow = currentCell.parentElement as HTMLTableRowElement;
    const currentRowIndex = currentRow.rowIndex - 1; // -1 to subtract header row
    const currentColumnIndex = currentCell.cellIndex;

    const rows = tableBodyRef.current?.getElementsByTagName('tr');
    if (!rows) return;

    for (let rowIndex = 0; rowIndex < rows.length; rowIndex++)
    {
      const row = rows[rowIndex];
      const cells = row.getElementsByTagName('td');

      for (let cellIndex = 0; cellIndex < cells.length; cellIndex++)
      {
        const cell = cells[cellIndex];
        const highlighted = currentRowIndex === rowIndex || currentColumnIndex === cellIndex;

        if (highlighted && !cell.classList.contains(classes.HighlightedCell))
        {
          cell.classList.add(classes.HighlightedCell);
        }

        if (!highlighted && cell.classList.contains(classes.HighlightedCell))
        {
          cell.classList.remove(classes.HighlightedCell);
        }
      }
    }
  }, []);

  const relationCheckboxChangeHandler = useCallback(
    (input: ITextualEnumItem, output: ITextualEnumItem, allowed: boolean | undefined) =>
    {
      setInternalRelations(prevRelations =>
      {
        const newRelations = new Map(prevRelations);
        const allowedRelations = newRelations.get(input.moniker) ?? [];

        if (allowed)
        {
          if (!allowedRelations.includes(output.moniker))
          {
            newRelations.set(input.moniker, allowedRelations.concat(output.moniker));
            return newRelations;
          }
        }
        else
        {
          if (allowedRelations.includes(output.moniker))
          {
            newRelations.set(input.moniker, allowedRelations.filter(relation => relation !== output.moniker));
            return newRelations;
          }
        }

        return prevRelations;
      });
    }, []);

  const isRelationDisabled = useCallback((input: ITextualEnumItem, output: ITextualEnumItem) =>
  {
    return disabledRelations?.get(input.moniker)?.includes(output.moniker);
  }, [disabledRelations]);

  const isRelationActive = (input: ITextualEnumItem, output: ITextualEnumItem) =>
  {
    return internalRelations.get(input.moniker)?.includes(output.moniker);
  };

  return (
    <div style={{ display: 'grid', gridTemplateRows: 'max-content auto', gridTemplateColumns: 'max-content auto' }}>
      {outputOptionsCaption &&
        <Header style={{ gridColumn: '2', justifySelf: 'center' }}>
          {outputOptionsCaption}
        </Header>
      }
      {inputOptionsCaption &&
        <Header
          className={classes.VerticalText}
          style={{
            alignSelf: 'center',
            marginRight: '0.8em',
            marginTop: '2em',
            backgroundColor: 'white',
          }}
        >
          {inputOptionsCaption}
        </Header>
      }

      <div className={[classes.RelationsTableContainer, classes.Sticky].join(' ')}>
        <Table celled className={classes.RelationsTable}>
          <Table.Header>
            <Table.Row>
              <Table.Cell
                className={[classes.EmptyCell, classes.FirstRow, classes.FirstColumn, classes.Sticky].join(' ')}
                style={{ zIndex: 2000 }} // top left corner cell has to be above all other cells to hide them on scroll
              />
              {outputOptions.map(outputOption => (
                <Table.Cell
                  key={outputOption.moniker}
                  className={[classes.HeaderCell, classes.FirstRow, classes.Sticky].join(' ')}
                >
                  {outputOption.title}
                </Table.Cell>
              ))}
            </Table.Row>
          </Table.Header>
          <Ref innerRef={tableBodyRef}>
            <Table.Body>
              {inputOptions.map(inputOption => (
                <Table.Row key={inputOption.moniker}>
                  <Table.Cell
                    className={[classes.HeaderCell, classes.FirstColumn, classes.Sticky].join(' ')}
                  >
                    {inputOption.title}
                  </Table.Cell>
                  {outputOptions.map(outputOption => (
                    <Table.Cell
                      key={`${inputOption.moniker} ${outputOption.moniker}`}
                      className={classes.CheckboxCell}
                      onMouseOver={tableCellMouseOverHandler}
                    >
                      <Checkbox
                        checked={isRelationActive(inputOption, outputOption)}
                        disabled={isRelationDisabled(inputOption, outputOption)}
                        onChange={(_e, d) => relationCheckboxChangeHandler(inputOption, outputOption, d.checked)}
                      />
                    </Table.Cell>
                  ))}
                </Table.Row>
              ))}
            </Table.Body>
          </Ref>
        </Table>
      </div>
    </div>
  );
};

export default EnumRelationsTable;
