import { FormApi } from 'final-form';
import React, { Fragment, SyntheticEvent, useCallback, useEffect, useMemo, useState } from 'react';
import { Field, Form as FinalForm } from 'react-final-form';
import { Link, RouteComponentProps } from 'react-router-dom';
import { toast } from 'react-toastify';
import { combineValidators, composeValidators, isRequired } from 'revalidate';
import {
  Button,
  DropdownProps,
  Form,
  Header,
  Icon,
  InputOnChangeData,
  List,
  SearchResultData,
  Segment,
} from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { PartsEnum } from '../../../../api/enumerations';
import { extractErrorDetails } from '../../../../api/utilities';
import FormattedTextAreaField from '../../../../components/common/form/FormattedTextAreaField';
import SearchField from '../../../../components/common/form/SearchField';
import SelectField from '../../../../components/common/form/SelectField';
import SpinnerField from '../../../../components/common/form/SpinnerField';
import TextField from '../../../../components/common/form/TextField';
import { useLocalStorage } from '../../../../hooks/useLocalStorage';
import usePermissions from '../../../../hooks/usePermissions';
import { EmptyTextualEnumItem } from '../../../../models/api/enumerations';
import { IPartType } from '../../../../models/api/enumerations/parts';
import { IPrototype } from '../../../../models/api/items/part/prototype';
import { IPrototypeFilter } from '../../../../models/api/items/part/prototypeFilter';
import { IPrototypeSet } from '../../../../models/api/items/part/set/prototypeSet';
import { SortDirection } from '../../../../models/api/sort/sortDirection';
import { ISortParameters } from '../../../../models/api/sort/sortParameters';
import { IViewableUser } from '../../../../models/api/users/viewableUser';
import { generateMoniker } from '../../../../utilities/objects';
import { toastDistinctError } from '../../../../utilities/toast';
import { isMaterialNumber, isNotEmpty, isRevisionCode } from '../../../../utilities/validation/validators';
import {
  addToPrintList,
  generatePartsSearchHandler,
  generateUserSearchHandler,
  materialNumberInputPattern,
  revisionCodeInputPattern,
} from '../../itemsUtilities';
import {
  NumberingStyle,
  NumberingStyleOptions,
  formatPrototypePartCode,
  generatePartIndices,
  generatePartPrintLabel,
  generatePrototypesCreateData,
} from '../partUtilities';

interface IAddPrototypesFormProps extends RouteComponentProps<{prototypeSetId: string}>
{
}

const validate = combineValidators({
  numberingStyle: isRequired('Numbering style'),
  numberOfPrototypes: isRequired('Number of Prototypes'),
  owner: isRequired('Owner'),
  part: isNotEmpty({ message: 'Part is required' }),
  materialNumber: composeValidators(isRequired, isMaterialNumber)('Material Number'),
  revisionCode: composeValidators(isRequired, isRevisionCode)('Revision Code'),
});

