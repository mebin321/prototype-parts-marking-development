import _ from 'lodash';
import React, { useCallback, useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { Button, Header, Segment } from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { extractErrorDetails } from '../../../../api/utilities';
import AddVariantForm from '../../../../components/items/part/variant/AddVariantForm';
import { IPrototypeVariant } from '../../../../models/api/items/part/variant/prototypeVariant';
import { IPrototypeVariantsResponse } from '../../../../models/api/responses';
import VariantsList from './VariantsList';

interface IVariantsSectionProps
{
  prototypeSetId: number;
  prototypeId: number;
  etag: string;
  addVariantVisible: boolean;
  addVariantEnabled: boolean;
  onVariantCreated?: (variant: IPrototypeVariant) => void;
}

const VariantsSection: React.FC<IVariantsSectionProps> = ({
  prototypeSetId,
  prototypeId,
  etag,
  addVariantVisible,
  addVariantEnabled,
  onVariantCreated,
}) =>
{
  const [currentVariant, setCurrentVariant] = useState<IPrototypeVariant>();
  const [recentVariants, setRecentVariants] = useState<IPrototypeVariant[]>([]);
  const [pages, setPages] = useState<IPrototypeVariantsResponse[]>([]);
  const [error, setError] = useState('');
  const [isLoading, setLoading] = useState(false);
  const [isSubmitting, setSubmitting] = useState(false);

  const [isAddFormVisible, setAddFormVisible] = useState(false);

  const getVariants = useCallback(async (page: number, pageSize?: number) =>
  {
    try
    {
      setLoading(true);
      return await agent.Prototypes.listVariants(prototypeSetId, prototypeId, page, pageSize);
    }
    catch (error)
    {
      setError(`Couldn't load variants of part ${prototypeId} due ${extractErrorDetails(error)}`);
    }
    finally
    {
      setLoading(false);
    }
  }, [prototypeId, prototypeSetId]);

  const loadVariants = useCallback(async (page: number) =>
  {
    const response = await getVariants(page);
    if (!response) return;

    setPages(prevPages => prevPages.concat(response));
  }, [getVariants]);

  const createVariant = useCallback(async (comment: string, resetForm: () => void) =>
  {
    try
    {
      setSubmitting(true);
      const createdVariant = await agent.Variants.create(prototypeSetId, prototypeId, etag, { comment: comment });
      setRecentVariants(prevRecentVariants =>
        prevRecentVariants.length < 1 && currentVariant
          ? [createdVariant, currentVariant] // at least one variant exists and recent variants array is empty
          : [createdVariant, ...prevRecentVariants]); // prepend created variant to recent variants array
      setAddFormVisible(false);
      resetForm();
      if (onVariantCreated) onVariantCreated(createdVariant);
      toast.success(`Successfully created variant v${createdVariant.version}`, { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't create variant of part ${prototypeId} due ${extractErrorDetails(error)}`);
    }
    finally
    {
      setSubmitting(false);
    }
  }, [prototypeSetId, prototypeId, etag, currentVariant, onVariantCreated]);

  // load current variant on page load
  useEffect(() =>
  {
    const loadCurrentVariant = async () =>
    {
      const response = await getVariants(1, 1);
      setCurrentVariant(_.first(response?.items));
    };

    loadCurrentVariant();
  }, [getVariants]);

  useEffect(() =>
  {
    setCurrentVariant(prevCurrentVariant =>
    {
      const latestCreatedVariant = _.first(recentVariants);
      if (latestCreatedVariant)
      {
        return prevCurrentVariant?.id === latestCreatedVariant.id ? prevCurrentVariant : latestCreatedVariant;
      }

      const latestLoadedVariant = _.first(pages.flatMap(page => page.items));
      if (latestLoadedVariant)
      {
        return prevCurrentVariant?.id === latestLoadedVariant.id ? prevCurrentVariant : latestLoadedVariant;
      }

      return prevCurrentVariant;
    });
  }, [recentVariants, pages]);

  return (
    <Segment>
    <Header as='h2' style={{ display: 'inline' }}>Variant</Header>
    <Segment basic compact floated='right' style={{ margin: '0', padding: '0' }}>
      {addVariantVisible &&
        <Button
          basic={!isAddFormVisible}
          color='green'
          title='Add new variant'
          icon='plus'
          disabled={!addVariantEnabled && !isAddFormVisible}
          onClick={() => setAddFormVisible(prevVisible => !prevVisible)}
        />
      }
    </Segment>
    <AddVariantForm
      version={currentVariant ? currentVariant.version + 1 : 1}
      style={{ display: isAddFormVisible ? 'block' : 'none' }}
      disabled={!addVariantEnabled}
      submitting={isSubmitting}
      onCancel={() => setAddFormVisible(false)}
      onSubmit={createVariant}
    />
    <VariantsList
      currentVariant={currentVariant}
      recentVariants={recentVariants}
      pages={pages}
      error={error}
      isLoading={isLoading}
      onAdditionalVariantsRequested={loadVariants}
    />
  </Segment>
  );
};

export default VariantsSection;
