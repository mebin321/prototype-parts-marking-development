import {
  CompositeDecorator,
  ContentState,
  DraftHandleValue,
  Editor,
  EditorState,
  Modifier,
} from 'draft-js';
import React, { useRef, useState } from 'react';
import { FieldInputProps } from 'react-final-form';

import { multipleNewLineRegex } from './formatStrategy/formatStrategy';
import strategy from './formatStrategy/formatStrategyCaller';
import FormattedText from './formattedText';

interface TextAreaProps
{
  placeholder?: string;
  readOnly: boolean | undefined;
  input: FieldInputProps<string, HTMLElement>;
}

const FormattedTextArea: React.FC<TextAreaProps> = (props) =>
{
  const editorRef = useRef<Editor>(null);
  const compositeDecorator = new CompositeDecorator([
    {
      strategy: strategy,
      component: FormattedText,
    },
  ]);
  const [editor, setEditorState] = useState(() =>
  {
    if (props.input.value)
    {
      const text = props.input.value.replaceAll(multipleNewLineRegex, ' ');
      const content = ContentState.createFromText(text);
      return EditorState.createWithContent(content, compositeDecorator);
    }

    return EditorState.createEmpty(compositeDecorator);
  });

  const editorChangeHandler = (state: EditorState) =>
  {
    const text = state.getCurrentContent().getPlainText(' ');
    props.input.onChange(text);
    setEditorState(state);
  };

  const handleReturn = (e: any, editorState: EditorState) =>
  {
    if (e.key !== 'Enter')
    {
      return 'not-handled';
    }

    const contentState = Modifier.insertText(editorState.getCurrentContent(), editorState.getSelection(), ' ');
    const newState = EditorState.push(editorState, contentState, 'insert-characters');
    setEditorState(newState);
    return 'handled';
  };

  const focus = () =>
  {
    editorRef.current?.focus();
  };

  const handlePastedText = (text: string, html: string | undefined, editorState: EditorState) =>
  {
    const newText = text.replaceAll(multipleNewLineRegex, ' ');
    const pastedBlocks = ContentState.createFromText(newText).getBlockMap();
    const newState = Modifier.replaceWithFragment(
      editorState.getCurrentContent(),
      editorState.getSelection(),
      pastedBlocks
    );
    const newEditorState = EditorState.push(editorState, newState, 'insert-fragment');
    editorChangeHandler(newEditorState);
    return 'handled' as DraftHandleValue;
  };

  return (
    <div onClick={focus}>
      <Editor
        editorState={editor}
        onChange={editorChangeHandler}
        placeholder={props?.placeholder ?? ''}
        handleReturn={handleReturn}
        readOnly={props.readOnly === undefined ? false : props.readOnly}
        handlePastedText={handlePastedText}
        ref={editorRef}
      />
    </div>
  );
};

export default FormattedTextArea;
