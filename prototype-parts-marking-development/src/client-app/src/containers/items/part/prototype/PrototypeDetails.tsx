import React, { SyntheticEvent, useCallback, useEffect, useMemo, useState } from 'react';
import { Field, Form as FinalForm } from 'react-final-form';
import { StaticContext } from 'react-router';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { toast } from 'react-toastify';
import { combineValidators, composeValidators, isRequired } from 'revalidate';
import { Button, Divider, Form, Header, Icon, Message, SearchResultData, Segment } from 'semantic-ui-react';

import agent from '../../../../api/agent';
import
{
  TaggedData,
  extractErrorDetails,
  extractErrorDetailsFromPutResponse,
  listAll,
} from '../../../../api/utilities';
import FormattedTextAreaField from '../../../../components/common/form/FormattedTextAreaField';
import SearchField from '../../../../components/common/form/SearchField';
import TextField from '../../../../components/common/form/TextField';
import RestoreItemPrompt from '../../../../components/items/RestoreItemPrompt';
import ScrapComponentsAndParentPrompt from '../../../../components/items/ScrapComponentsAndParentPrompt';
import ScrapItemsPrompt from '../../../../components/items/ScrapItemsPrompt';
import ScrapPrototypeAndComponentsPrompt from '../../../../components/items/ScrapPrototypeAndComponentsPrompt';
import { useLocalStorage } from '../../../../hooks/useLocalStorage';
import usePermissions from '../../../../hooks/usePermissions';
import { createFakePartType } from '../../../../models/api/enumerations/parts';
import { ItemType } from '../../../../models/api/items/part/itemType';
import { IPrototype } from '../../../../models/api/items/part/prototype';
import { IPrototypeSet } from '../../../../models/api/items/part/set/prototypeSet';
import { IPrototypeSetItemsFilter } from '../../../../models/api/items/part/set/prototypeSetItemsFilter';
import { IViewableUser } from '../../../../models/api/users/viewableUser';
import { parseIdFromParameter } from '../../../../utilities/routing';
import { toastDistinctError } from '../../../../utilities/toast';
import { isMaterialNumber, isRevisionCode } from '../../../../utilities/validation/validators';
import {
  ItemFormMode,
  addToPrintList,
  convertUserForSearchInput,
  generateUserSearchHandler,
  materialNumberInputPattern,
  revisionCodeInputPattern,
} from '../../itemsUtilities';
import ComponentsList from '../component/ComponentsList';
import { formatPrototypePartCode, generatePartPrintLabel } from '../partUtilities';
import PrototypeSetForm from '../set/PrototypeSetForm';
import VariantsSection from '../variant/VariantsSection';

type PrototypeDetailsParams = {prototypeSetId: string; prototypeId: string};
type PrototypeDetailsState = TaggedData<IPrototypeSet>;

interface IPrototypeDetailsProps extends
  RouteComponentProps<PrototypeDetailsParams, StaticContext, PrototypeDetailsState>
{
}

const validate = combineValidators({
  owner: isRequired('Owner'),
  materialNumber: composeValidators(isRequired, isMaterialNumber)('Material Number'),
  revisionCode: composeValidators(isRequired, isRevisionCode)('Revision Code'),
});

