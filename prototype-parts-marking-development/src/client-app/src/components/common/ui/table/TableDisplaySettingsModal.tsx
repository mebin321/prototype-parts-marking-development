import _ from 'lodash';
import React, { useEffect, useState } from 'react';
import { Button, Modal } from 'semantic-ui-react';

import { ITableDisplaySettings } from '../../../../store/configuration/types';
import DualListBox from '../input/DualListBox';
import { IDataTableColumn } from './dataTableColumn';

interface ITableDisplaySettingsModalProps
{
  visible: boolean;
  settings?: ITableDisplaySettings;
  availableColumns: IDataTableColumn<any>[];
  onCancel: () => void;
  onConfirm: (settings: ITableDisplaySettings) => void;
}

const TableDisplaySettingsModal: React.FC<ITableDisplaySettingsModalProps> = ({
  visible,
  settings,
  availableColumns,
  onCancel,
  onConfirm,
}) =>
{
  const [visibleColumns, setVisibleColumns] = useState(settings?.visibleColumns ?? []);

  // update internal state from settings property
  // dependency on visible property forces to refresh state on modal open
  useEffect(() =>
  {
    const newVisibleColumns = settings?.visibleColumns ?? [];
    setVisibleColumns(prevVisibleColumns =>
      _.isEqual(prevVisibleColumns, newVisibleColumns) ? prevVisibleColumns : newVisibleColumns);
  }, [settings, visible]);

  const availableColumnsSorted = _.sortBy(availableColumns, column => column.name);

  return (
      <div>
      <Modal
        size='small'
        closeOnEscape
        open={visible}
        onClose={onCancel}
      >
        <Modal.Header>Table display settings</Modal.Header>
        <Modal.Content>
          <div>
            <DualListBox
              optionsLabel='Available Columns'
              selectionLabel='Visible Columns'
              options={availableColumnsSorted.map(column => { return { label: column.name, value: column.name }; })}
              selection={visibleColumns.map(columnName => { return { label: columnName, value: columnName }; })}
              onChange={selection => setVisibleColumns(selection.map(item => item.value))}
            />
          </div>
        </Modal.Content>
        <Modal.Actions>
          <Button
            content='Cancel'
            onClick={onCancel}
          />
          <Button
            primary
            content='OK'
            onClick={() => onConfirm({ visibleColumns: visibleColumns })}
          />
        </Modal.Actions>
      </Modal>
    </div>
  );
};

export default TableDisplaySettingsModal;
