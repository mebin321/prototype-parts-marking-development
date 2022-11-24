import React, { SyntheticEvent, useCallback, useEffect, useMemo, useState } from 'react';
import { Field } from 'react-final-form';
import
{
  Form,
  InputOnChangeData,
  SearchResultData,
} from 'semantic-ui-react';

import agent from '../../../api/agent';
import { GateLevelsEnum, LocationsEnum, OutletsEnum, PartsEnum, ProductGroupsEnum } from '../../../api/enumerations';
import FormattedTextAreaField from '../../../components/common/form/FormattedTextAreaField';
import SearchField from '../../../components/common/form/SearchField';
import SpinnerField from '../../../components/common/form/SpinnerField';
import { useLocalStorage } from '../../../hooks/useLocalStorage';
import { EmptyTextualEnumItem } from '../../../models/api/enumerations';
import { IGateLevel } from '../../../models/api/enumerations/gateLevels';
import { ILocation } from '../../../models/api/enumerations/locations';
import { IOutlet } from '../../../models/api/enumerations/outlets';
import { IPartType } from '../../../models/api/enumerations/parts';
import { IProductGroup } from '../../../models/api/enumerations/productGroups';
import { IPackage } from '../../../models/api/items/package/package';
import { IViewableCustomer } from '../../../models/api/projects/viewableCustomer';
import { IViewableProject } from '../../../models/api/projects/viewableProject';
import { IViewableUser } from '../../../models/api/users/viewableUser';
import IPartCode from '../../../models/partCode';
import { updateObject } from '../../../store/utilities';
import { isProjectSearchQueryValidator } from '../../../utilities/validation/validators';
import
{
  ItemFormMode,
  convertUserForSearchInput,
  generateCustomersSearchHandler,
  generateEnumItemResultSelectHandler,
  generateEnumItemSearchHandler,
  generateOutletResultSelectHandler,
  generatePartsSearchHandler,
  generateProductGroupResultSelectHandler,
  generateProductGroupSearchHandler,
  generateProjectsSearchHandler,
  generateUserSearchHandler,
  getInitialGateLevel,
  getInitialLocation,
  getInitialOutlet,
  getInitialPartType,
  getInitialProductGroup,
} from '../itemsUtilities';

interface IPackageFormProps
{
  mode: ItemFormMode;
  data?: IPackage;
  loading?: boolean;
  onChange?: (partCode: Partial<IPartCode>) => void;
  onSubmit?: (event: SyntheticEvent) => void;
}

