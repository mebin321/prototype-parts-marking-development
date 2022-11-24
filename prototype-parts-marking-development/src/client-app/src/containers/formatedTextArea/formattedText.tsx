import React from 'react';

const FormattedText = (props:
{
  offsetKey: any;
  children:
      | boolean
      | React.ReactChild
      | React.ReactFragment
      | React.ReactPortal
      | null
      | undefined;
  }) =>
{
  return (
      <span className='format' data-offset-key={props.offsetKey}>
        {props.children}
      </span>
  );
};

export default FormattedText;
