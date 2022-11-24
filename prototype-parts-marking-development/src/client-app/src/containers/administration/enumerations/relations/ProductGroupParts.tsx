import _ from 'lodash';
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { toast } from 'react-toastify';
import { Button, Dimmer, Loader } from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { extractErrorDetails, listAll } from '../../../../api/utilities';
import EnumRelationsTable from '../../../../components/administration/enumerations/relations/EnumRelationsTable';
import usePermissions from '../../../../hooks/usePermissions';
import { IPartType } from '../../../../models/api/enumerations/parts';
import { IProductGroup } from '../../../../models/api/enumerations/productGroups';
import { toastDistinctError } from '../../../../utilities/toast';

const ProductGroupParts: React.FC = () =>
{
  const { canModifyEntityRelation } = usePermissions();

  const [loading, setLoading] = useState(false);
  const [submitting, setSubmitting] = useState(false);
  const [productGroups, setProductGroups] = useState<IProductGroup[]>([]);
  const [parts, setParts] = useState<IPartType[]>([]);
  const [initialRelations, setInitialRelations] = useState(new Map<string, string[]>());
  const [relations, setRelations] = useState(new Map<string, string[]>());

  const loadRelations = useCallback(async (productGroupMonikers: string[]) =>
  {
    const result = new Map<string, string[]>();
    for (const productGroupMoniker of productGroupMonikers)
    {
      const response = await agent.Enumerations.ProductGroups.listPermittedParts(productGroupMoniker);
      result.set(productGroupMoniker, response.map(item => item.moniker));
    }

    return result;
  }, []);

  const loadData = useCallback(async () =>
  {
    setLoading(true);
    try
    {
      const loadedProductGroups = await listAll(page => agent.Enumerations.ProductGroups.listItems(page));
      const loadedParts = await listAll(page => agent.Enumerations.Parts.listItems(page));
      const loadedRelations = await loadRelations(loadedProductGroups.map(productGroup => productGroup.moniker));

      setProductGroups(loadedProductGroups);
      setParts(loadedParts);
      setInitialRelations(loadedRelations);
    }
    catch (error)
    {
      toastDistinctError('Couldn\'t load permitted parts of product groups:', extractErrorDetails(error));
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
      for (const [productGroup, allowedParts] of relationEntries)
      {
        const initialParts = initialRelations.get(productGroup) ?? [];
        if (_.isEqual(initialParts, allowedParts))
        {
          continue;
        }

        await agent.Enumerations.ProductGroups.updatePermittedParts(productGroup, allowedParts);
        setInitialRelations(new Map(relations));
      }

      toast.success('Successfully updated permitted parts of product groups', { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't update permitted parts of product groups: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setSubmitting(false);
    }
  }, [initialRelations, relations]);

  const relationsChanged = useMemo(() =>
  {
    for (const productGroup of productGroups)
    {
      const initialProductGroups = initialRelations.get(productGroup.moniker) ?? [];
      const newProductGroups = relations.get(productGroup.moniker) ?? [];
      if (_.xor(initialProductGroups, newProductGroups).length > 0)
      {
        return true;
      }
    }

    return false;
  }, [productGroups, initialRelations, relations]);

  return (
    <Dimmer.Dimmable>
      <Dimmer inverted active={loading}>
        <Loader />
      </Dimmer>

      <EnumRelationsTable
        inputOptionsCaption='Selected Product Groups'
        outputOptionsCaption='Available Parts'
        inputOptions={productGroups}
        outputOptions={parts}
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

export default ProductGroupParts;
