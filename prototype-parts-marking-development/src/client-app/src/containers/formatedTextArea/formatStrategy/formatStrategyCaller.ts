import { ContentBlock, ContentState } from 'draft-js';

import formatStrategy from './formatStrategy';

export const formatStrategyCaller = (
  contentBlock: ContentBlock,
  callbackFun: (start: number, end: number) => void,
  _: ContentState
) =>
{
  const text = contentBlock.getText() as string;
  const index = formatStrategy(text);
  callbackFun(0, index);
};

export default formatStrategyCaller;
