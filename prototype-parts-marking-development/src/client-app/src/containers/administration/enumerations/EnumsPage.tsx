import React, { SyntheticEvent, useCallback, useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { Container, Dropdown, DropdownProps, Header } from 'semantic-ui-react';

import agent from '../../../api/agent';
import {
  AllEnumerations,
  EvidenceYearsEnum,
  GateLevelsEnum,
  LocationsEnum,
  OutletsEnum,
  PartsEnum,
  ProductGroupsEnum,
  getEnumEndpoint,
} from '../../../api/enumerations';
import { extractErrorDetails } from '../../../api/utilities';
import AddEnumItemForm from '../../../components/administration/enumerations/AddEnumItemForm';
import Enum from '../../../components/administration/enumerations/Enum';
import usePermissions from '../../../hooks/usePermissions';
import { EnumItemsResponse } from '../../../models/api/responses';
import { toastDistinctError } from '../../../utilities/toast';

enum DisplayMode
{
  View,
  Add,
}

const EnumsPage: React.FC = () =>
{
  const permissions = usePermissions();

  const [displayMode, setDisplayMode] = useState(DisplayMode.View);
  const [selectedEnum, setSelectedEnum] = useState('');
  const [enumeration, setEnumeration] = useState<EnumItemsResponse>();
  const [pageNumber, setPageNumber] = useState(1);

  useEffect(() =>
  {
    const fetchEnumeration = async () =>
    {
      if (!selectedEnum)
      {
        return;
      }

      try
      {
        const enumEndpoint = getEnumEndpoint(selectedEnum);
        const response = await agent.Enumerations[enumEndpoint].listItems(pageNumber);
        setEnumeration(response);
      }
      catch (error)
      {
        setEnumeration(undefined);
        toastDistinctError(
          `Couldn't list enumeration ${selectedEnum} page ${pageNumber}:`, extractErrorDetails(error));
      }
    };

    fetchEnumeration();
  }, [selectedEnum, pageNumber]);

  const addItemHandler = async (values: any) =>
  {
    try
    {
      const enumEndpoint = getEnumEndpoint(selectedEnum);
      await agent.Enumerations[enumEndpoint].createItem(values);
      toast.success(`Successfully added enumeration item to ${selectedEnum}`, { autoClose: 5000 });
      const response = await agent.Enumerations[enumEndpoint].listItems(enumeration!.pagination.totalPages);
      setEnumeration(response);
      setPageNumber(enumeration!.pagination.totalPages);
      setDisplayMode(DisplayMode.View);
    }
    catch (error)
    {
      toast.error(`Couldn't add enumeration item: ${extractErrorDetails(error)}`);
      setDisplayMode(DisplayMode.View);
    }
  };

  const selectedEnumDropdownChangeHandler = useCallback((_event: SyntheticEvent, data: DropdownProps) =>
  {
    setSelectedEnum(data.value ? String(data.value) : '');
    setPageNumber(1);
  }, []);

  const canModifyEnumeration = () =>
  {
    switch (selectedEnum)
    {
      case EvidenceYearsEnum:
        return permissions.canModifyEvidenceYear;
      case GateLevelsEnum:
        return permissions.canModifyGateLevel;
      case LocationsEnum:
        return permissions.canModifyLocation;
      case OutletsEnum:
        return permissions.canModifyOutlet;
      case PartsEnum:
        return permissions.canModifyPart;
      case ProductGroupsEnum:
        return permissions.canModifyProductGroup;
      default:
        return false;
    }
  };

  let content;
  if (!enumeration)
  {
    content = undefined;
  }
  else
  {
    switch (displayMode)
    {
      case DisplayMode.View:
        content = (
          <Enum
            name={selectedEnum}
            items={enumeration.items}
            itemDescriptor={AllEnumerations[selectedEnum as keyof typeof AllEnumerations]}
            pageNumber={pageNumber}
            totalPages={enumeration.pagination.totalPages}
            showAddButton={canModifyEnumeration()}
            onPageNumberChange={setPageNumber}
            onAddButtonClick={() => setDisplayMode(DisplayMode.Add)}
          />);
        break;
      case DisplayMode.Add:
        content = (
          <AddEnumItemForm
            enumerationName={selectedEnum}
            itemDescriptor={AllEnumerations[selectedEnum as keyof typeof AllEnumerations]}
            onSubmit={addItemHandler}
            onCancel={() => setDisplayMode(DisplayMode.View)}
          />);
        break;
    }
  }

  return (
    <Container>
      <Header as='h2'>
        <Header.Content>
          Enumeration:&nbsp;
          <Dropdown
            inline
            placeholder='select any'
            selectOnBlur={false}
            selectOnNavigation={false}
            value={selectedEnum}
            onChange={selectedEnumDropdownChangeHandler}
            options={Object.getOwnPropertyNames(AllEnumerations)
              .map(enumName => { return { text: enumName, value: enumName }; })}
          />
        </Header.Content>
      </Header>

      <Container style={{ marginTop: '2em' }}>
        {content}
      </Container>
    </Container>
  );
};

export default EnumsPage;
