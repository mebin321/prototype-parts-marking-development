import React, { SyntheticEvent, useCallback, useEffect, useMemo, useState } from 'react';
import { Field } from 'react-final-form';
import
{
  Form,
  SearchResultData,
} from 'semantic-ui-react';

import agent from '../../../../api/agent';
import { GateLevelsEnum, LocationsEnum, OutletsEnum, ProductGroupsEnum } from '../../../../api/enumerations';
import SearchField from '../../../../components/common/form/SearchField';
import { useLocalStorage } from '../../../../hooks/useLocalStorage';
import { EmptyTextualEnumItem } from '../../../../models/api/enumerations';
import { IGateLevel } from '../../../../models/api/enumerations/gateLevels';
import { ILocation } from '../../../../models/api/enumerations/locations';
import { IOutlet } from '../../../../models/api/enumerations/outlets';
import { IProductGroup } from '../../../../models/api/enumerations/productGroups';
import { IPrototypeSet } from '../../../../models/api/items/part/set/prototypeSet';
import { IViewableCustomer } from '../../../../models/api/projects/viewableCustomer';
import { IViewableProject } from '../../../../models/api/projects/viewableProject';
import IPartCode from '../../../../models/partCode';
import { updateObject } from '../../../../store/utilities';
import { isProjectSearchQueryValidator } from '../../../../utilities/validation/validators';
import
{
  ItemFormMode,
  generateCustomersSearchHandler,
  generateEnumItemResultSelectHandler,
  generateEnumItemSearchHandler,
  generateOutletResultSelectHandler,
  generateProductGroupSearchHandler,
  generateProjectsSearchHandler,
  getInitialGateLevel,
  getInitialLocation,
  getInitialOutlet,
  getInitialProductGroup,
} from '../../itemsUtilities';

interface IPrototypeSetFormProps
{
  mode: ItemFormMode;
  data?: IPrototypeSet;
  loading?: boolean;
  onChange?: (partCode: Partial<IPartCode>) => void;
  onSubmit?: (event: SyntheticEvent) => void;
}

const PrototypeSetForm: React.FC<IPrototypeSetFormProps> = ({
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
  const [storedLocation, setStoredLocation] = useLocalStorage<ILocation>(LocationsEnum);
  const [storedGateLevel, setStoredGateLevel] = useLocalStorage<IGateLevel>(GateLevelsEnum);

  const [project, setProject] = useState<IViewableProject>();
  const [customer, setCustomer] = useState<IViewableCustomer>();
  const [projectsOptions, setProjectOptions] = useState<IViewableProject[]>();
  const [customersOptions, setCustomersOptions] = useState<IViewableCustomer[]>();

  const [outletsFiltered, setOutletsFiltered] = useState<IOutlet[]>([]);
  const [productGroupsFiltered, setProductGroupsFiltered] = useState<IProductGroup[]>([]);
  const [locationsFiltered, setLocationsFiltered] = useState<ILocation[]>([]);
  const [gateLevelsFiltered, setGateLevelsFiltered] = useState<IGateLevel[]>([]);
  const [partCode, setPartCode] = useState<Partial<IPartCode>>({ numberOfPrototypes: 0 });

  const outletsSearchHandler = useMemo(() =>
    generateEnumItemSearchHandler(agent.Enumerations.Outlets, setOutletsFiltered), []);
  const productGroupsSearchHandler = useMemo(() =>
    generateProductGroupSearchHandler(storedOutlet?.moniker ?? '', setProductGroupsFiltered), [storedOutlet?.moniker]);
  const locationsSearchHandler = useMemo(() =>
    generateEnumItemSearchHandler(agent.Enumerations.Locations, setLocationsFiltered), []);
  const gateLevelsSearchHandler = useMemo(() =>
    generateEnumItemSearchHandler(agent.Enumerations.GateLevels, setGateLevelsFiltered), []);
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
    return generateEnumItemResultSelectHandler<IProductGroup>(
      value => setPartCode(prevPartCode => updateObject(prevPartCode, { productGroup: value })),
      setStoredProductGroup
    );
  }, [setStoredProductGroup]);
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

  const initialOutlet =
    getInitialOutlet(mode, data?.outletTitle, data?.outletCode, storedOutlet);
  const initialProductGroup =
    getInitialProductGroup(mode, data?.productGroupTitle, data?.productGroupCode, storedProductGroup);
  const initialLocation =
    getInitialLocation(mode, data?.locationTitle, data?.locationCode, storedLocation);
  const initialGateLevel =
    getInitialGateLevel(mode, data?.gateLevelTitle, data?.gateLevelCode, storedGateLevel);

  // initialize part code from existing prototype set field values
  useEffect(() =>
  {
    const partCodeFromData: Partial<IPartCode> =
    {
      outlet: data?.outletCode,
      productGroup: data?.productGroupCode,
      evidenceYear: data?.evidenceYearCode,
      location: data?.locationCode,
      uniqueIdentifier: data?.setIdentifier,
      gateLevel: data?.gateLevelCode,
    };

    setPartCode(prevPartCode => updateObject(prevPartCode, partCodeFromData));
  }, [data]);

  useEffect(() =>
  {
    const initialPartCode: Partial<IPartCode> =
    {
      outlet: initialOutlet?.code,
      productGroup: initialProductGroup?.code,
      location: initialLocation?.code,
      gateLevel: initialGateLevel?.code,
    };

    setPartCode(prevPartCode => updateObject(prevPartCode, initialPartCode));
    // no dependencies array is given to execute update from initial values only on component mount
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
    }

    if (!storedOutlet)
    {
      setStoredProductGroup(EmptyTextualEnumItem);
    }
    // StoredProductGroup is omitted from dependencies array to not run this effect on that change
    // to prevent infinite loop because storedProductGroup setter is in executing area
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [storedOutlet, productGroupsFiltered]);

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
          label='Location'
          name='location'
          readOnly={mode !== ItemFormMode.Create}
          options={locationsFiltered}
          defaultSelection={initialLocation}
          onSearchChange={locationsSearchHandler}
          onResultSelect={locationResultSelectHandler}
          component={SearchField}
        />
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

export default PrototypeSetForm;
