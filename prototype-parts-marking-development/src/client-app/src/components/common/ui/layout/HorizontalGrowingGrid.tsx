import React from 'react';

const HorizontalGrowingGrid: React.FC = ({
  children,
}) =>
{
  return (
      <div style={{
        width: '100%',
        height: '100%',
        display: 'flex',
        flexWrap: 'wrap',
        flexDirection: 'column',
      }}>
        {children}
      </div>
  );
};

export default HorizontalGrowingGrid;
