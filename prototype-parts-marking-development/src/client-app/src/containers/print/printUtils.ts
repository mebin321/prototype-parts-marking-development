import { IPrintItem } from '../../models/api/print/printItem';
import formatStrategy from '../formatedTextArea/formatStrategy/formatStrategy';
import { ICompletePrintItem } from './PrintPage';

export const transformDescription = (item: IPrintItem): ICompletePrintItem =>
{
  if (item.description)
  {
    let description = item.description.replaceAll(/\n+/g, ' ');
    const index = formatStrategy(description);
    description = description.substring(0, index).trim();
    return { ...item, description: description, fullDescription: item.description };
  }

  return { ...item, fullDescription: item.description };
};
