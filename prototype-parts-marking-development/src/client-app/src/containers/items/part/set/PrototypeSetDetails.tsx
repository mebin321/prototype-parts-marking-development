import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Form as FinalForm } from 'react-final-form';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Button, Header, Icon, Segment } from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { TaggedData, extractErrorDetails, listAll } from '../../../../api/utilities';
import RestoreItemPrompt from '../../../../components/items/RestoreItemPrompt';
import ScrapItemsPrompt from '../../../../components/items/ScrapItemsPrompt';
import ScrapPrototypeSetAndChildrenPrompt from '../../../../components/items/ScrapPrototypeSetAndChildrenPrompt';
import usePermissions from '../../../../hooks/usePermissions';
import { ItemType } from '../../../../models/api/items/part/itemType';
import { IPrototype } from '../../../../models/api/items/part/prototype';
import { IPrototypeFilter } from '../../../../models/api/items/part/prototypeFilter';
import { IPrototypeSet } from '../../../../models/api/items/part/set/prototypeSet';
import { IPrototypeSetItemsFilter } from '../../../../models/api/items/part/set/prototypeSetItemsFilter';
import { parseIdFromParameter } from '../../../../utilities/routing';
import { toastDistinctError } from '../../../../utilities/toast';
import { ItemFormMode, addToPrintList } from '../../itemsUtilities';
import { formatPrototypePartCode, generatePartPrintLabel } from '../partUtilities';
import PrototypesList from '../prototype/PrototypesList';
import PrototypeSetForm from './PrototypeSetForm';

interface IPrototypeSetDetailsProps extends RouteComponentProps<{prototypeSetId: string}>
{
}

