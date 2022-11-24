import _ from 'lodash';
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { toast } from 'react-toastify';
import { Button, Dimmer, Loader } from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { extractErrorDetails, listAll } from '../../../../api/utilities';
import EnumRelationsTable from '../../../../components/administration/enumerations/relations/EnumRelationsTable';
import usePermissions from '../../../../hooks/usePermissions';
import { IOutlet } from '../../../../models/api/enumerations/outlets';
import { IProductGroup } from '../../../../models/api/enumerations/productGroups';
import { toastDistinctError } from '../../../../utilities/toast';

const OutletProductGroups: React.FC = () =>
{
  const { canModifyEntityRelation } = usePermissions();

  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [outlets, setOutlets] = useState<IOutlet[]>([]);
  const [productGroups, setProductGroups] = useState<IProductGroup[]>([]);
  const [initialRelations, setInitialRelations] = useState(new Map<string, string[]>());
  const [relations, setRelations] = useState(new Map<string, string[]>());

  const loadRelations = useCallback(async (outletMonikers: string[]) =>
  {
    const result = new Map<string, string[]>();
    for (const outletMoniker of outletMonikers)
    {
      const response = await agent.Enumerations.Outlets.listPermittedProductGroups(outletMoniker);
      result.set(outletMoniker, response.map(item => item.moniker));
    }

    return result;
  }, []);

  const loadData = useCallback(async () =>
  {
    setLoading(true);
    try
    {
      const loadedOutlets = await listAll(page => agent.Enumerations.Outlets.listItems(page));
      const loadedProductGroups = await listAll(page => agent.Enumerations.ProductGroups.listItems(page));
      const loadedRelations = await loadRelations(loadedOutlets.map(outlet => outlet.moniker));

      setOutlets(loadedOutlets);
      setProductGroups(loadedProductGroups);
      setInitialRelations(loadedRelations);
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t load permitted product groups of outlets:', extractErrorDetails(error));
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
      for (const [outlet, allowedProductGroups] of relationEntries)
      {
        const initialProductGroups = initialRelations.get(outlet) ?? [];
        if (_.isEqual(initialProductGroups, allowedProductGroups))
        {
          continue;
        }

        await agent.Enumerations.Outlets.updatePermittedProductGroups(outlet, allowedProductGroups);
        setInitialRelations(new Map(relations));
      }

      toast.success('Successfully updated permitted product groups of outlets', { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't update permitted product groups of outlets: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setSubmitting(false);
    }
  }, [initialRelations, relations]);

  const relationsChanged = useMemo(() =>
  {
    for (const outlet of outlets)
    {
      const initialProductGroups = initialRelations.get(outlet.moniker) ?? [];
      const newProductGroups = relations.get(outlet.moniker) ?? [];
      if (_.xor(initialProductGroups, newProductGroups).length > 0)
      {
        return true;
      }
    }

    return false;
  }, [outlets, initialRelations, relations]);

  return (
    <Dimmer.Dimmable>
      <Dimmer inverted active={loading}>
        <Loader />
      </Dimmer>

      <EnumRelationsTable
        inputOptionsCaption='Selected Outlet'
        outputOptionsCaption='Available Product Groups'
        inputOptions={outlets}
        outputOptions={productGroups}
        relations={initialRelations}
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

export default OutletProductGroups;
