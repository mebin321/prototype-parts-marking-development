import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { Form as FinalForm } from 'react-final-form';
import { RouteComponentProps, withRouter } from 'react-router-dom';
import { toast } from 'react-toastify';
import { Button, Header, Icon, Segment } from 'semantic-ui-react';

import agent from '../../../api/agent';
import { TaggedData, extractErrorDetails, extractErrorDetailsFromPutResponse } from '../../../api/utilities';
import RestoreItemPrompt from '../../../components/items/RestoreItemPrompt';
import ScrapItemsPrompt from '../../../components/items/ScrapItemsPrompt';
import usePermissions from '../../../hooks/usePermissions';
import { IPackage } from '../../../models/api/items/package/package';
import { parseIdFromParameter } from '../../../utilities/routing';
import { toastDistinctError } from '../../../utilities/toast';
import { ItemFormMode, addToPrintList } from '../itemsUtilities';
import PackageForm from './PackageForm';
import { formatPackagePartCode, generatePackagePrintLabel } from './packageUtilities';

interface IPackageDetailsProps extends RouteComponentProps<{packageId: string}>
{
}

const PackageDetails: React.FC<IPackageDetailsProps> = ({
  history,
  match,
}) =>
{
  const {
    canModifyPrototypePackages,
    canReactivatePrototypesPackages,
    canScrapPrototypesPackages,
    canModifyPrintingLabels,
  } = usePermissions();
  const mode = canModifyPrototypePackages ? ItemFormMode.Edit : ItemFormMode.View;

  const [packageData, setPackageData] = useState<TaggedData<IPackage>>();
  const [loading, setLoading] = useState(false);
  const [showScrapRestoreConfirm, setShowScrapRestoreConfirm] = useState(false);
  const [deletingRestoring, setDeletingRestoring] = useState(false);

  const isScrapped = packageData?.deletedAt !== null;

  const loadPackage = useCallback(async () =>
  {
    setLoading(true);
    try
    {
      const packageId = parseIdFromParameter('package', match.params.packageId);
      const data = await agent.Packages.read(packageId);
      setPackageData(data);
    }
    catch (error)
    {
      toastDistinctError(`Couldn't load package with ID ${match.params.packageId}:`, extractErrorDetails(error));
    }
    finally
    {
      setLoading(false);
    }
  }, [match.params.packageId]);

  useEffect(() =>
  {
    loadPackage();
  }, [loadPackage]);

  const finalFormSubmitHandler = useCallback(async (values: any) =>
  {
    try
    {
      if (!packageData) return;

      const packageUpdateData =
      {
        actualCount: values.numberOfItems,
        owner: values.owner?.id,
        comment: values.comment,
      };

      await agent.Packages.update(packageData.id, packageData.etag, packageUpdateData);
      toast.success(`Successfully updated package ${packageData.id}`, { autoClose: 5000 });
      loadPackage();
    }
    catch (error)
    {
      toast.error(`Couldn't update package: ${extractErrorDetailsFromPutResponse(error)}`);
    }
  }, [packageData, loadPackage]);

  const scrapPackageHandler = useCallback(async () =>
  {
    try
    {
      if (!packageData) return;
      setDeletingRestoring(true);
      await agent.Packages.scrap(packageData.id);
      loadPackage();
      toast.success(`Successfully scrapped package ${packageData.id}`, { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't scrap package: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapRestoreConfirm(false);
      setDeletingRestoring(false);
    }
  }, [packageData, loadPackage]);

  const restorePackageHandler = useCallback(async () =>
  {
    try
    {
      if (!packageData) return;
      setDeletingRestoring(true);
      await agent.Packages.restore(packageData.id);
      loadPackage();
      toast.success(`Successfully restored package ${packageData.id}`, { autoClose: 5000 });
    }
    catch (error)
    {
      toast.error(`Couldn't restore package: ${extractErrorDetails(error)}`);
    }
    finally
    {
      setShowScrapRestoreConfirm(false);
      setDeletingRestoring(false);
    }
  }, [packageData, loadPackage]);

  const addPackageToPrintList = useCallback(() =>
  {
    if (!packageData) return;
    const printLabelItem = generatePackagePrintLabel(packageData);

    addToPrintList([printLabelItem], () => history.push('/print'));
  }, [packageData, history]);

  const scrapRestorePackagePrompt = useMemo(() =>
  {
    return (isScrapped
      ? <RestoreItemPrompt
          itemType='package'
          visible={showScrapRestoreConfirm}
          onCancel={() => setShowScrapRestoreConfirm(false)}
          loading={deletingRestoring}
          onConfirm={restorePackageHandler}
        />
      : <ScrapItemsPrompt
          itemType='package'
          loading={deletingRestoring}
          visible={showScrapRestoreConfirm}
          onCancel={() => setShowScrapRestoreConfirm(false)}
          onConfirm={scrapPackageHandler}
        />
    );
  }, [showScrapRestoreConfirm, deletingRestoring, isScrapped, scrapPackageHandler, restorePackageHandler]);

  return (
    <div style={{ maxWidth: '750px', margin: 'auto' }}>
      <Segment clearing>
        <Header as='h2' style={{ display: 'inline' }}>
        {isScrapped && 'Scrapped '} Package {packageData?.id}
        </Header>
        <Segment basic compact floated='right' style={{ margin: '0', padding: '0' }}>
          <Button
            basic
            title='Print this package'
            icon={
              <Icon.Group>
                <Icon name='print' />
                <Icon corner='bottom right' name='plus' />
              </Icon.Group>
            }
            color='blue'
            disabled={isScrapped || !canModifyPrintingLabels}
            onClick={addPackageToPrintList}
          />
          {!loading && (
            isScrapped
              ? (canReactivatePrototypesPackages &&
                  <Button
                    basic
                    color='green'
                    title='Restore this package'
                    icon='redo alternate'
                    onClick={() => setShowScrapRestoreConfirm(true)}
                  />
                )
              : (canScrapPrototypesPackages &&
                  <Button
                    basic
                    color='red'
                    title='Scrap this package'
                    icon='trash'
                    onClick={() => setShowScrapRestoreConfirm(true)}
                  />
                )
          )}
        </Segment>
        <Header>{packageData && formatPackagePartCode(packageData)}</Header>
        <FinalForm
          onSubmit={finalFormSubmitHandler}
          render={({ handleSubmit, invalid, pristine, submitting }) => (
            <PackageForm mode={mode}
              data={packageData}
              loading={loading}
              onSubmit={handleSubmit}
            >
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
            </PackageForm>
          )}
        />
      </Segment>
      {scrapRestorePackagePrompt}
    </div>
  );
};

export default withRouter(PackageDetails);
