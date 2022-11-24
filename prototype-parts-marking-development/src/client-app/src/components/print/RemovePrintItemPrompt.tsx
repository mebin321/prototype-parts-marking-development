import React from 'react';
import { Button, Modal } from 'semantic-ui-react';

interface IRemovePrintItemPromptProps
{
  all?: boolean;
  count: number;
  visible: boolean;
  loading: boolean;
  onCancel: () => void;
  onConfirm: () => void;
}

const RemovePrintItemPrompt: React.FC<IRemovePrintItemPromptProps> = ({
  all,
  count,
  visible,
  loading,
  onCancel,
  onConfirm,
}) =>
{
  return (
      <div>
      <Modal
        size='tiny'
        closeOnEscape
        open={visible}
        closeOnDimmerClick
        onClose={onCancel}
      >
        <Modal.Header>Delete Labels to Print</Modal.Header>
        <Modal.Content>
          <p>
            Are you sure you want to remove
            {all ? ` all ${count} ` : ` ${count} selected `}
            label{(all || count > 1) && 's'}?
          </p>
        </Modal.Content>
        <Modal.Actions>
          <Button content='No' onClick={onCancel}
          />
          <Button negative content='Yes' loading={loading} onClick={onConfirm}
          />
        </Modal.Actions>
      </Modal>
    </div>
  );
};

export default RemovePrintItemPrompt;
