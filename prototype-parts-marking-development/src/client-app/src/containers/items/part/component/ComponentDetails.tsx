import React, { SyntheticEvent, useCallback, useEffect, useMemo, useState } from 'react';
import { Field, Form as FinalForm } from 'react-final-form';
import { StaticContext } from 'react-router';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { toast } from 'react-toastify';
import { combineValidators, composeValidators, isRequired } from 'revalidate';
import { Button, Divider, Form, Header, Icon, Message, SearchResultData, Segment } from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { TaggedData, extractErrorDetails, extractErrorDetailsFromPutResponse } from '../../../../api/utilities';
import FormattedTextAreaField from '../../../../components/common/form/FormattedTextAreaField';
import SearchField from '../../../../components/common/form/SearchField';
import TextField from '../../../../components/common/form/TextField';
import RestoreItemPrompt from '../../../../components/items/RestoreItemPrompt';
import ScrapComponentsAndParentPrompt from '../../../../components/items/ScrapComponentsAndParentPrompt';
import ScrapItemsPrompt from '../../../../components/items/ScrapItemsPrompt';
import { useLocalStorage } from '../../../../hooks/useLocalStorage';
import usePermissions from '../../../../hooks/usePermissions';
import { createFakePartType } from '../../../../models/api/enumerations/parts';
import { ItemType } from '../../../../models/api/items/part/itemType';
import { IPrototype } from '../../../../models/api/items/part/prototype';
import { IPrototypeFilter } from '../../../../models/api/items/part/prototypeFilter';
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
import { formatPrototypePartCode, generatePartPrintLabel } from '../partUtilities';
import PrototypeSetForm from '../set/PrototypeSetForm';
import VariantsSection from '../variant/VariantsSection';

type ComponentDetailsParams = {prototypeSetId: string; prototypeId: string; componentId: string};
type ComponentDetailsState = TaggedData<IPrototypeSet>;

interface IComponentDetailsProps extends
  RouteComponentProps<ComponentDetailsParams, StaticContext, ComponentDetailsState>
{
}

const validate = combineValidators({
  owner: isRequired('Owner'),
  materialNumber: composeValidators(isRequired, isMaterialNumber)('Material Number'),
  revisionCode: composeValidators(isRequired, isRevisionCode)('Revision Code'),
});

