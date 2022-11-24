import React, { useCallback, useEffect, useRef, useState } from 'react';
import { Input, InputProps, Ref } from 'semantic-ui-react';
import useResizeObserver from 'use-resize-observer';

import InputEditor from './InputEditor';

interface IInputWithEditorProps extends InputProps
{
  editorDisabled?: boolean;
  onEditorOpen?: () => void;
  onEditorClose?: () => void;
}

const InputWithEditor: React.FC<IInputWithEditorProps> = ({
  editorDisabled,
  style,
  children,
  onEditorOpen,
  onEditorClose,
  ...props
}) =>
{
  const timeoutIdRef = useRef<number>();
  const containerRef = useRef<HTMLDivElement>(null);
  const editorRef = useRef<HTMLElement>(null);

  const [focused, setFocused] = useState(false);

  const resizeHandler = () =>
  {
    if (!document?.documentElement) return;
    if (!containerRef.current) return;
    if (!editorRef.current) return;

    const containerBounds = containerRef.current.getBoundingClientRect();
    const editorBounds = editorRef.current.getBoundingClientRect();
    const viewportWidth = document.documentElement.clientWidth;

    const widthDifference = editorBounds.width - containerBounds.width;
    const editorWouldOverflowViewportOnLeft = (containerBounds.left - widthDifference) < 0;
    const editorWouldOverflowViewportOnRight = (containerBounds.right + widthDifference) > viewportWidth;

    if (editorWouldOverflowViewportOnRight && !editorWouldOverflowViewportOnLeft)
    {
      editorRef.current.style.left = 'auto';
      editorRef.current.style.right = '0';
      return;
    }

    editorRef.current.style.left = '0';
    editorRef.current.style.right = 'auto';
  };

  useResizeObserver<HTMLDivElement>({ ref: containerRef, onResize: resizeHandler });
  useResizeObserver<HTMLElement>({ ref: editorRef, onResize: resizeHandler });

  useEffect(() =>
  {
    // if editor is disabled then it will not open/close
    if (editorDisabled) return;

    if (focused)
    {
      if (onEditorOpen) onEditorOpen();
    }
    else
    {
      if (onEditorClose) onEditorClose();
    }
  }, [editorDisabled, focused, onEditorOpen, onEditorClose]);

  const focusHandler = useCallback(() =>
  {
    // Any of input or input editor components has gained the focus. Therefore cancel blur event handler invocation.
    clearTimeout(timeoutIdRef.current);
    setFocused(true);
  }, []);

  const blurHandler = useCallback(() =>
  {
    // The blur and focus events will happen in the same tick (under normal circumstances), allowing the component
    // to cancel its reaction to the blur event if a focus event occurs in the next moment and clears the timeout.
    // If no focus event from an element occurs (if the user has clicked out of the input and input editor),
    // then the blur event will be processed in the next tick and the input will toggle focused to false.
    timeoutIdRef.current = setTimeout(() =>
    {
      setFocused(false);
    });
  }, []);

  const keyDownHandler = useCallback((event: React.KeyboardEvent<HTMLDivElement>) =>
  {
    switch (event.key)
    {
      case 'Enter':
      case 'Esc':
      case 'Escape':
        setFocused(false);
        break;
    }
  }, []);

  return (
    <div
      ref={containerRef}
      style={{ position: 'relative' }}
      onFocusCapture={focusHandler}
      onBlurCapture={blurHandler}
      onKeyDownCapture={keyDownHandler}
    >
      <Input
        input={{ style: { ...style } }}
        {...props}
      />
      {!editorDisabled &&
        <Ref innerRef={editorRef}>
          <InputEditor
            show={focused}
            style={{ minWidth: 'fit-content' }}
          >
            {children}
          </InputEditor>
        </Ref>
      }
    </div>
  );
};

export default InputWithEditor;
