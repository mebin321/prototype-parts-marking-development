import React, { useCallback, useState } from 'react';
import { Form as FinalForm } from 'react-final-form';
import { RouteComponentProps } from 'react-router-dom';
import { toast } from 'react-toastify';
import { combineValidators, composeValidators, isRequired } from 'revalidate';
import { Button, Header, Segment } from 'semantic-ui-react';

import agent from '../../../api/agent';
import { extractErrorDetails } from '../../../api/utilities';
import { IPackageCreateData } from '../../../models/api/items/package/packageCreateData';
import IPartCode from '../../../models/partCode';
import { isGreaterThan, isNotEmpty } from '../../../utilities/validation/validators';
import { ItemFormMode, formatPartCode } from '../itemsUtilities';
import PackageForm from './PackageForm';

// Form Validation
const validate = combineValidators({
  location: isRequired('Location'),
  outlet: isRequired('Outlet'),
  productGroup: isNotEmpty({ message: 'Product group is required' }),
  gateLevel: isRequired('Gate level'),
  part: isNotEmpty({ message: 'Parts is required' }),
  numberOfItems: composeValidators(isRequired, isGreaterThan(1))('Number of items'),
  customer: isRequired('Customer'),
  project: isRequired('Projects'),
  owner: isRequired('Owner'),
});

interface INewPackageFormProps extends RouteComponentProps
{
}

const NewPackageForm: React.FC<INewPackageFormProps> = ({
  history,
}) =>
{
  const [partCodePreview, setPartCodePreview] = useState('');

  const finalFormSubmitHandler = async (values: any) =>
  {
    try
    {
      const packageCreateData: IPackageCreateData =
      {
        outletMoniker: values.outlet.moniker,
        productGroupMoniker: values.productGroup.moniker,
        locationMoniker: values.location.moniker,
        gateLevelMoniker: values.gateLevel.moniker,
        partMoniker: values.part.moniker,
        evidenceYear: new Date().getFullYear(),
        initialCount: values.numberOfItems,
        comment: values.comment,
        customer: values.project.price,
        project: values.project.description,
        projectNumber: values.project.title,
        ownerId: values.owner.id,
      };

      const response = await agent.Packages.create(packageCreateData);
      toast.success('Successfully created package', { autoClose: 5000 });
      history.push(`/packages/${response.id}`);
    }
    catch (error)
    {
      toast.error(`Couldn't add package: ${extractErrorDetails(error)}`);
    }
  };

  const formFieldsChangeHandler = useCallback(
    (partCode: Partial<IPartCode>) => setPartCodePreview(formatPartCode(partCode)), []);

  return (
    <div style={{ maxWidth: '750px', margin: 'auto' }}>
      <Segment clearing>
        <Header as='h2'>New package</Header>
        <FinalForm
          onSubmit={finalFormSubmitHandler}
          validate={validate}
          render={({ handleSubmit, invalid, pristine, submitting }) => (
            <PackageForm
              mode={ItemFormMode.Create}
              onChange={formFieldsChangeHandler}
              onSubmit={handleSubmit}
            >
              <Button
                onClick={() => history.goBack()}
                floated='left'
                type='button'
                content='Cancel'
                basic
              />
              <Button
                floated='right'
                type='submit'
                content='Add package'
                primary
                disabled={pristine || invalid}
                loading={submitting}
              />
            </PackageForm>
          )}
        />
      </Segment>
      <Header as='h2'>PartCode preview: <span style={{ color: 'black' }} >{partCodePreview}</span></Header>
    </div>
  );
};

export default NewPackageForm;