const AddPrototypesForm: React.FC<IAddPrototypesFormProps> = ({
  history,
  match,
}) =>
{
  const { canModifyPrintingLabels } = usePermissions();

  const defaultNumberingStyle = NumberingStyle.Consecutive;

  const [prototypeSetData, setPrototypeSetData] = useState<IPrototypeSet>();
  const [lastPrototypeIndex, setLastPrototypeIndex] = useState(0);
  const [etag, setEtag] = useState('');
  const [numberingStyle, setNumberingStyle] = useState(defaultNumberingStyle);
  const [numberOfPrototypes, setNumberOfPrototypes] = useState(0);
  const [createdPrototypes, setCreatedPrototypes] = useState<IPrototype[]>([]);

  const [partCodeIndices, setPartCodeIndices] = useState<number[]>([]);

  const numberingStyleChangeHandler = useCallback((_e: SyntheticEvent, d: DropdownProps) =>
  {
    setNumberingStyle(d.value?.toString() ?? '');
  }, []);

  const [storedOwner, setStoredOwner] = useLocalStorage<IViewableUser>('owner');
  const [ownersFiltered, setOwnersFiltered] = useState<IViewableUser[]>([]);
  const ownersSearchHandler = generateUserSearchHandler(setOwnersFiltered);
  const ownerResultSelectHandler = useCallback((_event: SyntheticEvent, data: SearchResultData) =>
  {
    setStoredOwner(data.result);
  }, [setStoredOwner]);

  const numberOfPrototypesChangeHandler = useCallback((_e: SyntheticEvent, d: InputOnChangeData) =>
  {
    setNumberOfPrototypes(d.value ? +d.value : 0);
  }, []);

  const [storedPart, setStoredPart] = useLocalStorage<IPartType>(PartsEnum);
  const [partsFiltered, setPartsFiltered] = useState<IPartType[]>([]);

  const partsSearchHandler = useMemo(
    () => generatePartsSearchHandler(generateMoniker(prototypeSetData?.productGroupTitle ?? ''), setPartsFiltered),
    [prototypeSetData]);
  const partResultSelectHandler = useCallback((_event: SyntheticEvent, data: SearchResultData) =>
  {
    setStoredPart(data.result);
  }, [setStoredPart]);

  const refreshPrototypeSetData = async (prototypeSetId: number) =>
  {
    await agent.PrototypeSets.read(prototypeSetId)
      .then(response =>
      {
        setPrototypeSetData(response);
        setEtag(response.etag);
      })
      .catch(error => toastDistinctError('Failed to read prototype set:', extractErrorDetails(error)));
  };

  const retrieveLastPrototypeIndex = async (prototypeSetId: number) =>
  {
    try
    {
      const filter: IPrototypeFilter = { prototypeSets: [prototypeSetId] };
      const sort: ISortParameters = { sortBy: 'Index', sortDirection: SortDirection.Descending };
      const response = await agent.Prototypes.list(filter, sort, 1, 1);
      const maxIndex = response.items.length < 1 ? 0 : response.items[0].index;

      setLastPrototypeIndex(maxIndex);
    }
    catch (error)
    {
      toastDistinctError('Failed to read prototype set:', extractErrorDetails(error));
    }
  };

  const fetchPermittedPartTypes = useCallback(async () =>
  {
    if (!prototypeSetData || !storedPart) return;
    await agent.Enumerations.ProductGroups.listPermittedParts(
      generateMoniker(prototypeSetData.productGroupTitle ?? ''))
      .then(permittedParts =>
      {
        if (permittedParts.every(part => part.moniker !== storedPart.moniker))
        {
          setStoredPart(EmptyTextualEnumItem);
        }
      }).catch(error => toastDistinctError('Failed to fetch relation dependency:', extractErrorDetails(error)));
    // StoredPart is omitted from dependencies array to not run this effect on that change
    // to prevent infinite loop because storedProductGroup setter is in executing area
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [prototypeSetData]);

  const finalFormSubmitHandler = async (values: any) =>
  {
    try
    {
      const prototypes = generatePrototypesCreateData(
        partCodeIndices,
        values.part.moniker,
        values.materialNumber,
        values.revisionCode,
        values.owner.id,
        values.comment
      );

      if (prototypes.length === 0)
      {
        throw new Error('No prototypes to create are given');
      }

      if (!prototypeSetData)
      {
        throw new Error('Don\'t have prototype set in which to create prototypes');
      }

      const response = await agent.Prototypes.createPrototype(prototypeSetData.id, etag, prototypes);
      toast.success('Successfully added prototypes', { autoClose: 5000 });
      setCreatedPrototypes(prevPrototypes => prevPrototypes.concat(response));
      await refreshPrototypeSetData(prototypeSetData.id);
    }
    catch (error)
    {
      toast.error(`Failed to add prototypes to set: ${extractErrorDetails(error)}`);
    }
  };

  // initialize PrototypeSet from backEnd
  useEffect(() =>
  {
    const prototypeSetId = +match.params.prototypeSetId;
    refreshPrototypeSetData(prototypeSetId);
    retrieveLastPrototypeIndex(prototypeSetId);
  }, [match.params]);

  useEffect(() =>
  {
    fetchPermittedPartTypes();
  }, [fetchPermittedPartTypes]);

  // update part code indices that will be created
  useEffect(() =>
  {
    if (!numberingStyle || !numberOfPrototypes)
    {
      return;
    }

    let lastUsedIndex = lastPrototypeIndex;
    let partCodeIndices: number[];
    do
    {
      partCodeIndices = generatePartIndices(lastUsedIndex, numberOfPrototypes, numberingStyle);
      lastUsedIndex += numberingStyle === NumberingStyle.Consecutive ? 1 : 2;
    } while (partCodeIndices.some(index => createdPrototypes.map(item => item.index).includes(index)));

    setPartCodeIndices(partCodeIndices);
  }, [createdPrototypes, lastPrototypeIndex, numberOfPrototypes, numberingStyle]);

  const resetForm = (form: FormApi) =>
  {
    form.reset({ part: storedPart, owner: storedOwner });
    for (const fieldName of form.getRegisteredFields())
    {
      form.resetFieldState(fieldName);
    }

    setNumberingStyle('');
    setNumberOfPrototypes(0);
    setPartCodeIndices([]);
  };

  const printCreatedPrototypes = useCallback(() =>
  {
    if (!prototypeSetData) return;
    const printLabels = createdPrototypes.map(prototype => generatePartPrintLabel(prototypeSetData, prototype));

    addToPrintList(printLabels, () => history.push('/print'));
  }, [prototypeSetData, createdPrototypes, history]);

  return (
    <div style={{ maxWidth: '750px', margin: 'auto' }}>
      <FinalForm
        onSubmit={finalFormSubmitHandler}
        validate={validate}
        render={({ handleSubmit, invalid, pristine, submitting, form }) => (
          <Fragment>
            <Segment clearing>
              <Header as='h2'>Add new prototypes</Header>
              <Form>
                <Form.Group widths='two'>
                  <Field
                    label='Numbering style'
                    name='numberingStyle'
                    defaultValue={defaultNumberingStyle}
                    onChange={numberingStyleChangeHandler}
                    options={NumberingStyleOptions}
                    component={SelectField}
                  />
                  <Field
                    label='Part'
                    name='part'
                    placeholder={`Product group was set to ${prototypeSetData?.productGroupTitle}`}
                    options={partsFiltered}
                    selection={storedPart}
                    onSearchChange={partsSearchHandler}
                    onResultSelect={partResultSelectHandler}
                    component={SearchField}
                  />
                </Form.Group>

                <Form.Group widths='two'>
                  <Field
                    label='Owner'
                    name='owner'
                    placeholder='Select owner'
                    options={ownersFiltered}
                    defaultSelection={storedOwner}
                    onSearchChange={ownersSearchHandler}
                    onResultSelect={ownerResultSelectHandler}
                    component={SearchField}
                  />
                  <Field
                    label='Number of prototypes'
                    name='numberOfPrototypes'
                    placeholder='Number of prototypes'
                    onChange={numberOfPrototypesChangeHandler}
                    component={SpinnerField}
                  />
                </Form.Group>

                <Form.Group widths='two'>
                  <Field
                    label='Material Number'
                    name='materialNumber'
                    placeholder='Material number (alphanumeric)'
                    pattern={materialNumberInputPattern}
                    uppercase
                    component={TextField}
                  />
                  <Field
                    label='Revision Code'
                    name='revisionCode'
                    placeholder='Revision code (numeric)'
                    pattern={revisionCodeInputPattern}
                    component={TextField}
                  />
                </Form.Group>

                <Field
                  label='Comment'
                  name='comment'
                  placeholder='Comment is optional'
                  component={FormattedTextAreaField}
                />

                <Button
                  basic
                  type='button'
                  content='Close'
                  floated='right'
                  onClick={() => history.goBack()}
                />
              </Form>
            </Segment>
            <Segment.Group>
            <Segment>
              {prototypeSetData && (
                <Fragment>
                  <Header as='h3'>
                    <span><Icon name='asterisk' /></span>
                    Prototypes to be created
                    <Button
                      floated='right'
                      type='button'
                      content='Create'
                      primary
                      disabled={pristine || invalid}
                      loading={submitting}
                      onClick={async event => { await handleSubmit(event); resetForm(form); }}
                    />
                  </Header>
                  <List>
                    {!submitting && partCodeIndices.map(index => (
                      <List.Item style={{ fontSize: '1.2em' }} key={index}>
                        <b>{formatPrototypePartCode(prototypeSetData, storedPart?.code ?? '', index)}</b>
                      </List.Item>
                    ))}
                  </List>
                </Fragment>
              )}
            </Segment>
            <Segment>
              {prototypeSetData && (
                <Fragment>
                  <Header as='h3'>
                    <span><Icon name='checkmark' /></span>
                    Recently created prototypes
                    <Button
                      floated='right'
                      type='button'
                      content='Print'
                      primary
                      disabled={createdPrototypes.length < 1 || !canModifyPrintingLabels}
                      loading={submitting}
                      onClick={printCreatedPrototypes}
                    />
                  </Header>
                  <List>
                    {createdPrototypes.map(prototype => (
                      <List.Item style={{ fontSize: '1.2em' }} key={prototype.index}>
                        <Link
                          to={`/prototype-sets/${match.params.prototypeSetId}/prototypes/${prototype.id}`}
                          style={{ color: 'black' }}
                        >
                          <b>{formatPrototypePartCode(prototypeSetData, prototype.partTypeCode, prototype.index)}</b>
                        </Link>
                      </List.Item>
                    ))}
                  </List>
                </Fragment>
              )}
            </Segment>
          </Segment.Group>
        </Fragment>
        )}
      />
    </div>
  );
};

export default AddPrototypesForm;
