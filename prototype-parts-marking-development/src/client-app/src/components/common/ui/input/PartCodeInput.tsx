import React, { useCallback, useEffect, useRef, useState } from 'react';
import AutosizeInput from 'react-input-autosize';
import { Segment, SegmentProps } from 'semantic-ui-react';

import IPartCode from '../../../../models/partCode';
import styles from './PartCodeInput.module.css';

const nonNegativeNumberPattern = /^\d+$/;

const splitValueOnFirstSeparator = (value: string | undefined, separator: string | undefined): [string, string] =>
{
  value = value?.trim();
  if (!value) return ['', ''];
  if (!separator) return [value, ''];

  const indexOfSeparator = value.indexOf(separator);
  if (indexOfSeparator < 0) return [value, ''];

  const firstPart = value.substr(0, indexOfSeparator + 1);
  const secondPart = indexOfSeparator < value.length ? value.substr(indexOfSeparator + 1) : '';
  return [firstPart, secondPart];
};

const splitValueOnAllSeparators = (value: string | undefined, limit: number): string[] =>
{
  value = value?.trim();
  if (!value) return [];

  const chunksByUnderscore = value.split('_');
  if (chunksByUnderscore.length < 2) return chunksByUnderscore;

  const prototypeNumber = chunksByUnderscore.pop()!;
  const partCodeWithoutPrototypeNumber = chunksByUnderscore.join('_');
  const chunksByDot = partCodeWithoutPrototypeNumber.split('.');

  // return back underscore removed when split to chunks by underscore
  const gateLevel = chunksByDot.pop() + '_';
  // return back dots removed when split to chunks by dot
  const chunksByDotWithSeparator = chunksByDot.map(chunk => chunk + '.');

  const result = chunksByDotWithSeparator.concat(gateLevel, prototypeNumber);
  if (limit < 1) return result;

  // handle when there is too much chunks
  while (result.length > limit)
  {
    const firstChunk = result.shift()!;
    result[0] = firstChunk + result[0];
  }

  return result;
};

interface IPartCodeInputProps extends SegmentProps
{
  onChange?: (partCode: IPartCode) => void;
  onSubmit?: () => void;
}