const PackageForm: React.FC<IPackageFormProps> = ({
  mode,
  data,
  loading,
  children,
  onChange,
  onSubmit,
}) =>
{
  const [storedOutlet, setStoredOutlet] = useLocalStorage<IOutlet>(OutletsEnum);
  const [storedProductGroup, setStoredProductGroup] = useLocalStorage<IProductGroup>(ProductGroupsEnum);
  const [storedPart, setStoredPart] = useLocalStorage<IPartType>(PartsEnum);
  const [storedLocation, setStoredLocation] = useLocalStorage<ILocation>(LocationsEnum);
  const [storedGateLevel, setStoredGateLevel] = useLocalStorage<IGateLevel>(GateLevelsEnum);
  const [storedOwner, setStoredOwner] = useLocalStorage<IViewableUser>('owner');

  const [project, setProject] = useState<IViewableProject>();
  const [customer, setCustomer] = useState<IViewableCustomer>();
  const [projectsOptions, setProjectOptions] = useState<IViewableProject[]>();
  const [customersOptions, setCustomersOptions] = useState<IViewableCustomer[]>();

  const [outletsFiltered, setOutletsFiltered] = useState<IOutlet[]>([]);
  const [productGroupsFiltered, setProductGroupsFiltered] = useState<IProductGroup[]>([]);
  const [partsFiltered, setPartsFiltered] = useState<IPartType[]>([]);
  const [locationsFiltered, setLocationsFiltered] = useState<ILocation[]>([]);
  const [gateLevelsFiltered, setGateLevelsFiltered] = useState<IGateLevel[]>([]);
  const [ownersFiltered, setOwnersFiltered] = useState<IViewableUser[]>([]);
  const [partCode, setPartCode] = useState<Partial<IPartCode>>({ numberOfPrototypes: 0 });

  const outletsSearchHandler = useMemo(() =>
    generateEnumItemSearchHandler(agent.Enumerations.Outlets, setOutletsFiltered), []);
  const productGroupsSearchHandler = useMemo(() =>
    generateProductGroupSearchHandler(storedOutlet?.moniker ?? '', setProductGroupsFiltered), [storedOutlet]);
  const partsSearchHandler = useMemo(() =>
    generatePartsSearchHandler(storedProductGroup?.moniker ?? '', setPartsFiltered), [storedProductGroup]);
  const locationsSearchHandler = useMemo(() =>
    generateEnumItemSearchHandler(agent.Enumerations.Locations, setLocationsFiltered), []);
  const gateLevelsSearchHandler = useMemo(() =>
    generateEnumItemSearchHandler(agent.Enumerations.GateLevels, setGateLevelsFiltered), []);

  const ownersSearchHandler = useMemo(() => generateUserSearchHandler(setOwnersFiltered), []);
  const projectsSearchHandler = useMemo(() =>
    generateProjectsSearchHandler(setProjectOptions, customer?.title), [customer]);
  const customersSearchHandler = useMemo(() =>
    generateCustomersSearchHandler(setCustomersOptions), []);

  const outletResultSelectHandler = useMemo(() =>
  {
    return generateOutletResultSelectHandler(
      value => setPartCode(prevPartCode => updateObject(prevPartCode, { outlet: value })),
      setStoredOutlet,
      setProductGroupsFiltered
    );
  }, [setStoredOutlet]);
  const productGroupResultSelectHandler = useMemo(() =>
  {
    return generateProductGroupResultSelectHandler(
      value => setPartCode(prevPartCode => updateObject(prevPartCode, { productGroup: value })),
      setStoredProductGroup,
      setPartsFiltered
    );
  }, [setStoredProductGroup]);
  const partResultSelectHandler = useMemo(() =>
  {
    return generateEnumItemResultSelectHandler<IPartType>(
      value => setPartCode(prevPartCode => updateObject(prevPartCode, { partType: value })),
      setStoredPart
    );
  }, [setStoredPart]);
  const locationResultSelectHandler = useMemo(() =>
  {
    return generateEnumItemResultSelectHandler<ILocation>(
      value => setPartCode(prevPartCode => updateObject(prevPartCode, { location: value })),
      setStoredLocation
    );
  }, [setStoredLocation]);
  const gateLevelResultSelectHandler = useMemo(() =>
  {
    return generateEnumItemResultSelectHandler<IGateLevel>(
      value => setPartCode(prevPartCode => updateObject(prevPartCode, { gateLevel: value })),
      setStoredGateLevel
    );
  }, [setStoredGateLevel]);
  const ownerResultSelectHandler = useCallback((_event: SyntheticEvent, data: SearchResultData) =>
  {
    setStoredOwner(data.result);
  }, [setStoredOwner]);
  const projectResultSelectHandler = useCallback((_event: SyntheticEvent, data: SearchResultData) =>
  {
    setProject(data.result);
    if (data.result !== undefined)
    {
      setCustomer({ title: data.result.price });
    }
  }, []);
  const customerResultSelectHandler = useCallback((_event: SyntheticEvent, data: SearchResultData) =>
  {
    setCustomer(data.result);
  }, []);
  const numberOfItemsChangeHandler = useCallback((_e: SyntheticEvent, d: InputOnChangeData) =>
  {
    // set number of items in part code only when creating new package
    if (mode === ItemFormMode.Create)
    {
      const value = d.value ? +d.value : 0;
      setPartCode(prevPartCode => updateObject(prevPartCode, { numberOfPrototypes: value }));
    }
  }, [mode]);

  const initialOutlet =
    getInitialOutlet(mode, data?.outletTitle, data?.outletCode, storedOutlet);
  const initialProductGroup =
    getInitialProductGroup(mode, data?.productGroupTitle, data?.productGroupCode, storedProductGroup);
  const initialPartType =
    getInitialPartType(mode, data?.partTypeTitle, data?.partTypeCode, storedPart);
  const initialLocation =
    getInitialLocation(mode, data?.locationTitle, data?.locationCode, storedLocation);
  const initialGateLevel =
    getInitialGateLevel(mode, data?.gateLevelTitle, data?.gateLevelCode, storedGateLevel);
  const initialOwner =
    mode === ItemFormMode.Create ? storedOwner : convertUserForSearchInput(data?.owner);

  // initialize part code from existing package field values
  useEffect(() =>
  {
    const partCodeFromData: Partial<IPartCode> =
    {
      outlet: data?.outletCode,
      productGroup: data?.productGroupCode,
      partType: data?.partTypeCode,
      evidenceYear: data?.evidenceYearCode,
      location: data?.locationCode,
      uniqueIdentifier: data?.packageIdentifier,
      gateLevel: data?.gateLevelCode,
      numberOfPrototypes: data?.initialCount ?? 0,
    };

    setPartCode(prevPartCode => updateObject(prevPartCode, partCodeFromData));
  }, [data]);

  useEffect(() =>
  {
    const initialPartCode: Partial<IPartCode> =
    {
      outlet: initialOutlet?.code,
      productGroup: initialProductGroup?.code,
      partType: initialPartType?.code,
      location: initialLocation?.code,
      gateLevel: initialGateLevel?.code,
    };

    setPartCode(prevPartCode => updateObject(prevPartCode, initialPartCode));
    // empty dependencies array is given to execute update from initial values only on component mount
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() =>
  {
    if (data)
    {
      setCustomer({ title: data.customer });

      setProject({
        title: data.projectNumber,
        description: data.project,
        price: data.customer,
      });
    }
  }, [data]);

  useEffect(() => { if (onChange) onChange(partCode); }, [partCode, onChange]);

  useEffect(() =>
  {
    if (!storedProductGroup) return;
    if (storedOutlet &&
      productGroupsFiltered.length > 0 &&
      productGroupsFiltered.every(item => item.moniker !== storedProductGroup.moniker))
    {
      setStoredProductGroup(EmptyTextualEnumItem);
      setStoredPart(EmptyTextualEnumItem);
    }

    if (!storedOutlet)
    {
      setStoredProductGroup(EmptyTextualEnumItem);
      setStoredPart(EmptyTextualEnumItem);
    }
    // StoredProductGroup is omitted from dependencies array to not run this effect on that change
    // to prevent infinite loop because storedProductGroup setter is in executing area
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [storedOutlet, productGroupsFiltered]);

  useEffect(() =>
  {
    if (!storedPart) return;
    if (storedProductGroup &&
      partsFiltered.length > 0 &&
      partsFiltered.every(item => item.moniker !== storedPart.moniker))
    {
      setStoredPart(EmptyTextualEnumItem);
    }

    if (!storedProductGroup)
    {
      setStoredPart(EmptyTextualEnumItem);
    }
    // StoredPart is omitted from dependencies array to not run this effect on that change
    // to prevent infinite loop because storedProductGroup setter is in executing area
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [storedProductGroup, partsFiltered]);

  return (
    <Form loading={loading} onSubmit={onSubmit}>
      <Form.Group widths='two'>
        <Field
          label='Outlet'
          name='outlet'
          readOnly={mode !== ItemFormMode.Create}
          options={outletsFiltered}
          defaultSelection={initialOutlet}
          onSearchChange={outletsSearchHandler}
          onResultSelect={outletResultSelectHandler}
          component={SearchField}
        />
        <Field
          label='Product Group'
          name='productGroup'
          placeholder= {!storedOutlet ? 'Select outlet first' : undefined}
          readOnly={mode !== ItemFormMode.Create}
          options={productGroupsFiltered}
          defaultSelection={initialProductGroup} // must be in defaultSelection to prevent infinite loop
          selection={mode === ItemFormMode.Create ? storedProductGroup : undefined}
          disabled={mode === ItemFormMode.Create && !storedOutlet}
          onSearchChange={productGroupsSearchHandler}
          onResultSelect={productGroupResultSelectHandler}
          component={SearchField}
        />
      </Form.Group>

      <Form.Group widths='two'>
        <Field
          label='Part'
          name='part'
          placeholder= {!storedProductGroup?.moniker ? 'Select Product Group first' : undefined}
          readOnly={mode !== ItemFormMode.Create}
          options={partsFiltered}
          defaultSelection={ initialPartType} // must be in defaultSelection to prevent infinite loop
          selection={mode === ItemFormMode.Create ? storedPart : undefined}
          disabled={mode === ItemFormMode.Create && (!storedOutlet || !storedProductGroup?.title)}
          onSearchChange={partsSearchHandler}
          onResultSelect={partResultSelectHandler}
          component={SearchField}
        />
        <Field
          label='Location'
          name='location'
          readOnly={mode !== ItemFormMode.Create}
          options={locationsFiltered}
          defaultSelection={initialLocation}
          onSearchChange={locationsSearchHandler}
          onResultSelect={locationResultSelectHandler}
          component={SearchField}
        />
      </Form.Group>

      <Form.Group widths='two'>
        <Field
          label='Gate Level'
          name='gateLevel'
          readOnly={mode !== ItemFormMode.Create}
          options={gateLevelsFiltered}
          defaultSelection={initialGateLevel}
          onSearchChange={gateLevelsSearchHandler}
          onResultSelect={gateLevelResultSelectHandler}
          component={SearchField}
        />
        <Field
          label='Owner'
          name='owner'
          placeholder='Select owner'
          readOnly={mode === ItemFormMode.View}
          options={ownersFiltered}
          defaultSelection={initialOwner}
          onSearchChange={ownersSearchHandler}
          onResultSelect={ownerResultSelectHandler}
          component={SearchField}
        />
      </Form.Group>

      <Form.Group widths='two'>
        {
          !loading &&
          <Field
            label='Comment'
            name='comment'
            placeholder='Comment is optional'
            readOnly={mode === ItemFormMode.View}
            initialValue={data?.comment}
            component={FormattedTextAreaField}
          />
        }

        <Field
          label='Number of items'
          name='numberOfItems'
          readOnly={mode === ItemFormMode.View}
          initialValue={data?.actualCount}
          min={mode === ItemFormMode.Create ? 1 : 0}
          max={data?.initialCount}
          onChange={numberOfItemsChangeHandler}
          component={SpinnerField}
        />
      </Form.Group>

      <Form.Group widths='two'>
        <Field
          label='Customer'
          name='customer'
          readOnly={mode !== ItemFormMode.Create}
          options={customersOptions}
          selection={customer}
          onSearchChange={customersSearchHandler}
          onResultSelect={customerResultSelectHandler}
          component={SearchField}
        />
        <Field
          label='Project'
          name='project'
          readOnly={mode !== ItemFormMode.Create}
          validator={isProjectSearchQueryValidator}
          options={projectsOptions}
          selection={project}
          onSearchChange={projectsSearchHandler}
          onResultSelect={projectResultSelectHandler}
          component={SearchField}
        />
      </Form.Group>
      {children}
    </Form>
  );
};

export default PackageForm;
