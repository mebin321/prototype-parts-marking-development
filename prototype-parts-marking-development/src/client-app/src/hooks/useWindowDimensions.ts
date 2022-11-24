import _ from 'lodash';
import { useEffect, useState } from 'react';

function getWindowDimensions()
{
  const { innerWidth: width, innerHeight: height } = window;
  return { width, height };
}

export default function useWindowDimensions()
{
  const [windowDimensions, setWindowDimensions] = useState(getWindowDimensions());

  useEffect(() =>
  {
    const windowResizeHandler = _.throttle(
      () => setWindowDimensions(getWindowDimensions()),
      100,
      { leading: true, trailing: true }
    );

    window.addEventListener('resize', windowResizeHandler);
    return () => window.removeEventListener('resize', windowResizeHandler);
  }, []);

  return windowDimensions;
}
