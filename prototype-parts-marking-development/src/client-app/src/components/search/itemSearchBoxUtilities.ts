import { DropdownItemProps } from 'semantic-ui-react';

import { IIconDropdownItemProps } from '../common/ui/input/IconDropdown';

type SearchKindType =
{
  readonly PartCode: string;
  readonly Comment: string;
};

type PartCodeSearchTargetType =
{
  readonly All: string;
  readonly Parts: string;
  readonly Packages: string;
};

type CommentSearchTargetType =
{
  readonly All: string;
  readonly Parts: string;
  readonly Variants: string;
  readonly PartsAndVariants: string;
  readonly Packages: string;
};

export const SearchKind: SearchKindType =
{
  PartCode: 'Part Code',
  Comment: 'Comment',
};

export const PartCodeSearchTarget: PartCodeSearchTargetType =
{
  All: 'All',
  Parts: 'Parts',
  Packages: 'Packages',
};

export const CommentSearchTarget: CommentSearchTargetType =
{
  All: 'All',
  Parts: 'Parts',
  Variants: 'Variants',
  PartsAndVariants: 'Parts & Variants',
  Packages: 'Packages',
};

export const SearchKindOptions: IIconDropdownItemProps[] =
[
  { icon: 'barcode', text: SearchKind.PartCode, value: SearchKind.PartCode },
  { icon: 'comment outline', text: SearchKind.Comment, value: SearchKind.Comment },
];

export const PartCodeSearchTargetOptions: DropdownItemProps[] =
[
  { text: PartCodeSearchTarget.All, value: PartCodeSearchTarget.All },
  { text: PartCodeSearchTarget.Parts, value: PartCodeSearchTarget.Parts },
  { text: PartCodeSearchTarget.Packages, value: PartCodeSearchTarget.Packages },
];

export const CommentSearchTargetOptions: DropdownItemProps[] =
[
  { text: CommentSearchTarget.All, value: CommentSearchTarget.All },
  { text: CommentSearchTarget.Parts, value: CommentSearchTarget.Parts },
  { text: CommentSearchTarget.Variants, value: CommentSearchTarget.Variants },
  { text: CommentSearchTarget.PartsAndVariants, value: CommentSearchTarget.PartsAndVariants },
  { text: CommentSearchTarget.Packages, value: CommentSearchTarget.Packages },
];

export const SearchTargetOptionsByKind: {[index: string]: DropdownItemProps[]} =
{
  'Part Code': PartCodeSearchTargetOptions,
  Comment: CommentSearchTargetOptions,
};