const PartCodeInput: React.FC<IPartCodeInputProps> = ({
  onChange,
  onSubmit,
  ...props
}) =>
{
  const [outlet, setOutlet] = useState('');
  const [productGroup, setProductGroup] = useState('');
  const [partType, setPartType] = useState('');
  const [evidenceYear, setEvidenceYear] = useState('');
  const [location, setLocation] = useState('');
  const [uniqueIdentifier, setUniqueIdentifier] = useState('');
  const [gateLevel, setGateLevel] = useState('');
  const [prototypeNumber, setPrototypeNumber] = useState('');

  const outletInputRef = useRef<HTMLInputElement | null>(null);
  const productGroupInputRef = useRef<HTMLInputElement | null>(null);
  const partTypeInputRef = useRef<HTMLInputElement | null>(null);
  const evidenceYearInputRef = useRef<HTMLInputElement | null>(null);
  const locationInputRef = useRef<HTMLInputElement | null>(null);
  const uniqueIdentifierInputRef = useRef<HTMLInputElement | null>(null);
  const gateLevelInputRef = useRef<HTMLInputElement | null>(null);
  const prototypeNumberInputRef = useRef<HTMLInputElement | null>(null);

  const inputRefs = Object.freeze([
    undefined,
    outletInputRef,
    productGroupInputRef,
    partTypeInputRef,
    evidenceYearInputRef,
    locationInputRef,
    uniqueIdentifierInputRef,
    gateLevelInputRef,
    prototypeNumberInputRef,
    undefined,
  ]);
  const inputsCount = inputRefs.length - 2; // first and last element is undefined

  useEffect(() =>
  {
    if (!onChange) return;

    const partCode =
    {
      outlet: outlet,
      productGroup: productGroup,
      partType: partType,
      evidenceYear: evidenceYear,
      location: location,
      uniqueIdentifier: uniqueIdentifier,
      gateLevel: gateLevel,
      numberOfPrototypes: prototypeNumber.match(nonNegativeNumberPattern) ? +prototypeNumber : -1,
    };

    onChange(partCode);
  }, [evidenceYear, gateLevel, location, outlet, partType, productGroup, prototypeNumber, uniqueIdentifier, onChange]);

  const setInputValue = useCallback((input: HTMLInputElement, value: string) =>
  {
    switch (input)
    {
      case outletInputRef.current:
        setOutlet(value);
        break;
      case productGroupInputRef.current:
        setProductGroup(value);
        break;
      case partTypeInputRef.current:
        setPartType(value);
        break;
      case evidenceYearInputRef.current:
        setEvidenceYear(value);
        break;
      case locationInputRef.current:
        setLocation(value);
        break;
      case uniqueIdentifierInputRef.current:
        setUniqueIdentifier(value);
        break;
      case gateLevelInputRef.current:
        setGateLevel(value);
        break;
      case prototypeNumberInputRef.current:
        setPrototypeNumber(value);
        break;
      default:
        break;
    }
  }, []);

  const getInputMaxLength = useCallback((input: HTMLInputElement) =>
  {
    switch (input)
    {
      case uniqueIdentifierInputRef.current:
        return 4;
      case prototypeNumberInputRef.current:
        return 3;
      default:
        return 2;
    }
  }, []);

  const getInputSeparator = useCallback((input: HTMLInputElement) =>
  {
    switch (input)
    {
      case gateLevelInputRef.current:
        return '_';
      case prototypeNumberInputRef.current:
        return undefined;
      default:
        return '.';
    }
  }, []);

  const inputChangeHandler = useCallback((input: HTMLInputElement) =>
  {
    if (!input) return;

    const currentInputIndex = inputRefs.findIndex(e => e?.current === input);
    if (currentInputIndex < 1) return;

    // if underscore is present then align pasted part code
    if (input.value.includes('_'))
    {
      const chunks = splitValueOnAllSeparators(input.value, inputsCount);
      const lastInputIndex = inputRefs.length - 2;
      const startInputIndex = lastInputIndex - chunks.length + 1;
      const startInput = inputRefs[startInputIndex]?.current;
      if (startInput && startInputIndex !== currentInputIndex)
      {
        startInput.value = input.value;
        inputChangeHandler(startInput);
        return;
      }
    }

    const inputSeparator = getInputSeparator(input);
    let [value, remainder] = splitValueOnFirstSeparator(input.value, inputSeparator);

    if (inputSeparator && value.endsWith(inputSeparator))
    {
      // strip separator and advance to next input when separator is entered
      value = value.substr(0, value.length - 1);
      inputRefs[currentInputIndex + 1]?.current?.focus();
    }
    else
    {
      // split value when current input maximum length is exceeded
      const inputMaxLength = getInputMaxLength(input);
      if (!remainder && value.length > inputMaxLength)
      {
        remainder = value.substr(inputMaxLength);
        value = value.substr(0, inputMaxLength);
      }
    }

    input.value = value; // set stripped value to override pasted value
    setInputValue(input, value); // store stripped value to state

    if (remainder)
    {
      const nextInput = inputRefs[currentInputIndex + 1]?.current;
      if (nextInput)
      {
        nextInput.focus();
        nextInput.value = remainder;
        inputChangeHandler(nextInput);
      }
    }
  }, [inputRefs, inputsCount, getInputMaxLength, getInputSeparator, setInputValue]);

  const determineInput = useCallback((input: object | null): [number, HTMLInputElement | undefined] =>
  {
    const currentInputIndex = inputRefs.findIndex(e => e?.current === input);
    if (currentInputIndex < 1) return [-1, undefined];

    const currentInput = inputRefs[currentInputIndex]?.current || undefined;
    return [currentInputIndex, currentInput];
  }, [inputRefs]);

  const moveToPreviousInput = useCallback((currentInputIndex: number) =>
  {
    const previousInput = inputRefs[currentInputIndex - 1]?.current;
    if (!previousInput)
    {
      return;
    }

    // without this the cursor will not be moved to adjacent input
    previousInput.focus();
    // move cursor behind last character in adjacent input
    previousInput.setSelectionRange(previousInput.value.length, previousInput.value.length, 'forward');
  }, [inputRefs]);

  const moveToNextInput = useCallback((currentInputIndex: number) =>
  {
    const nextInput = inputRefs[currentInputIndex + 1]?.current;
    if (!nextInput)
    {
      return;
    }

    // without this the cursor will not be moved to adjacent input
    nextInput.focus();
    // move cursor ahead first character in adjacent input
    nextInput.setSelectionRange(0, 0, 'forward');
  }, [inputRefs]);

  const moveCaretToBeginning = useCallback(() =>
  {
    const validInputRefs = inputRefs.filter(inputRef => !!inputRef);
    const firstInput = validInputRefs[0]?.current;
    firstInput?.focus();
    firstInput?.setSelectionRange(0, 0, 'forward');
  }, [inputRefs]);

  const moveCaretToEnd = useCallback(() =>
  {
    const validInputRefs = inputRefs.filter(inputRef => !!inputRef);
    const lastInput = validInputRefs[validInputRefs.length - 1]?.current;
    lastInput?.focus();
    lastInput?.setSelectionRange(lastInput.value.length, lastInput.value.length, 'forward');
  }, [inputRefs]);

  const handleCaretSetback = useCallback((event: KeyboardEvent) =>
  {
    const [currentInputIndex, currentInput] = determineInput(event.target);
    if (!currentInput) return;

    const selectionStart = currentInput.selectionStart;
    const selectionEnd = currentInput.selectionEnd;
    if (selectionStart === null || selectionEnd !== selectionStart) return;

    if (selectionStart > 0) return;

    // cancel arrow key press, otherwise the caret position would be changed to 1 character left from end
    if (event.key === 'ArrowLeft')
    {
      event.preventDefault();
    }

    moveToPreviousInput(currentInputIndex);
  }, [determineInput, moveToPreviousInput]);

  const handleCaretAdvance = useCallback((event: KeyboardEvent) =>
  {
    const [currentInputIndex, currentInput] = determineInput(event.target);
    if (!currentInput) return;

    const selectionStart = currentInput.selectionStart;
    const selectionEnd = currentInput.selectionEnd;
    if (selectionEnd === null || selectionEnd !== selectionStart) return;

    if (selectionEnd < currentInput.value.length) return;

    // cancel arrow key press, otherwise the caret position would be changed to 1 character right from start
    if (event.key === 'ArrowRight')
    {
      event.preventDefault();
    }

    moveToNextInput(currentInputIndex);
  }, [determineInput, moveToNextInput]);

  const keyDownHandler = useCallback((event: KeyboardEvent) =>
  {
    switch (event.key)
    {
      case 'Home':
        if (!event.altKey && !event.shiftKey && !event.metaKey) moveCaretToBeginning();
        break;
      case 'End':
        if (!event.altKey && !event.shiftKey && !event.metaKey) moveCaretToEnd();
        break;
      case 'Delete':
        if (!event.altKey && !event.metaKey) handleCaretAdvance(event);
        break;
      case 'Backspace':
        if (!event.altKey && !event.metaKey) handleCaretSetback(event);
        break;
      case 'ArrowLeft':
        if (!event.altKey && !event.shiftKey && !event.metaKey) handleCaretSetback(event);
        break;
      case 'ArrowRight':
        if (!event.altKey && !event.shiftKey && !event.metaKey) handleCaretAdvance(event);
        break;
      case 'Enter':
        if (onSubmit) onSubmit();
        break;
    }
  }, [handleCaretAdvance, handleCaretSetback, moveCaretToBeginning, moveCaretToEnd, onSubmit]);

  useEffect(() =>
  {
    const inputs = inputRefs.map(inputRef => inputRef?.current).filter(inputRef => !!inputRef);
    inputs.forEach(input => input?.addEventListener('keydown', keyDownHandler));

    return () => inputs.forEach(input => input?.removeEventListener('keydown', keyDownHandler));
  }, [inputRefs, keyDownHandler]);

  return (
    <Segment compact {...props}>
      <AutosizeInput
        name='outlet'
        title='Outlet'
        placeholder='00'
        inputClassName={styles.Input}
        value={outlet}
        inputRef={element => { outletInputRef.current = element; }}
        onChange={event => inputChangeHandler(event.target)}
      />
      <span>.</span>
      <AutosizeInput
        name='product-group'
        title='Product group'
        placeholder='00'
        inputClassName={styles.Input}
        value={productGroup}
        inputRef={element => { productGroupInputRef.current = element; }}
        onChange={event => inputChangeHandler(event.target)}
      />
      <span>.</span>
      <AutosizeInput
        name='part-type'
        title='Part type'
        placeholder='00'
        inputClassName={styles.Input}
        value={partType}
        inputRef={element => { partTypeInputRef.current = element; }}
        onChange={event => inputChangeHandler(event.target)}
      />
      <span>.</span>
      <AutosizeInput
        name='evidence-year'
        title='Year of evidence'
        placeholder='00'
        inputClassName={styles.Input}
        value={evidenceYear}
        inputRef={element => { evidenceYearInputRef.current = element; }}
        onChange={event => inputChangeHandler(event.target)}
      />
      <span>.</span>
      <AutosizeInput
        name='location'
        title='Location'
        placeholder='FR'
        inputClassName={styles.Input}
        value={location}
        inputRef={element => { locationInputRef.current = element; }}
        onChange={event => inputChangeHandler(event.target)}
      />
      <span>.</span>
      <AutosizeInput
        name='unique-identifier'
        title='Unique identifier'
        placeholder='0000'
        inputClassName={styles.Input}
        value={uniqueIdentifier}
        inputRef={element => { uniqueIdentifierInputRef.current = element; }}
        onChange={event => inputChangeHandler(event.target)}
      />
      <span>.</span>
      <AutosizeInput
        name='gate-level'
        title='Gate level'
        placeholder='00'
        inputClassName={styles.Input}
        value={gateLevel}
        inputRef={element => { gateLevelInputRef.current = element; }}
        onChange={event => inputChangeHandler(event.target)}
      />
      <span>_</span>
      <AutosizeInput
        name='prototype-number'
        title='Prototype number'
        placeholder='000'
        inputClassName={styles.Input}
        value={prototypeNumber}
        inputRef={element => { prototypeNumberInputRef.current = element; }}
        onChange={event => inputChangeHandler(event.target)}
      />
    </Segment>
  );
};

export default PartCodeInput;
