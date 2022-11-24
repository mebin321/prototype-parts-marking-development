import React, { useEffect, useMemo, useState } from 'react';
import { Field, Form as FinalForm } from 'react-final-form';
import { ConfiguredValidator, combineValidators } from 'revalidate';
import { Button, Form, Segment } from 'semantic-ui-react';

import { IEnumItem, IEnumItemDescriptor } from '../../../models/api/enumerations';
import { prettifyPropertyName } from '../../../utilities/objects';
import TextField from '../../common/form/TextField';
import Loader from '../../common/ui/Loader';

interface IAddEnumItemFormProps
{
  enumerationName: string;
  itemDescriptor: IEnumItemDescriptor;
  onSubmit: (data: any) => void;
  onCancel: () => void;
}

const AddEnumItemForm: React.FC<IAddEnumItemFormProps> = ({
  enumerationName,
  itemDescriptor,
  onSubmit,
  onCancel,
}) =>
{
  const [loading, setLoading] = useState(false);

  // reset loading state when component is mounted
  useEffect(() =>
  {
    setLoading(false);
  }, []);

  const finalFormSubmitHandler = async (values: any) =>
  {
    setLoading(true);
    const item: IEnumItem = { ...values };
    item.code = values.code.toUpperCase();
    onSubmit(item);
  };

  const validators = useMemo(() =>
  {
    const validators: {[index: string]: ConfiguredValidator} = {};
    itemDescriptor.initialProperties.forEach(propertyName =>
    {
      const property = itemDescriptor.properties[propertyName];
      if (property.validator)
      {
        validators[propertyName] = property.validator;
      }
    });

    return combineValidators(validators);
  }, [itemDescriptor]);

  if (loading) return <Loader content={`Adding ${enumerationName} item`} />;

  return (
    <Segment clearing style={{ maxWidth: '650px' }}>
      <h3>New {enumerationName} item</h3>
      <FinalForm
        onSubmit={finalFormSubmitHandler}
        validate={validators}
        render={({ handleSubmit, invalid, pristine }) => (
          <Form onSubmit={handleSubmit}>
            {
              itemDescriptor.initialProperties.map(propertyName =>
              {
                const property = itemDescriptor.properties[propertyName];
                return (
                  <Field
                    key={propertyName}
                    type={property.type}
                    label={prettifyPropertyName(propertyName)}
                    name={propertyName}
                    component={TextField}
                  />
                );
              })
            }
            <Button
              floated='right'
              type='submit'
              content='Create'
              primary
              disabled={invalid || pristine}
            />
            <Button
              onClick={onCancel}
              floated='left'
              type='button'
              content='Cancel'
              basic
            />
          </Form>
        )}
      />
    </Segment>
  );
};

export default AddEnumItemForm;
