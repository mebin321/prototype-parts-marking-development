import _ from 'lodash';
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { toast } from 'react-toastify';
import { Button, Dimmer, Loader } from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { extractErrorDetails, listAll } from '../../../../api/utilities';
import EnumRelationsTable from '../../../../components/administration/enumerations/relations/EnumRelationsTable';
import usePermissions from '../../../../hooks/usePermissions';
import { IPartType } from '../../../../models/api/enumerations/parts';
import { toastDistinctError } from '../../../../utilities/toast';

const determineDisabledRelations = (parts: IPartType[]) =>
{
  const hasDuplicate = (parts: IPartType[], possibleDuplicate: IPartType) =>
  {
    return parts.filter(part => part.code === possibleDuplicate.code).map(part => part.moniker);
  };

  // component cannot have part type representing complete prototype
  const completePrototypes = parts.filter(part => part.code === '00').map(part => part.moniker);
  // component cannot have same part type as parent prototype
  return new Map(parts.map(part =>
    [part.moniker, [part.moniker, ...hasDuplicate(parts, part), ...completePrototypes]]));
};

const PartComponents: React.FC = () =>
{
  const { canModifyEntityRelation } = usePermissions();

  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [parts, setParts] = useState<IPartType[]>([]);
  const [initialRelations, setInitialRelations] = useState(new Map<string, string[]>());
  const [relations, setRelations] = useState(new Map<string, string[]>());
  const [disabledRelations, setDisabledRelations] = useState(new Map<string, string[]>());

  const loadRelations = useCallback(async (partMonikers: string[]) =>
  {
    const result = new Map<string, string[]>();
    for (const partMoniker of partMonikers)
    {
      const response = await agent.Enumerations.Parts.listPermittedComponentParts(partMoniker);
      result.set(partMoniker, response.map(item => item.moniker));
    }

    return result;
  }, []);

  const loadData = useCallback(async () =>
  {
    setLoading(true);
    try
    {
      const loadedParts = await listAll(page => agent.Enumerations.Parts.listItems(page));
      const loadedRelations = await loadRelations(loadedParts.map(part => part.moniker));

      setParts(loadedParts);
      setInitialRelations(loadedRelations);
      setDisabledRelations(determineDisabledRelations(loadedParts));
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t load permitted component part types of parts:', extractErrorDetails(error));
    }
    finally
    {
      setLoading(false);
    }
  }, [loadRelations]);

  useEffect(() =>
  {
    loadData();
  }, [loadData]);

  const updateRelationsHandler = useCallback(async () =>
  {
    if (relations.size < 1) return;

    setSubmitting(true);
    try
    {
      const relationEntries = Array.from(relations.entries());
      for (const [part, allowedComponentParts] of relationEntries)
      {
        const initialComponentParts = initialRelations.get(part) ?? [];
        if (_.isEqual(initialComponentParts, allowedComponentParts))
        {
          continue;
        }

        await agent.Enumerations.Parts.updatePermittedComponentParts(part, allowedComponentParts);
        setInitialRelations(new Map(relations));
      }

      toast.success('Successfully updated permitted component part types of parts', { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't update permitted component part types of parts: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setSubmitting(false);
    }
  }, [initialRelations, relations]);

  const relationsChanged = useMemo(() =>
  {
    for (const part of parts)
    {
      const initialComponentParts = initialRelations.get(part.moniker) ?? [];
      const newComponentParts = relations.get(part.moniker) ?? [];
      if (_.xor(initialComponentParts, newComponentParts).length > 0)
      {
        return true;
      }
    }

    return false;
  }, [parts, initialRelations, relations]);

  return (
    <Dimmer.Dimmable>
      <Dimmer inverted active={loading}>
        <Loader />
      </Dimmer>

      <EnumRelationsTable
        inputOptionsCaption='Selected Part Type'
        outputOptionsCaption='Available Component Part Types'
        inputOptions={parts}
        outputOptions={parts}
        relations={initialRelations}
        disabledRelations={disabledRelations}
        onChange={setRelations}
      />
      {canModifyEntityRelation &&
        <Button
          primary
          floated='right'
          content='Update'
          disabled={!relationsChanged}
          loading={submitting}
          style={{ margin: '1%' }} // 1% to be aligned with enum relations table which is 99% wide
          onClick={updateRelationsHandler}
        />
      }
    </Dimmer.Dimmable>
  );
};

export default PartComponents;
