import _ from 'lodash';
import React, { Fragment, useCallback, useEffect, useState } from 'react';
import { Form as FinalForm } from 'react-final-form';
import { Link, RouteComponentProps } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Button, Form, Header, Icon, List, Segment } from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { extractErrorDetails, listAll } from '../../../../api/utilities';
import usePermissions from '../../../../hooks/usePermissions';
import { IPartType } from '../../../../models/api/enumerations/parts';
import { ItemType } from '../../../../models/api/items/part/itemType';
import { IPrototype } from '../../../../models/api/items/part/prototype';
import { IPrototypeSet } from '../../../../models/api/items/part/set/prototypeSet';
import { IPrototypeSetItemsFilter } from '../../../../models/api/items/part/set/prototypeSetItemsFilter';
import { generateMoniker } from '../../../../utilities/objects';
import { toastDistinctError } from '../../../../utilities/toast';
import { materialNumberPatter, revisionCodePattern } from '../../../../utilities/validation/validators';
import { addToPrintList } from '../../itemsUtilities';
import { formatPrototypePartCode, generatePartPrintLabel } from '../partUtilities';
import ComponentDefinition, { IComponentData } from './ComponentDefinition';

const isComponentDataValid = (components: IComponentData[]) =>
{
  return components.every(component =>
    component.materialNumber && materialNumberPatter.test(component.materialNumber) &&
    component.revisionCode && revisionCodePattern.test(component.revisionCode) &&
    component.partType && component.partType.moniker
  );
};

interface IAddComponentsFormProps extends RouteComponentProps<{prototypeId: string; prototypeSetId: string}>
{
}

