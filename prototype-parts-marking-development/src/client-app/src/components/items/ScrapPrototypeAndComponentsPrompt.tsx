import React from 'react';
import { Button, Modal } from 'semantic-ui-react';

interface IScrapModalProps
{
  count?: number;
  visible: boolean;
  loading: boolean;
  onCancel: () => void;
  onConfirm: () => void;
}

const ScrapPrototypeAndComponentsPrompt: React.FC<IScrapModalProps> = ({
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
        <Modal.Header>Delete prototype{count && count > 1 ? 's' : ''}</Modal.Header>
        <Modal.Content>
          <p>
            Are you sure you want to delete {count === undefined ? 'this ' : `${count} selected `}
            prototype{count && count > 1 ? 's' : ''} and all related components?
          </p>
        </Modal.Content>
        <Modal.Actions>
          <Button
            floated='left'
            content='Cancel'
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

export default ScrapPrototypeAndComponentsPrompt;