const PrototypeDetails: React.FC<IPrototypeDetailsProps> = ({
  history,
  location,
  match,
}) =>
{
  const {
    canCreatePrototypes,
    canModifyPrototypes,
    canModifyPrototypeVariants,
    canReactivatePrototypes,
    canScrapPrototypes,
    canModifyPrintingLabels,
  } = usePermissions();
  const mode = canModifyPrototypes ? ItemFormMode.Edit : ItemFormMode.View;

  const [prototypeSetData, setPrototypeSetData] = useState<PrototypeDetailsState | undefined>(location.state);
  const [prototypeData, setPrototypeData] = useState<TaggedData<IPrototype>>();
  const [loading, setLoading] = useState(false);

  const [, setStoredOwner] = useLocalStorage<IViewableUser>('owner');
  const [ownersFiltered, setOwnersFiltered] = useState<IViewableUser[]>([]);
  const ownersSearchHandler = useMemo(() => generateUserSearchHandler(setOwnersFiltered), []);
  const ownerResultSelectHandler = useCallback((_event: SyntheticEvent, data: SearchResultData) =>
  {
    setStoredOwner(data.result);
  }, [setStoredOwner]);

  const initialPartType = createFakePartType(prototypeData?.partTypeTitle, prototypeData?.partTypeCode);
  const initialOwner = convertUserForSearchInput(prototypeData?.owner);

  const [showScrapRestorePrototypeConfirm, setShowScrapRestorePrototypeConfirm] = useState(false);
  const [showScrapRestoreComponentConfirm, setShowScrapRestoreComponentConfirm] = useState(false);

  const [isScrapped, setScrapped] = useState<boolean>();
  const [activeComponentsCount, setActiveComponentsCount] = useState<number>();
  const [selectedComponents, setSelectedComponents] = useState<IPrototype[]>([]);
  const [deletingRestoring, setDeletingRestoring] = useState(false);
  const [notice, setNotice] = useState<string | undefined>(undefined);

  const loadPrototype = useCallback(async () =>
  {
    setLoading(true);
    try
    {
      const prototypeSetId = parseIdFromParameter('prototype set', match.params.prototypeSetId);
      const prototypeId = parseIdFromParameter('prototype', match.params.prototypeId);

      const prototype = await agent.Prototypes.read(prototypeSetId, prototypeId);
      setPrototypeData(prototype);

      const filter: IPrototypeSetItemsFilter = { type: ItemType.Component, index: prototype.index };
      await agent.PrototypeSets.listActiveItems(prototypeSetId, filter, undefined, 1, 1)
        .then(response => setActiveComponentsCount(response.pagination.totalCount))
        .catch(error => toastDistinctError('Couldn\'t list components:', extractErrorDetails(error)));

      // read prototype set if parent page visited by entering address
      if (!prototypeSetData)
      {
        const setData = await agent.PrototypeSets.read(prototypeSetId);
        setPrototypeSetData(setData);
      }
      else
      {
        // This is for refreshing isActive column after multi/single deleting in component list section
        setPrototypeSetData(prevData => { return prevData ? { ...prevData } : prevData; });
      }

      setScrapped(prototype.deletedAt !== null);
    }
    catch (error)
    {
      toastDistinctError(`Couldn't load prototype with ID ${match.params.prototypeId}:`, extractErrorDetails(error));
    }
    finally
    {
      setLoading(false);
    }
  }, [match.params.prototypeSetId,
    match.params.prototypeId,
    prototypeSetData,
  ]);

  useEffect(() =>
  {
    loadPrototype();
    // empty dependencies array is given to execute update from initial values only on component mount
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() =>
  {
    // this effect depends on prototypeSetData which is set by loadPrototype and therefore
    // if it would be in same effect as loadPrototype (called in effect above)
    // then it would result in infinite loop
    if (isScrapped && prototypeSetData && prototypeSetData.deletedAt !== null)
    {
      setNotice('Component cannot be restored because the parent prototype set is scrapped.');
    }
  }, [prototypeSetData, isScrapped]);

  const finalFormSubmitHandler = useCallback(async (values: any) =>
  {
    if (!prototypeSetData || !prototypeData) return;

    try
    {
      const prototypeUpdateData =
      {
        materialNumber: values.materialNumber,
        revisionCode: values.revisionCode,
        comment: values.comment,
        owner: values.owner?.id,
      };

      await agent.Prototypes.update(prototypeSetData.id, prototypeData.id, prototypeData.etag, prototypeUpdateData);
      toast.success(`Successfully updated prototype ${prototypeData.id}`, { autoClose: 5000 });
      loadPrototype();
    }
    catch (error)
    {
      toast.error(`Couldn't update prototype: ${extractErrorDetailsFromPutResponse(error)}`);
    }
  }, [prototypeData, prototypeSetData, loadPrototype]);

  const goToParentHandler = () =>
  {
    history.push(`/prototype-sets/${match.params.prototypeSetId}`);
  };

  // reload ETag which has been change by creating new variant
  const variantCreatedHandler = useCallback(() =>
  {
    loadPrototype();
  }, [loadPrototype]);

  const componentRowClickHandler = useCallback((component: IPrototype) =>
  {
    history.push({
      pathname: `/prototype-sets/${match.params.prototypeSetId}/components/${component.id}`,
      state: prototypeSetData,
    });
  }, [history, match, prototypeSetData]);

  const scrapSelectedComponentsHandler = useCallback(async (scrapParent: boolean) =>
  {
    setDeletingRestoring(true);
    try
    {
      if (!prototypeData) return;

      for (const component of selectedComponents.filter(item => item.deletedAt === null))
      {
        await agent.Prototypes.scrap(prototypeData?.prototypeSetId, component.id);
      }

      if (scrapParent)
      {
        await agent.Prototypes.scrap(prototypeData?.prototypeSetId, prototypeData?.id);
      }

      loadPrototype();

      toast.success(scrapParent
        ? 'Successfully scrapped all selected components and parent prototype'
        : 'Successfully scrapped all selected components',
      { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't scrap the components: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapRestoreComponentConfirm(false);
      setDeletingRestoring(false);
    }
  }, [prototypeData, loadPrototype, selectedComponents]);

  const scrapPrototypeAndComponentsHandler = useCallback(async () =>
  {
    setDeletingRestoring(true);
    try
    {
      const prototypeSetId = parseIdFromParameter('prototype set', match.params.prototypeSetId);
      const filter: IPrototypeSetItemsFilter = { type: ItemType.Component, index: prototypeData?.index };
      const allActiveComponents = await listAll((page: number) =>
        agent.PrototypeSets.listActiveItems(prototypeSetId, filter, undefined, page));

      if (!prototypeData) return;

      for (const component of allActiveComponents)
      {
        await agent.Prototypes.scrap(prototypeSetId, component.id);
      }

      await agent.Prototypes.scrap(prototypeSetId, prototypeData?.id);
      loadPrototype();
      toast.success(`Successfully scrapped components and parent prototype ${prototypeData?.id}`, { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't scrap the components and parent prototype: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapRestorePrototypeConfirm(false);
      setDeletingRestoring(false);
    }
  }, [prototypeData, match.params.prototypeSetId, loadPrototype]);

  const restorePrototypeHandler = useCallback(async () =>
  {
    setDeletingRestoring(true);
    try
    {
      if (!prototypeData) return;
      await agent.Prototypes.restore(prototypeData?.prototypeSetId, prototypeData?.id);
      loadPrototype();
      toast.success(`Successfully restored item ${prototypeData?.id}`, { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't restore the component: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapRestorePrototypeConfirm(false);
      setShowScrapRestoreComponentConfirm(false);
      setDeletingRestoring(false);
    }
  }, [prototypeData, loadPrototype]);

  const addPrototypeOrComponentsToPrintListHandler = useCallback(async () =>
  {
    try
    {
      if (!prototypeSetData || !prototypeData) return;

      const prototypeSetId = parseIdFromParameter('prototype set', match.params.prototypeSetId);
      const filter: IPrototypeSetItemsFilter = { type: ItemType.Component, index: prototypeData?.index };
      const allActiveComponents = await listAll((page: number) =>
        agent.PrototypeSets.listActiveItems(prototypeSetId, filter, undefined, page));

      const printLabels = allActiveComponents.length > 0
        ? allActiveComponents.map(component => generatePartPrintLabel(prototypeSetData, component))
        : [generatePartPrintLabel(prototypeSetData, prototypeData)];

      addToPrintList(printLabels, () => history.push('/print'));
    }
    catch (error)
    {
      toast.error(`Couldn't add items to print list: ${extractErrorDetails(error)}`);
    }
  }, [match.params.prototypeSetId, prototypeSetData, prototypeData, history]);

  const addSelectedComponentsToPrintListHandler = useCallback(() =>
  {
    if (!prototypeSetData) return;
    const printLabels =
      selectedComponents.filter(item => item.deletedAt === null)
        .map(component => generatePartPrintLabel(prototypeSetData, component));

    addToPrintList(printLabels, () => history.push('/print'));
  }, [prototypeSetData, selectedComponents, history]);

  const scrapRestorePrototypePrompt = useMemo(() =>
  {
    return isScrapped
      ? <RestoreItemPrompt
          itemType='prototype'
          visible={showScrapRestorePrototypeConfirm}
          onCancel={() => setShowScrapRestorePrototypeConfirm(false)}
          loading={deletingRestoring}
          onConfirm={restorePrototypeHandler}
        />
      : <ScrapPrototypeAndComponentsPrompt
            visible={showScrapRestorePrototypeConfirm}
            onCancel={() => setShowScrapRestorePrototypeConfirm(false)}
            loading={deletingRestoring}
            onConfirm={scrapPrototypeAndComponentsHandler}
          />;
  }, [deletingRestoring,
    isScrapped,
    showScrapRestorePrototypeConfirm,
    scrapPrototypeAndComponentsHandler,
    restorePrototypeHandler,
  ]);

  const scrapRestoreComponentsPrompt = useMemo(() =>
  {
    const componentsToScrapCount = selectedComponents.filter(component => component.deletedAt === null).length;

    return componentsToScrapCount === activeComponentsCount
      ? <ScrapComponentsAndParentPrompt
          count={componentsToScrapCount}
          visible={showScrapRestoreComponentConfirm}
          onCancel={() => setShowScrapRestoreComponentConfirm(false)}
          loading={deletingRestoring}
          onConfirm={() => scrapSelectedComponentsHandler(false)}
          onConfirmBoth={() => scrapSelectedComponentsHandler(true)}
        />
      : <ScrapItemsPrompt
          itemType='component'
          count={componentsToScrapCount}
          visible={showScrapRestoreComponentConfirm}
          onCancel={() => setShowScrapRestoreComponentConfirm(false)}
          loading={deletingRestoring}
          onConfirm={() => scrapSelectedComponentsHandler(false)}
        />;
  }, [deletingRestoring,
    showScrapRestoreComponentConfirm,
    selectedComponents,
    activeComponentsCount,
    scrapSelectedComponentsHandler,
  ]);

  const allSelectedComponentsAreScrapped = selectedComponents.every(component => component.deletedAt !== null);

  return (
    <div style={{ maxWidth: '750px', margin: 'auto' }}>
       {notice && <Message warning>{notice}</Message>}
      <Segment clearing>
        <Header as='h2' style={{ display: 'inline' }}>
          {isScrapped && 'Scrapped '} Prototype {prototypeData?.id}
        </Header>
        <Segment basic compact floated='right' style={{ margin: '0', padding: '0' }}>
          <Button
            basic
            title='Go to parent'
            icon='level up alternate'
            disabled={!prototypeSetData}
            onClick={goToParentHandler}
          />
          <Button
            basic
            title='Print prototype, if has no components or all components only'
            icon={
              <Icon.Group>
                <Icon name='print' />
                <Icon corner='bottom right' name='plus' />
              </Icon.Group>
            }
            color='blue'
            disabled={isScrapped || !canModifyPrintingLabels}
            onClick={addPrototypeOrComponentsToPrintListHandler}
          />
          {!loading && (
            isScrapped
              ? (canReactivatePrototypes &&
                  <Button
                    basic
                    color='green'
                    title='Restore this prototype'
                    icon='redo alternate'
                    disabled={prototypeSetData?.deletedAt !== null}
                    onClick={() => setShowScrapRestorePrototypeConfirm(true)}
                  />
                )
              : (canScrapPrototypes &&
                  <Button
                    basic
                    color='red'
                    title='Scrap this prototype and all child components'
                    icon='trash'
                    onClick={() => setShowScrapRestorePrototypeConfirm(true)}
                  />
                )
          )}
        </Segment>
        <Header>
          {(prototypeData && prototypeSetData) &&
            formatPrototypePartCode(prototypeSetData, prototypeData.partTypeCode, prototypeData.index)}
        </Header>
        <FinalForm
          onSubmit={finalFormSubmitHandler}
          validate={validate}
          render={({ handleSubmit, invalid, pristine, submitting }) => (
            <PrototypeSetForm
              mode={ItemFormMode.View}
              data={prototypeSetData}
              loading={loading}
              onSubmit={handleSubmit}
            >
              <Divider />
              <Form.Group widths='two'>
                <Field
                  label='Part'
                  name='part'
                  readOnly={true}
                  defaultSelection={initialPartType}
                  component={SearchField}
                />
                <Field
                  label='Owner'
                  name='owner'
                  placeholder='Select owner'
                  readOnly={mode === ItemFormMode.View}
                  options={ownersFiltered}
                  defaultSelection={initialOwner}
                  onSearchChange={ownersSearchHandler}
                  onResultSelect={ownerResultSelectHandler}
                  component={SearchField}
                />
              </Form.Group>

              <Form.Group widths='two'>
                <Field
                  label='Material Number'
                  name='materialNumber'
                  placeholder='Material number (alphanumeric)'
                  pattern={materialNumberInputPattern}
                  uppercase
                  readOnly={mode === ItemFormMode.View}
                  initialValue={prototypeData?.materialNumber}
                  component={TextField}
                />
                <Field
                  label='Revision Code'
                  name='revisionCode'
                  placeholder='Revision code (numeric)'
                  pattern={revisionCodeInputPattern}
                  readOnly={mode === ItemFormMode.View}
                  initialValue={prototypeData?.revisionCode}
                  component={TextField}
                />
              </Form.Group>

              {
                !loading &&
                <Field
                  label='Comment'
                  name='comment'
                  placeholder='Comment is optional'
                  readOnly={mode === ItemFormMode.View}
                  initialValue={prototypeData?.comment}
                  component={FormattedTextAreaField}
                />
              }

              <Button
                onClick={() => history.goBack()}
                floated='left'
                type='button'
                content='Cancel'
                basic
              />
              {mode === ItemFormMode.Edit &&
                <Button
                  floated='right'
                  type='submit'
                  content='Update'
                  primary
                  disabled={pristine || invalid}
                  loading={submitting}
                />
              }
            </PrototypeSetForm>
          )}
        />
      </Segment>
      {prototypeSetData && prototypeData &&
        <VariantsSection
          prototypeSetId={prototypeSetData.id}
          prototypeId={prototypeData.id}
          etag={prototypeData.etag}
          addVariantVisible={canModifyPrototypeVariants}
          addVariantEnabled={!isScrapped}
          onVariantCreated={variantCreatedHandler}
        />
      }
      {prototypeData &&
        <Segment>
          <Header as='h2' style={{ display: 'inline' }}>Components</Header>
          <Segment basic compact floated='right' style={{ margin: '0', padding: '0' }}>
            <Button
              basic
              title='Print selected components'
              icon={
                <Icon.Group>
                  <Icon name='print' />
                  <Icon corner='bottom right' name='plus' />
                </Icon.Group>
              }
              color='blue'
              disabled={allSelectedComponentsAreScrapped || !canModifyPrintingLabels}
              onClick={addSelectedComponentsToPrintListHandler}
            />
            {!loading && canCreatePrototypes &&
              <Button
                basic
                color='green'
                title='Add new components'
                icon='plus'
                disabled={isScrapped}
                onClick={() => history.push(`${match.url}/components/new`)}
              />
            }
            {!loading && (
              (canScrapPrototypes &&
                <Button
                  basic
                  color='red'
                  title='Scrap selected components'
                  icon='trash'
                  disabled={allSelectedComponentsAreScrapped}
                  onClick={() => setShowScrapRestoreComponentConfirm(true)}
                />
              )
            )}
          </Segment>
          {prototypeSetData &&
            <ComponentsList
              prototypeSet={prototypeSetData}
              partIndex={prototypeData.index}
              onComponentRowClick={componentRowClickHandler}
              onComponentsSelectionChange={setSelectedComponents}
            />
          }
        </Segment>
      }

      {scrapRestoreComponentsPrompt}
      {scrapRestorePrototypePrompt}
    </div>
  );
};

export default withRouter(PrototypeDetails);
