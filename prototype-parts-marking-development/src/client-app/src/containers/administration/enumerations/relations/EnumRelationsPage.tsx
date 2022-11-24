import React, { SyntheticEvent, useCallback } from 'react';
import { Route, RouteComponentProps, Switch } from 'react-router-dom';
import { Container, Dropdown, DropdownProps, Header } from 'semantic-ui-react';

import { parseChildRoute } from '../../../../utilities/routing';
import OutletProductGroups from './OutletProductGroups';
import PartComponents from './PartComponents';
import ProductGroupParts from './ProductGroupParts';

interface IEnumRelationsPageProps extends RouteComponentProps
{
}

const EnumRelationsPage: React.FC<IEnumRelationsPageProps> = ({
  history,
  location,
  match,
}) =>
{
  const selectedRelationDropdownChangeHandler = useCallback((_event: SyntheticEvent, data: DropdownProps) =>
  {
    const value = data.value ? String(data.value) : '';
    if (!value) return;

    history.replace(`${match.url}/${value}`);
  }, [history, match]);

  return (
    <Container>
      <Header as='h2'>
        <Header.Content>
          Relations:&nbsp;
          <Dropdown
            inline
            placeholder='select any'
            selectOnBlur={false}
            selectOnNavigation={false}
            onChange={selectedRelationDropdownChangeHandler}
            options={[
              { text: 'Outlet Product Groups', value: 'outlet-productgroups' },
              { text: 'Product Group Parts', value: 'productgroup-parts' },
              { text: 'Part Components', value: 'part-components' },
            ]}
            value={parseChildRoute(location.pathname, match.url)}
            style={{ zIndex: 3000 }} // z-index must be greater than EnumRelationsTable
          />
        </Header.Content>
      </Header>

      <Container style={{ marginTop: '2em' }}>
        <Switch>
          <Route path={match.url + '/outlet-productgroups'} exact component={OutletProductGroups} />
          <Route path={match.url + '/productgroup-parts'} exact component={ProductGroupParts} />
          <Route path={match.url + '/part-components'} exact component={PartComponents} />
        </Switch>
      </Container>
    </Container>
  );
};

export default EnumRelationsPage;
