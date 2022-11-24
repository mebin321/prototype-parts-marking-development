import { SemanticICONS } from 'semantic-ui-react';
import { IconSizeProp } from 'semantic-ui-react/dist/commonjs/elements/Icon/Icon';

import { INavigationItemRouteProps } from './NavigationItem';

export interface INavigationItemDefinition extends INavigationItemRouteProps
{
  title?: string;
  description?: string;
  icon?: SemanticICONS;
  iconSize?: IconSizeProp;
  visible?: boolean;
  items?: INavigationItemDefinition[];
}
