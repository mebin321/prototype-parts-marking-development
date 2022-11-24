import React, { useCallback } from 'react';
import { Icon, Table } from 'semantic-ui-react';

interface ITableExpandToggleProps
{
  colSpan: number;
  toggled: boolean;
  onToggle: (isSelected: boolean) => void;
}

const TableExpandToggle: React.FC<ITableExpandToggleProps> = ({
  colSpan,
  toggled,
  onToggle,
}) =>
{
  const toggleSelectionHandler = useCallback(() =>
  {
    onToggle(!toggled);
  }, [onToggle, toggled]);

  return (
    <Table.Row onClick={toggleSelectionHandler}>
      <Table.Cell
        collapsing
        colSpan={colSpan}
        style={{ padding: '0', textAlign: 'center', borderStyle: 'none' }}>
        <Icon
          link
          name={toggled ? 'angle up' : 'angle down'}
          style={{ width: '100%', height: '100%' }}
        />
      </Table.Cell>
    </Table.Row>
  );
};

export default TableExpandToggle;
