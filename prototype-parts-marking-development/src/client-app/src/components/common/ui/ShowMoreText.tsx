import React, { useState } from 'react';

import styles from './ShowMoreText.module.css';

interface IShowMoreTextProps
{
  text: string | undefined | null;
  limit?: number;
  more?: string;
  less?: string;
}

const ShowMoreText: React.FC<IShowMoreTextProps> = ({
  text,
  limit = Number.MAX_SAFE_INTEGER,
  more = 'more',
  less = 'less',
}) =>
{
  const [isCollapsed, setCollapsed] = useState(!!text && text.length > limit);

  if (!text) return <span />;
  if (text.length <= limit) return <span>{text}</span>;

  return (
    <span>
      {isCollapsed ? text.substr(0, limit) : text}
      <span style={{ display: isCollapsed ? 'inline' : 'none' }}>...</span>
      &nbsp;
      <button className={styles.LinkButton} onClick={() => setCollapsed(prevCollapsed => !prevCollapsed)}>
        {isCollapsed ? more : less}
      </button>
      <a href='/' style={{ display: 'none' }}>a</a>
    </span>
  );
};

export default ShowMoreText;
