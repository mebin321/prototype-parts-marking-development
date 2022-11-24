import React, { useCallback, useState } from 'react';
import { Form as FinalForm } from 'react-final-form';
import { RouteComponentProps } from 'react-router-dom';
import { toast } from 'react-toastify';
import { combineValidators, isRequired } from 'revalidate';
import { Button, Header, Segment } from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { extractErrorDetails } from '../../../../api/utilities';
import { IPrototypeSetCreateData } from '../../../../models/api/items/part/set/prototypeSetCreateData';
import IPartCode from '../../../../models/partCode';
import { isNotEmpty } from '../../../../utilities/validation/validators';
import { ItemFormMode, formatPartCode } from '../../itemsUtilities';
import PrototypeSetForm from './PrototypeSetForm';

// Form Validation
const validate = combineValidators({
  location: isRequired('Location'),
  outlet: isRequired('Outlet'),
  productGroup: isNotEmpty({ message: 'Product group is required' }),
  gateLevel: isRequired('Gate level'),
  customer: isRequired('Customer'),
  project: isRequired('Projects'),
});

interface INewPrototypeSetFormProps extends RouteComponentProps
{
}

const NewPrototypeSetForm: React.FC<INewPrototypeSetFormProps> = ({
  history,
}) =>
{
  const [partCodePreview, setPartCodePreview] = useState('');

  const finalFormSubmitHandler = async (values: any) =>
  {
    try
    {
      const prototypeSetCreateData: IPrototypeSetCreateData =
      {
        outletMoniker: values.outlet.moniker,
        productGroupMoniker: values.productGroup.moniker,
        locationMoniker: values.location.moniker,
        gateLevelMoniker: values.gateLevel.moniker,
        evidenceYear: new Date().getFullYear(),
        customer: values.project.price,
        project: values.project.description,
        projectNumber: values.project.title,
      };

      const response = await agent.PrototypeSets.create(prototypeSetCreateData);
      history.push({ pathname: `/prototype-sets/${response.id}/prototypes/new` });
    }
    catch (error)
    {
      toast.error(`Couldn't add prototype set: ${extractErrorDetails(error)}`);
    }
  };

  const formFieldsChangeHandler = useCallback(
    (partCode: Partial<IPartCode>) => setPartCodePreview(formatPartCode(partCode)), []);

  return (
    <div style={{ maxWidth: '750px', margin: 'auto' }}>
      <Segment clearing>
        <Header as='h2'>New set of prototypes</Header>
        <FinalForm
          onSubmit={finalFormSubmitHandler}
          validate={validate}
          render={({ handleSubmit, invalid, pristine, submitting }) => (
            <PrototypeSetForm
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
                content='Next'
                primary
                disabled={pristine || invalid}
                loading={submitting}
              />
            </PrototypeSetForm>
          )}
        />
      </Segment>
      <Header as='h2'>PartCode preview: <span style={{ color: 'black' }} >{partCodePreview}</span></Header>
    </div>

  );
};

export default NewPrototypeSetForm;