const AddComponentsForm: React.FC<IAddComponentsFormProps> = ({
  match,
  history,
}) =>
{
  const { canModifyPrintingLabels } = usePermissions();

  const [prototypeSetData, setPrototypeSetData] = useState<IPrototypeSet>();
  const [prototypeData, setPrototypeData] = useState<IPrototype>();
  const [etag, setEtag] = useState('');
  const [allPartTypes, setAllPartTypes] = useState<IPartType[]>([]);
  const [componentsData, setComponentsData] = useState<IComponentData[]>([]);
  const [createdComponents, setCreatedComponents] = useState<IPrototype[]>([]);
  const [activeComponents, setActiveComponents] = useState<IPrototype[]>([]);

  const refreshPrototypeSetEtag = async (prototypeSetId: number) =>
  {
    await agent.PrototypeSets.read(prototypeSetId)
      .then(response => setEtag(response.etag))
      .catch(error => toastDistinctError('Failed to read prototype set:', extractErrorDetails(error)));
  };

  const fetchData = async (prototypeSetId: number, prototypeId: number) =>
  {
    try
    {
      const prototypeSet = await agent.PrototypeSets.read(prototypeSetId);
      const prototype = await agent.Prototypes.read(prototypeSetId, prototypeId);
      const partTypes =
        await agent.Enumerations.Parts.listPermittedComponentParts(generateMoniker(prototype.partTypeTitle));
      const filter: IPrototypeSetItemsFilter = { type: ItemType.Component, index: prototype.index };
      const activeComponents = await listAll((page: number) =>
        agent.PrototypeSets.listActiveItems(prototypeSet.id, filter, undefined, page));

      setPrototypeSetData(prototypeSet);
      setEtag(prototypeSet.etag);
      setPrototypeData(prototype);
      setAllPartTypes(partTypes?.filter(item => item.code !== '00').sort((a, b) => a.title.localeCompare(b.title)));
      setActiveComponents(activeComponents);
    }
    catch (error)
    {
      toastDistinctError('Failed to read data from server:', extractErrorDetails(error));
    }
  };

  const componentToCreateChangeHandler = useCallback((isSelected: boolean, data: IComponentData) =>
  {
    setComponentsData(prevPartTypes =>
    {
      const newPartTypes = prevPartTypes.filter(component => component.partType.moniker !== data.partType.moniker);
      if (isSelected)
      {
        newPartTypes.push(data);
      }

      return newPartTypes;
    });
  }, []);

  const isDuplicatePartType = useCallback((partType: IPartType) =>
  {
    if (componentsData.some(component =>
      component.partType.code === partType.code && component.partType.moniker !== partType.moniker)
    )
    {
      return true;
    }

    if (activeComponents.some(component => component.partTypeCode === partType.code))
    {
      return true;
    }

    if (createdComponents.some(component => component.partTypeCode === partType.code))
    {
      return true;
    }

    return false;
  }, [componentsData, activeComponents, createdComponents]);

  const finalFormSubmitHandler = async () =>
  {
    try
    {
      if (!prototypeSetData)
      {
        throw new Error('Don\'t have prototype set in which to create components');
      }

      if (!prototypeData)
      {
        throw new Error('Don\'t have prototype in which to create components');
      }

      const componentsToCreate = componentsData.map(component =>
      {
        return {
          index: prototypeData.index,
          partMoniker: component.partType.moniker,
          materialNumber: component.materialNumber,
          revisionCode: component.revisionCode,
          ownerId: prototypeData.owner.id,
          comment: prototypeData.comment,
        };
      });
      console.log(componentsToCreate);
      const response = await agent.Prototypes.createComponent(prototypeSetData.id, etag, componentsToCreate);
      toast.success('Successfully added components', { autoClose: 5000 });
      setCreatedComponents(prevPrototypes => prevPrototypes.concat(response));
      await refreshPrototypeSetEtag(prototypeSetData.id);

      setComponentsData([]);
    }
    catch (error)
    {
      toast.error(`Failed to split prototype to components: ${extractErrorDetails(error)}`);
    }
  };

  const printCreatedComponents = useCallback(() =>
  {
    if (!prototypeSetData) return;
    const labelsToPrint = createdComponents.map(component => generatePartPrintLabel(prototypeSetData, component));

    addToPrintList(labelsToPrint, () => history.push('/print'));
  }, [prototypeSetData, createdComponents, history]);

  useEffect(() =>
  {
    const prototypeSetId = +match.params.prototypeSetId;
    const prototypeId = +match.params.prototypeId;

    fetchData(prototypeSetId, prototypeId);
  }, [match.params]);

  return (
    <div style={{ width: 'fit-content', maxWidth: '750px', margin: 'auto' }}>
      <FinalForm
        onSubmit={finalFormSubmitHandler}
        render={({ handleSubmit, submitting }) => (
          <Fragment>
            <Segment clearing style={{ minWidth: 'fit-content' }}>
              <Form onSubmit={handleSubmit}>
                <Header as='h2'>Split prototype {`(${prototypeData?.partTypeTitle})`} to components</Header>
                <div
                  style={{
                    display: 'grid',
                    gridTemplateColumns: 'auto auto min-content auto min-content',
                    gap: '1em',
                    alignItems: 'center',
                    justifyItems: 'left',
                    justifyContent: 'start',
                    marginBottom: '1em',
                  }}
                >
                  {allPartTypes?.map(partType => (
                    <ComponentDefinition
                      key={partType.moniker}
                      partType={partType}
                      selected={componentsData.some(component => _.isEqual(component.partType, partType))}
                      disabled={isDuplicatePartType(partType)}
                      onChange={componentToCreateChangeHandler}
                    />
                  ))}
                </div>
                <Button
                  onClick={() => history.goBack()}
                  type='button'
                  content='Close'
                  basic
                  floated='right'
                />
             </Form>
            </Segment>
            <Segment.Group style={{ width: '100%' }}>
              <Segment>
                {prototypeSetData && allPartTypes && (
                  <Fragment>
                    <Header as='h3'>
                      <span><Icon name='asterisk' /></span>
                      Components to be created
                      <Button
                        floated='right'
                        type='button'
                        content='Create'
                        primary
                        disabled={componentsData.length < 1 || !isComponentDataValid(componentsData)}
                        loading={submitting}
                        onClick={handleSubmit}
                      />
                    </Header>
                    <List>
                      {prototypeSetData && prototypeData &&
                        componentsData.map(component => (
                          <List.Item style={{ fontSize: '1.2em' }} key={component.partType.moniker}>
                            <b>
                              {formatPrototypePartCode(prototypeSetData, component.partType.code, prototypeData.index)}
                            </b>
                          </List.Item>
                        ))}
                    </List>
                  </Fragment>
                )}
            </Segment>
            <Segment>
              {prototypeSetData && allPartTypes && (
                <Fragment>
                  <Header as='h3'>
                    <span><Icon name='checkmark' /></span>
                    Recently created components
                    <Button
                      floated='right'
                      type='button'
                      content='Print'
                      primary
                      disabled={createdComponents.length < 1 || !canModifyPrintingLabels}
                      loading={submitting}
                      onClick={printCreatedComponents}
                    />
                  </Header>
                  <List>
                    {createdComponents.map(component => (
                      <List.Item style={{ fontSize: '1.2em' }} key={component.id}>
                        <Link
                          to={`/prototype-sets/${match.params.prototypeSetId}/components/${component.id}`}
                          style={{ color: 'black' }}
                        >
                          <b>{formatPrototypePartCode(prototypeSetData, component.partTypeCode, component.index)}</b>
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

export default AddComponentsForm;
