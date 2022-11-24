import React, { SyntheticEvent, useCallback, useEffect, useRef, useState } from 'react';
import { FieldRenderProps } from 'react-final-form';
import { ConfiguredValidator } from 'revalidate';
import { Form, FormFieldProps, Label, Search, SearchProps, SearchResultData } from 'semantic-ui-react';

const SEARCH_TIMEOUT = 300;

interface ISemanticSearchItem
{
  title?: string;
  description?: string;
  price?: string;
}

interface ISearchFieldProps<T extends ISemanticSearchItem>
  extends FieldRenderProps<string>, FormFieldProps
{
  readOnly?: boolean;
  options?: T[];
  defaultSelection?: T;
  selection?: T;
  validator?: ConfiguredValidator;
  placeholder?: string;
  onResultSelect?: (event: SyntheticEvent, data: SearchResultData) => void;
  onSearchChange?: (event: SyntheticEvent, data: SearchProps) => void;
}

const SearchField = <T extends ISemanticSearchItem, >(props: ISearchFieldProps<T>) =>
{
  const {
    width,
    label,
    input,
    readOnly,
    disabled,
    options,
    defaultSelection,
    selection,
    validator,
    placeholder,
    onResultSelect,
    onSearchChange,
    meta: { error, touched },
  } = props;

  const timeoutRef = useRef<number>();
  const [value, setValue] = useState('');
  const [internalSelection, setInternalSelection] = useState<T>();
  const [loading, setLoading] = useState(false);
  const [isEditing, setEditing] = useState(false);
  const [open, setOpen] = useState(false);
  const [queryError, setQueryError] = useState('');

  useEffect(() =>
  {
    // when default selection is provided (e.g. from local storage)
    // then it has to be set explicitly as when selected by user (see handleResultSelect)
    // value of text search box is set to title of provided item
    // underlying input element onChange event handler is called to store the value
    // which will be read by React Final Form library and then verified + provided on form submit
    if (!touched && !isEditing && !value && defaultSelection)
    {
      setValue(defaultSelection.title ?? '');
      setInternalSelection(defaultSelection);
      input.onChange(defaultSelection);
    }
  }, [input, touched, isEditing, value, defaultSelection]);

  useEffect(() =>
  {
    // when selection property is provided (controlled component)
    // then it has to be set explicitly as when selected by user (see handleResultSelect)
    // value of text search box is set to title of provided item
    // underlying input element onChange event handler is called to store the value
    // which will be read by React Final Form library and then verified + provided on form submit
    if (!isEditing && selection)
    {
      setValue(selection?.title ?? '');
      setInternalSelection(selection);
      input.onChange(selection);
    }
  }, [selection, input, isEditing]);

  // reset loading state when component is re-rendered because of change in available options
  useEffect(() =>
  {
    setLoading(false);
  }, [options]);

  const searchChangeHandler = useCallback((e: SyntheticEvent, data: SearchProps) =>
  {
    if (readOnly) return;

    setEditing(true);
    const query = data.value ?? '';

    setValue(query);
    setInternalSelection(undefined);
    const searchTimeout = timeoutRef.current;
    if (searchTimeout)
    {
      clearTimeout(searchTimeout);
    }

    const issue = validator && validator(query);
    if (issue)
    {
      setQueryError(`Invalid search query: ${issue}`);
      return;
    }
    else
    {
      setQueryError('');
    }

    const timeout = setTimeout(() =>
    {
      if (!onSearchChange)
      {
        return;
      }

      setLoading(true);
      onSearchChange(e, data);
    }, SEARCH_TIMEOUT);
    timeoutRef.current = +timeout;
  }, [readOnly, validator, onSearchChange]);

  const focusHandler = useCallback((e: SyntheticEvent, data: SearchProps) =>
  {
    if (!onSearchChange || readOnly) return;

    // no validation error message
    if (!validator || !validator(data.value))
    {
      setLoading(true);
      onSearchChange(e, data);
    }

    setOpen(true);
  }, [readOnly, validator, onSearchChange]);

  const resultSelectHandler = useCallback((event: SyntheticEvent, data: SearchResultData) =>
  {
    if (readOnly) return;

    setValue(data.result.title);
    setInternalSelection(data.result);

    input.onChange(data.result);
    if (onResultSelect) onResultSelect(event, data);
    setOpen(false);
  }, [input, readOnly, onResultSelect]);

  // blur occurs when user selects a result or when moves to other input (nothing selected)
  // underlying input element onChange event handler is called to store the value
  // which will be read by React Final Form library and then verified + provided on form submit
  const formFieldBlurHandler = useCallback((event: React.FocusEvent<HTMLElement>) =>
  {
    input.onBlur(event);
    input.onChange(internalSelection);
  }, [input, internalSelection]);

  const blurHandler = useCallback((event: SyntheticEvent, data: SearchProps) =>
  {
    setOpen(false);
    setEditing(false);

    // if blur event originates from selecting a result then onResultSelect was already called from Search component,
    // but in case nothing was selected and blur occurred (edit of the field value has ended) then parent element shall
    // be notified that nothing was selected - important when this component is used as controlled component
    if (!internalSelection && onResultSelect)
    {
      onResultSelect(event, { ...data, result: undefined });
    }
  }, [internalSelection, onResultSelect]);

  // prevent showing no results when value is entered but still querying results from server
  // otherwise no results message is shown for fraction of a second when value is entered
  // also ensure no results message is not shown when input is read-only
  const showNoResults = !readOnly && touched && !loading && !queryError;

  return (
    <Form.Field
      error={(touched && !!error) || !!queryError}
      width={width}
      style={{ fontWeight: 'bold' }}
      onBlur={formFieldBlurHandler}
      onFocus={input.onFocus}
    >
      <div style={{ marginBottom: '0.3em' }}>{label}</div>
      <Search
        readOnly={readOnly}
        disabled={disabled}
        showNoResults={showNoResults}
        loading={loading}
        value={value}
        results={queryError ? [] : options}
        open={open}
        placeholder={placeholder}
        onBlur={blurHandler}
        onFocus={focusHandler}
        onSearchChange={searchChangeHandler}
        onResultSelect={resultSelectHandler}
      />
      {((touched && error) || queryError) && (
        <Label
          basic color='red'
          style={{ marginTop: '1em' }}
        >
          {queryError || error}
        </Label>
      )}
    </Form.Field>
  );
};

export default SearchField;
