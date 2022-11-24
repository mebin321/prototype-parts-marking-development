import React from 'react';
import { Dimmer, Loader } from 'semantic-ui-react';

const LoaderComponent: React.FC<{ inverted?: boolean; content?: string }> = ({
  inverted = true,
  content,
}) =>
{
  return (
    <Dimmer active inverted={inverted}>
      <Loader style={{ margin: '100px auto' }} content={content} />
    </Dimmer>
  );
};

export default LoaderComponent;