const ComponentDetails: React.FC<IComponentDetailsProps> = ({
  history,
  location,
  match,
}) =>
{
  const {
    canModifyPrototypes,
    canModifyPrototypeVariants,
    canReactivatePrototypes,
    canScrapPrototypes,
    canModifyPrintingLabels,
  } = usePermissions();
  const mode = canModifyPrototypes ? ItemFormMode.Edit : ItemFormMode.View;

  const [prototypeSetData, setPrototypeSetData] = useState<ComponentDetailsState | undefined>(location.state);
  const [componentData, setComponentData] = useState<TaggedData<IPrototype>>();
  const [loading, setLoading] = useState(false);
  const [parent, setParent] = useState<IPrototype>();

  const [, setStoredOwner] = useLocalStorage<IViewableUser>('owner');
  const [ownersFiltered, setOwnersFiltered] = useState<IViewableUser[]>([]);
  const ownersSearchHandler = useMemo(() => generateUserSearchHandler(setOwnersFiltered), []);
  const ownerResultSelectHandler = useCallback((_event: SyntheticEvent, data: SearchResultData) =>
  {
    setStoredOwner(data.result);
  }, [setStoredOwner]);

  const initialPartType = createFakePartType(componentData?.partTypeTitle, componentData?.partTypeCode);
  const initialOwner = convertUserForSearchInput(componentData?.owner);

  const [showScrapRestoreConfirm, setShowScrapRestoreConfirm] = useState(false);
  const [isScrapped, setScrapped] = useState<boolean>();
  const [isLastActiveComponent, setLastActiveComponent] = useState<boolean>();
  const [deletingRestoring, setDeletingRestoring] = useState(false);
  const [isDuplicatedComponentCode, setDuplicatedComponentCode] = useState(false);
  const [notice, setNotice] = useState('');

  const loadParent = useCallback(async () =>
  {
    if (componentData?.index === undefined) return;
    try
    {
      const prototypeSetId = parseIdFromParameter('prototype set', match.params.prototypeSetId);
      const componentId = parseIdFromParameter('component', match.params.componentId);

      await agent.PrototypeSets
        .listItems(prototypeSetId, { type: ItemType.Prototype, index: componentData?.index }, undefined, 1, 1)
        .then(response =>
        {
          if (response.pagination.totalCount < 1)
          {
            throw new Error(`Couldn't find parent prototype of component ${componentId}`);
          }

          if (response.pagination.totalCount > 1)
          {
            throw new Error(`Ambiguous parent prototype of component ${componentId}` +
                              `(found ${response.pagination.totalCount} prototypes instead of 1)`);
          }

          setParent(response.items[0]);
        });
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t determine parent prototype:', extractErrorDetails(error));
    }
  }, [match.params.componentId, match.params.prototypeSetId, componentData?.index]);

  const loadComponent = useCallback(async () =>
  {
    try
    {
      const prototypeSetId = parseIdFromParameter('prototype set', match.params.prototypeSetId);
      const componentId = parseIdFromParameter('component', match.params.componentId);

      // read prototype set if parent page visited by entering address
      if (!prototypeSetData)
      {
        const setData = await agent.PrototypeSets.read(prototypeSetId);
        setPrototypeSetData(setData);
      }

      const data = await agent.Prototypes.read(prototypeSetId, componentId);
      setComponentData(data);
      setScrapped(data.deletedAt !== null);
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t load component:', extractErrorDetails(error));
    }
  }, [match.params.componentId, match.params.prototypeSetId, prototypeSetData]);

  const determineIsLastActiveComponent = useCallback(async () =>
  {
    if (componentData?.index === undefined) return;
    try
    {
      const prototypeSetId = parseIdFromParameter('prototype set', match.params.prototypeSetId);

      const filter: IPrototypeSetItemsFilter = { type: ItemType.Component, index: componentData?.index };

      const componentResponse =
        await agent.PrototypeSets.listActiveItems(prototypeSetId, filter, undefined, 1, 1);

      setLastActiveComponent(componentResponse.pagination.totalCount === 1);
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t load components from parent prototype:', extractErrorDetails(error));
    }
  }, [match.params.prototypeSetId, componentData?.index]);

  const checkComponentPartCodeIsUnique = useCallback(async () =>
  {
    try
    {
      if (!prototypeSetData || !componentData || !isScrapped)
      {
        setDuplicatedComponentCode(false);
        return;
      }

      const filter: IPrototypeFilter =
      {
        isActive: true,
        prototypeSets: [prototypeSetData.id],
        type: ItemType.Component,
        outletCodes: [prototypeSetData.outletCode],
        productGroupCodes: [prototypeSetData.productGroupCode],
        gateLevelCodes: [prototypeSetData.gateLevelCode],
        locationCodes: [prototypeSetData.locationCode],
        evidenceYearCodes: [prototypeSetData.evidenceYearCode],
        partTypeCodes: [componentData.partTypeCode],
        indexes: [componentData.index],
      };

      const activeComponentsWithSamePartCode = await agent.Prototypes.list(filter, undefined, 1, 1);

      if (activeComponentsWithSamePartCode.pagination.totalCount > 0)
      {
        setDuplicatedComponentCode(true);
        return;
      }

      setDuplicatedComponentCode(false);
    }
    catch (error)
    {
      toastDistinctError(
        'Failed to read all active components with duplicated part type code:', extractErrorDetails(error));
    }
  }, [prototypeSetData, componentData, isScrapped]);

  useEffect(() =>
  {
    const init = async () =>
    {
      setLoading(true);
      await loadParent();
      await loadComponent();
      await determineIsLastActiveComponent();
      setLoading(false);
    };

    init();
  }, [loadParent, loadComponent, determineIsLastActiveComponent]);

  useEffect(() =>
  {
    // this effect depends on parent which is set by loadParent and therefore
    // if it would be in same effect as loadParent (called in effect above)
    // then it would result in infinite loop
    checkComponentPartCodeIsUnique();
    if (isScrapped && parent && parent.deletedAt !== null)
    {
      setNotice('Component cannot be restored because parent the prototype is scrapped.');
    }

    if (isScrapped && isDuplicatedComponentCode)
    {
      setNotice('Component cannot be restored because active component with same part code already exists.');
    }
  }, [checkComponentPartCodeIsUnique, isScrapped, parent, isDuplicatedComponentCode]);

  const finalFormSubmitHandler = useCallback(async (values: any) =>
  {
    if (!prototypeSetData || !componentData) return;

    try
    {
      const prototypeUpdateData =
      {
        materialNumber: values.materialNumber,
        revisionCode: values.revisionCode,
        comment: values.comment,
        owner: values.owner?.id,
      };

      await agent.Prototypes.update(prototypeSetData.id, componentData.id, componentData.etag, prototypeUpdateData);
      toast.success(`Successfully updated prototype ${componentData.id}`, { autoClose: 5000 });
      loadComponent();
    }
    catch (error)
    {
      toast.error(`Couldn't update component: ${extractErrorDetailsFromPutResponse(error)}`);
    }
  }, [componentData, prototypeSetData, loadComponent]);

  const goToParentHandler = useCallback(() =>
  {
    if (!componentData)
    {
      toast.error('Couldn\'t navigate to parent prototype: component data is not loaded');
      return;
    }

    if (!parent)
    {
      toast.error('Couldn\'t navigate to parent prototype: parent data is not loaded');
      return;
    }

    const prototypeSetId = componentData.prototypeSetId;
    history.push(`/prototype-sets/${prototypeSetId}/prototypes/${parent?.id}`);
  }, [history, componentData, parent]);

  // reload ETag which has been change by creating new variant
  const variantCreatedHandler = useCallback(() =>
  {
    loadComponent();
  }, [loadComponent]);

  const scrapComponentHandler = useCallback(async (scrapParent: boolean) =>
  {
    setDeletingRestoring(true);
    try
    {
      if (!componentData || !parent) return;

      await agent.Prototypes.scrap(componentData?.prototypeSetId, componentData?.id);
      if (scrapParent)
      {
        await agent.Prototypes.scrap(componentData?.prototypeSetId, parent?.id);
      }

      loadComponent();
      toast.success(scrapParent
        ? `Successfully scrapped component ${componentData.id} and parent prototype ${parent.id}`
        : `Successfully scrapped component ${componentData.id}`,
      { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't scrap the component and parent prototype: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapRestoreConfirm(false);
      setDeletingRestoring(false);
    }
  }, [componentData, loadComponent, parent]);

  const restoreComponentHandler = useCallback(async () =>
  {
    setDeletingRestoring(true);
    try
    {
      if (!componentData) return;
      await agent.Prototypes.restore(componentData?.prototypeSetId, componentData?.id);
      loadComponent();
      toast.success(`Successfully restored item ${componentData?.id}`, { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't restore the component: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapRestoreConfirm(false);
      setDeletingRestoring(false);
    }
  }, [componentData, loadComponent]);

  const addComponentToPrintList = useCallback(() =>
  {
    if (!prototypeSetData || !componentData) return;
    const printLabelItem = generatePartPrintLabel(prototypeSetData, componentData);

    addToPrintList([printLabelItem], () => history.push('/print'));
  }, [componentData, prototypeSetData, history]);

  const scrapRestorePrompt = useMemo(() =>
  {
    return isScrapped
      ? <RestoreItemPrompt
          itemType='component'
          visible={showScrapRestoreConfirm}
          onCancel={() => setShowScrapRestoreConfirm(false)}
          loading={deletingRestoring}
          onConfirm={restoreComponentHandler}
        />
      : isLastActiveComponent
        ? <ScrapComponentsAndParentPrompt
            visible={showScrapRestoreConfirm}
            onCancel={() => setShowScrapRestoreConfirm(false)}
            loading={deletingRestoring}
            onConfirm={() => scrapComponentHandler(false)}
            onConfirmBoth={() => scrapComponentHandler(true)}
          />
        : <ScrapItemsPrompt
            itemType='component'
            visible={showScrapRestoreConfirm}
            onCancel={() => setShowScrapRestoreConfirm(false)}
            loading={deletingRestoring}
            onConfirm={() => scrapComponentHandler(false)}
          />;
  }, [deletingRestoring,
    isScrapped,
    showScrapRestoreConfirm,
    isLastActiveComponent,
    scrapComponentHandler,
    restoreComponentHandler,
  ]);

  return (
    <div style={{ maxWidth: '750px', margin: 'auto' }}>
      {notice && <Message warning>{notice}</Message>}
      <Segment clearing>
        <Header as='h2' style={{ display: 'inline' }}>
          {isScrapped && 'Scrapped '} Component {componentData?.id}
        </Header>
        <Segment basic compact floated='right' style={{ margin: '0', padding: '0' }}>
          <Button
            basic
            title='Go to parent'
            icon='level up alternate'
            disabled={!parent}
            onClick={goToParentHandler}
          />
          <Button
            basic
            title='Add this component to print list'
            icon={
              <Icon.Group>
                <Icon name='print' />
                <Icon corner='bottom right' name='plus' />
              </Icon.Group>
            }
            color='blue'
            disabled={isScrapped || !canModifyPrintingLabels}
            onClick={addComponentToPrintList}
          />
          {!loading && (
            isScrapped
              ? (canReactivatePrototypes &&
                  <Button
                    basic
                    color='green'
                    title='Restore this component'
                    icon='redo alternate'
                    disabled={parent?.deletedAt !== null || isDuplicatedComponentCode}
                    onClick={() => setShowScrapRestoreConfirm(true)}
                  />
                )
              : (canScrapPrototypes &&
                  <Button
                    basic
                    color='red'
                    title='Scrap this component'
                    icon='trash'
                    onClick={() => setShowScrapRestoreConfirm(true)}
                  />
                )
          )}
        </Segment>
        <Header>
          {(componentData && prototypeSetData) &&
            formatPrototypePartCode(prototypeSetData, componentData.partTypeCode, componentData.index)}
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
                  initialValue={componentData?.materialNumber}
                  component={TextField}
                />
                <Field
                  label='Revision Code'
                  name='revisionCode'
                  placeholder='Revision code (numeric)'
                  pattern={revisionCodeInputPattern}
                  readOnly={mode === ItemFormMode.View}
                  initialValue={componentData?.revisionCode}
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
                initialValue={componentData?.comment}
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
      {prototypeSetData && componentData &&
        <VariantsSection
          prototypeSetId={prototypeSetData.id}
          prototypeId={componentData.id}
          etag={componentData.etag}
          addVariantVisible={canModifyPrototypeVariants}
          addVariantEnabled={!isScrapped}
          onVariantCreated={variantCreatedHandler}
        />
      }

      {scrapRestorePrompt}
    </div>
  );
};

export default withRouter(ComponentDetails);
