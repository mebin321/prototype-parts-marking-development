import React from 'react';
import { Segment, SegmentProps } from 'semantic-ui-react';

interface IInputEditorProps extends SegmentProps
{
  show?: boolean;
}

const InputEditor: React.FC<IInputEditorProps> = ({
  show,
  style,
  children,
  ...props
}) =>
{
  return (
    <Segment
      tabIndex={1}
      style={{
        display: show ? 'block' : 'none',
        position: 'absolute',
        width: '100%',
        zIndex: 1000,
        margin: '3px 0',
        outline: 'none',
        ...style,
      }}
      {...props}
    >
      {children}
    </Segment>
  );
};

export default InputEditor;
