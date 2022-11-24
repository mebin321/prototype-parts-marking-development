import React from 'react';
import { Button, Modal } from 'semantic-ui-react';

interface IScrapModalProps
{
  count?: number;
  visible: boolean;
  loading: boolean;
  onCancel: () => void;
  onConfirm: () => void;
  onConfirmBoth: () => void;
}

const ScrapComponentsAndParentPrompt: React.FC<IScrapModalProps> = ({
  count,
  visible,
  loading,
  onCancel,
  onConfirm,
  onConfirmBoth,
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
        <Modal.Header>Delete component{count && count > 1 ? 's' : ''}</Modal.Header>
        <Modal.Content>
          <p>
            Are you sure you want to delete {count === undefined ? 'this ' : `${count} selected `}
            component{count && count > 1 ? 's' : ''}?
          </p>
          <p>
            <b>
              Scrap components{count && count > 1 ? 's' : ''}:&nbsp;
            </b>
            Only use when the complete prototype still exists
          </p>
          <p>
            <b>
              Scrap components{count && count > 1 ? 's' : ''} and parent:&nbsp;
            </b>
            Use when all components of the prototype has been scrapped
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
            content={`Scrap component${count && count > 1 ? 's' : ''}`}
            loading={loading}
            onClick={onConfirm}
          />
          <Button
            negative
            content={`Scrap component(s) and parent${count && count > 1 ? 's' : ''}`}
            loading={loading}
            onClick={onConfirmBoth}
          />
        </Modal.Actions>
      </Modal>
    </div>
  );
};

export default ScrapComponentsAndParentPrompt;
