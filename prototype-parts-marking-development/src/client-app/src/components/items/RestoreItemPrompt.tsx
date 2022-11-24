import React from 'react';
import { Button, Modal } from 'semantic-ui-react';

interface IRestoreModalProps
{
  itemType: string;
  visible: boolean;
  loading: boolean;
  onCancel: () => void;
  onConfirm: () => void;
}

const RestoreItemPrompt: React.FC<IRestoreModalProps> = ({
  itemType,
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
        <Modal.Header>Restore {itemType}</Modal.Header>
        <Modal.Content>
          <p>Are you sure you want to restore this {itemType}?</p>
        </Modal.Content>
        <Modal.Actions>
          <Button
            floated='left'
            content='No'
            onClick={onCancel}
          />
          <Button
            negative
            content='Yes'
            loading={loading}
            onClick={onConfirm}
          />
        </Modal.Actions>
      </Modal>
    </div>
  );
};

export default RestoreItemPrompt;
