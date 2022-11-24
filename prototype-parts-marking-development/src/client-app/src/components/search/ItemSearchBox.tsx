import React, { CSSProperties, ChangeEvent, SyntheticEvent, useCallback, useEffect, useState } from 'react';
import { Button, Dropdown, DropdownProps, Segment } from 'semantic-ui-react';

import { useLocalStorage } from '../../hooks/useLocalStorage';
import { createPackageFilterFromPartCode } from '../../models/api/items/package/packageFilter';
import { createPrototypeFilterFromPartCode } from '../../models/api/items/part/prototypeFilter';
import IPartCode from '../../models/partCode';
import { ISearchFilters } from '../../store/search/types';
import BorderlessInput from '../common/ui/input/BorderlessInput';
import IconDropdown from '../common/ui/input/IconDropdown';
import PartCodeInput from '../common/ui/input/PartCodeInput';
import styles from './ItemSearchBox.module.css';
import
{
  CommentSearchTarget,
  PartCodeSearchTarget,
  SearchKind,
  SearchKindOptions,
  SearchTargetOptionsByKind,
} from './itemSearchBoxUtilities';

interface IItemSearchBoxProps
{
  style?: CSSProperties;
  onSearch: (filters: ISearchFilters) => void;
}

const ItemSearchBox: React.FC<IItemSearchBoxProps> = ({
  style,
  onSearch,
}) =>
{
  const defaultSearchKind = SearchKind.PartCode;

  const [searchKind, setSearchKind] = useLocalStorage('search.kind', defaultSearchKind);
  const searchTargetOptions = SearchTargetOptionsByKind[searchKind];
  const defaultSearchTarget = (searchTargetOptions && searchTargetOptions[0]?.value?.toString()) ?? '';
  const [searchTarget, setSearchTarget] = useLocalStorage('search.target', defaultSearchTarget);
  const [partCode, setPartCode] = useState<IPartCode>();
  const [commentText, setCommentText] = useState('');

  // reset search target when not existent for current search kind
  useEffect(() =>
  {
    if (searchTargetOptions.every(option => option.value !== searchTarget))
    {
      setSearchTarget(searchTargetOptions[0]?.value?.toString() ?? '');
    }
  }, [searchKind, searchTarget, searchTargetOptions, setSearchTarget]);

  const searchKindChangeHandler = useCallback((_event: SyntheticEvent, data: DropdownProps) =>
  {
    setSearchKind(data.value?.toString() ?? '');
  }, [setSearchKind]);

  const searchTargetChangeHandler = useCallback((_event: SyntheticEvent, data: DropdownProps) =>
  {
    setSearchTarget(data.value?.toString() ?? '');
  }, [setSearchTarget]);

  const commentTextChangeHandler = useCallback((event: ChangeEvent<HTMLInputElement>) =>
  {
    setCommentText(event.currentTarget.value);
  }, []);

  const partCodeSearchHandler = useCallback(() =>
  {
    if (!partCode) return;

    const filters: ISearchFilters = { };
    if (searchTarget === PartCodeSearchTarget.Packages ||
        searchTarget === PartCodeSearchTarget.All)
    {
      filters.packages = createPackageFilterFromPartCode(partCode);
    }

    if (searchTarget === PartCodeSearchTarget.Parts ||
        searchTarget === PartCodeSearchTarget.All)
    {
      filters.prototypes = createPrototypeFilterFromPartCode(partCode);
    }

    onSearch(filters);
  }, [searchTarget, partCode, onSearch]);

  const commentTextSearchHandler = useCallback(() =>
  {
    if (!commentText) return;

    const filters: ISearchFilters = { };
    if (searchTarget === CommentSearchTarget.Packages ||
        searchTarget === CommentSearchTarget.All)
    {
      filters.packages = { search: commentText };
    }

    if (searchTarget === CommentSearchTarget.Parts ||
        searchTarget === CommentSearchTarget.PartsAndVariants ||
        searchTarget === CommentSearchTarget.All)
    {
      filters.prototypes = { search: commentText };
    }

    if (searchTarget === CommentSearchTarget.Variants ||
      searchTarget === CommentSearchTarget.PartsAndVariants ||
      searchTarget === CommentSearchTarget.All)
    {
      filters.variants = { search: commentText };
    }

    onSearch(filters);
  }, [searchTarget, commentText, onSearch]);

  const searchButtonClickHandler = useCallback(() =>
  {
    switch (searchKind)
    {
      case SearchKind.PartCode:
        return partCodeSearchHandler();
      case SearchKind.Comment:
        return commentTextSearchHandler();
    }
  }, [searchKind, commentTextSearchHandler, partCodeSearchHandler]);

  const renderSearchInput = () =>
  {
    switch (searchKind)
    {
      case SearchKind.PartCode:
        return (
          <PartCodeInput
            className={styles.SearchBoxSegment}
            onChange={setPartCode}
            onSubmit={partCodeSearchHandler}
          />);
      case SearchKind.Comment:
        return (
          <Segment className={styles.SearchBoxSegment}>
            <BorderlessInput
              onChange={commentTextChangeHandler}
              onSubmit={commentTextSearchHandler}
            />
          </Segment>
        );
      default:
        return (
          <Segment className={styles.SearchBoxSegment}>
            <BorderlessInput
              disabled
              value='Select search type'
            />
          </Segment>);
    }
  };

  return (
    <Segment.Group compact horizontal style={{ padding: '0.15em 0 0.15em 0', ...style }}>
      <Segment className={styles.SearchBoxSegment}>
        <IconDropdown
          compact
          title='Click to select search type'
          options={SearchKindOptions}
          value={searchKind}
          onChange={searchKindChangeHandler}
        />
      </Segment>

      <Segment className={styles.SearchBoxSegment}>
        <Dropdown
          icon='dropdown'
          title='CLick to select search scope'
          options={searchTargetOptions}
          value={searchTarget}
          onChange={searchTargetChangeHandler}
        />
      </Segment>

      {renderSearchInput()}

      <Segment className={styles.SearchBoxSegment}>
        <Button
          icon='search'
          className={styles.SearchButton}
          onClick={searchButtonClickHandler}
        />
      </Segment>
    </Segment.Group>
  );
};

export default ItemSearchBox;