const PrototypeSetDetails: React.FC<IPrototypeSetDetailsProps> = ({
  history,
  match,
}) =>
{
  const {
    canCreatePrototypes,
    canScrapPrototypes,
    canScrapPrototypeSets,
    canReactivatePrototypeSets,
    canModifyPrintingLabels,
  } = usePermissions();

  const [prototypeSetData, setPrototypeSetData] = useState<TaggedData<IPrototypeSet>>();
  const [loading, setLoading] = useState(false);
  const [selectedPrototypes, setSelectedPrototypes] = useState<IPrototype[]>([]);

  const [isScrapped, setScrapped] = useState<boolean>();
  const [deletingRestoring, setDeletingRestoring] = useState(false);
  const [showScrapRestorePrototypeSetConfirm, setShowScrapRestorePrototypeSetConfirm] = useState(false);
  const [showScrapSelectedPrototypesConfirm, setShowScrapSelectedPrototypesConfirm] = useState(false);

  const loadPrototypeSet = useCallback(async () =>
  {
    setLoading(true);
    try
    {
      const prototypeSetId = parseIdFromParameter('prototype set', match.params.prototypeSetId);
      const data = await agent.PrototypeSets.read(prototypeSetId);
      setPrototypeSetData(data);
      setScrapped(data.deletedAt !== null);
    }
    catch (error)
    {
      toastDistinctError(
          `Couldn't load prototype set with ID ${match.params.prototypeSetId}:`, extractErrorDetails(error));
    }
    finally
    {
      setLoading(false);
    }
  }, [match.params.prototypeSetId]);

  useEffect(() =>
  {
    loadPrototypeSet();
  }, [loadPrototypeSet]);

  const finalFormSubmitHandler = useCallback(async (_values: any) =>
  {
  }, []);

  const prototypeRowClickHandler = useCallback((prototype: IPrototype) =>
  {
    history.push({ pathname: `${match.url}/prototypes/${prototype.id}`, state: prototypeSetData });
  }, [history, match, prototypeSetData]);

  const scrapPrototypeSetHandler = useCallback(async () =>
  {
    setDeletingRestoring(true);
    try
    {
      if (!prototypeSetData) return;

      const componentsFilter: IPrototypeSetItemsFilter = { type: ItemType.Component };
      const allActiveComponents = await listAll((page: number) =>
        agent.PrototypeSets.listActiveItems(prototypeSetData.id, componentsFilter, undefined, page));
      allActiveComponents.forEach(async (component) =>
      {
        return await agent.Prototypes.scrap(prototypeSetData.id, component.id);
      });

      const prototypesFilter: IPrototypeSetItemsFilter = { type: ItemType.Prototype };
      const allActivePrototypes = await listAll((page: number) =>
        agent.PrototypeSets.listActiveItems(prototypeSetData.id, prototypesFilter, undefined, page));
      allActivePrototypes.forEach(async (prototype) =>
      {
        await agent.Prototypes.scrap(prototypeSetData.id, prototype.id);
      });

      await agent.PrototypeSets.scrap(prototypeSetData.id);

      loadPrototypeSet();
      toast.success(`Successfully scrapped prototype set ${prototypeSetData?.id} and all related items`,
        { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't scrap the prototype set: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapRestorePrototypeSetConfirm(false);
      setDeletingRestoring(false);
    }
  }, [loadPrototypeSet, prototypeSetData]);

  const scrapSelectedPrototypes = useCallback(async () =>
  {
    setDeletingRestoring(true);
    try
    {
      if (!prototypeSetData) return;
      const prototypesToScrap = selectedPrototypes.filter(prototype => prototype.deletedAt === null);

      const filter: IPrototypeFilter =
      {
        isActive: true,
        prototypeSets: [prototypeSetData.id],
        type: ItemType.Component,
        indexes: prototypesToScrap.map(prototype => prototype.index),
      };

      const allActiveComponents = await listAll((page: number) => agent.Prototypes.list(filter, undefined, page));
      allActiveComponents.forEach(async (component) =>
      {
        return await agent.Prototypes.scrap(prototypeSetData.id, component.id);
      });

      prototypesToScrap.forEach(async (prototype) =>
      {
        await agent.Prototypes.scrap(prototypeSetData.id, prototype.id);
      });

      loadPrototypeSet();
      toast.success(`Successfully scrapped ${prototypesToScrap.length} 
        prototype${prototypesToScrap.length > 1 ? 's' : ''} and all related components`, { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't scrap selected prototypes: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapSelectedPrototypesConfirm(false);
      setDeletingRestoring(false);
    }
  }, [prototypeSetData, selectedPrototypes, loadPrototypeSet]);

  const restorePrototypeSetHandler = useCallback(async () =>
  {
    setDeletingRestoring(true);
    try
    {
      if (!prototypeSetData) return;
      await agent.PrototypeSets.restore(prototypeSetData.id);

      loadPrototypeSet();
      toast.success(`Successfully restored prototype set ${prototypeSetData?.id}`, { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't restore the prototype set ${prototypeSetData?.id}: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapRestorePrototypeSetConfirm(false);
      setDeletingRestoring(false);
    }
  }, [loadPrototypeSet, prototypeSetData]);

  const addPrototypesToPrintList = useCallback(() =>
  {
    if (!prototypeSetData) return;
    const printLabelItem = selectedPrototypes
      .filter(item => item.deletedAt === null)
      .map(prototype => generatePartPrintLabel(prototypeSetData, prototype));

    addToPrintList(printLabelItem, () => history.push('/print'));
  }, [selectedPrototypes, prototypeSetData, history]);

  const scrapRestorePrototypeSetPrompt = useMemo(() =>
  {
    return isScrapped
      ? <RestoreItemPrompt
          itemType='prototype set'
          visible={showScrapRestorePrototypeSetConfirm}
          onCancel={() => setShowScrapRestorePrototypeSetConfirm(false)}
          loading={deletingRestoring}
          onConfirm={restorePrototypeSetHandler}
        />
      : <ScrapPrototypeSetAndChildrenPrompt
            visible={showScrapRestorePrototypeSetConfirm}
            onCancel={() => setShowScrapRestorePrototypeSetConfirm(false)}
            loading={deletingRestoring}
            onConfirm={scrapPrototypeSetHandler}
          />;
  }, [scrapPrototypeSetHandler,
    restorePrototypeSetHandler,
    isScrapped,
    deletingRestoring,
    showScrapRestorePrototypeSetConfirm]);

  const scrapSelectedPrototypePrompt = useMemo(() =>
  {
    return (<ScrapItemsPrompt
              itemType='prototype'
              count={selectedPrototypes.filter(prototype => prototype.deletedAt === null).length}
              visible={showScrapSelectedPrototypesConfirm}
              onCancel={() => setShowScrapSelectedPrototypesConfirm(false)}
              loading={deletingRestoring}
              onConfirm={scrapSelectedPrototypes}
            />
    );
  }, [scrapSelectedPrototypes, deletingRestoring, showScrapSelectedPrototypesConfirm, selectedPrototypes]);

  const allSelectedPrototypesAreScrapped = selectedPrototypes.every(prototype => prototype.deletedAt !== null);

  return (
    <div style={{ maxWidth: '750px', margin: 'auto' }}>
      <Segment.Group>
        <Segment clearing>
          <Header as='h2' style={{ display: 'inline' }}>
          {isScrapped && 'Scrapped '} Prototype Set {prototypeSetData?.id}
          </Header>
          <Segment basic compact floated='right' style={{ margin: '0', padding: '0' }}>
          {!loading && (
            isScrapped
              ? (canReactivatePrototypeSets &&
                  <Button
                    basic
                    color='green'
                    title='Restore this prototype set'
                    icon='redo alternate'
                    onClick={() => setShowScrapRestorePrototypeSetConfirm(true)}
                  />
                )
              : (canScrapPrototypeSets &&
                  <Button
                    basic
                    color='red'
                    title='Scrap this prototype set and all related items'
                    icon='trash'
                    onClick={() => setShowScrapRestorePrototypeSetConfirm(true)}
                  />
                )
          )}
          </Segment>
          <Header>{prototypeSetData && formatPrototypePartCode(prototypeSetData, 'XX', 0)}</Header>
          <FinalForm
            onSubmit={finalFormSubmitHandler}
            render={() => (
              <PrototypeSetForm
                mode={ItemFormMode.Edit}
                data={prototypeSetData}
                loading={loading}
              >
                <Button
                  onClick={() => history.goBack()}
                  floated='left'
                  type='button'
                  content='Cancel'
                  basic
                />
              </PrototypeSetForm>
            )}
          />
        </Segment>
        {prototypeSetData &&
          <Segment>
            <Header as='h2' style={{ display: 'inline' }}>Prototypes</Header>
            <Segment basic compact floated='right' style={{ margin: '0', padding: '0' }}>
              <Button
                basic
                title='Print selected prototypes'
                icon={
                  <Icon.Group>
                    <Icon name='print' />
                    <Icon corner='bottom right' name='plus' />
                  </Icon.Group>
                }
                color='blue'
                disabled={allSelectedPrototypesAreScrapped || !canModifyPrintingLabels}
                onClick={addPrototypesToPrintList}
              />
              {canCreatePrototypes &&
                <Button
                  basic
                  color='green'
                  title='Add new prototypes'
                  icon='plus'
                  disabled={isScrapped}
                  onClick={() => history.push(`${match.url}/prototypes/new`)}
                />
              }
              {!loading && canScrapPrototypes &&
                  <Button
                    basic
                    color='red'
                    title='Scrap selected prototypes and related components'
                    icon='trash'
                    disabled={allSelectedPrototypesAreScrapped}
                    onClick={() => setShowScrapSelectedPrototypesConfirm(true)}
                  />
              }
            </Segment>
            <PrototypesList
              prototypeSet={prototypeSetData}
              onPrototypeRowClick={prototypeRowClickHandler}
              onPrototypesSelectionChange={setSelectedPrototypes}
            />
          </Segment>
        }
      </Segment.Group>

      {scrapRestorePrototypeSetPrompt}
      {scrapSelectedPrototypePrompt}
    </div>
  );
};

export default withRouter(PrototypeSetDetails);
