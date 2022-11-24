import React from 'react';
import { Button, Icon, Modal } from 'semantic-ui-react';

interface IScrapModalProps
{
  visible: boolean;
  loading: boolean;
  onCancel: () => void;
  onConfirm: () => void;
}

const ScrapPrototypeSetAndChildrenPrompt: React.FC<IScrapModalProps> = ({
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
        <Modal.Header>
          Delete prototype set
        </Modal.Header>
        <Modal.Content>
          <Icon name='warning sign' color='red' size='huge' style={{ float: 'left', marginRight: '0.4em' }} />
          <div>
            <b>
              You are about to delete all prototypes and related components of this prototype set.
            </b>
            <br />
            This may be several hundreds of items.
          </div>
          <br />
          <p>
            Are you really sure you want to delete prototype set and all related items?
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

export default ScrapPrototypeSetAndChildrenPrompt;
