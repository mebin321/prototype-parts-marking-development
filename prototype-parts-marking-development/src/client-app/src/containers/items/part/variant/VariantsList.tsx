import _ from 'lodash';
import React, { Fragment } from 'react';
import { Button, Feed, Header, Loader, Message, Segment } from 'semantic-ui-react';

import ShowMoreText from '../../../../components/common/ui/ShowMoreText';
import { IPrototypeVariant } from '../../../../models/api/items/part/variant/prototypeVariant';
import { IPrototypeVariantsResponse } from '../../../../models/api/responses';

const CommentMaxLength = 200;

interface IVariantsListProps
{
  currentVariant?: IPrototypeVariant;
  recentVariants?: IPrototypeVariant[];
  pages?: IPrototypeVariantsResponse[];
  error?: string;
  isLoading?: boolean;
  onAdditionalVariantsRequested?: (page: number) => void;
}

const VariantsList: React.FC<IVariantsListProps> = ({
  currentVariant,
  recentVariants,
  pages,
  isLoading,
  error,
  onAdditionalVariantsRequested,
}) =>
{
  const lastPagePagination = _.last(pages)?.pagination;
  const hasMorePages =
    currentVariant
      // there exists at least one variant of the part
      ? lastPagePagination
        // last loaded page is the last existing page on the server
        ? lastPagePagination.page !== lastPagePagination.totalPages
        // no page is loaded - more variants exist if
        // current variant is higher than 1 and
        // not all existing variants are in recent variants (current variant + created variants without page refresh)
        : currentVariant.version > 1 && _.last(recentVariants)?.version !== 1
      // no current variant means that there are no variants of the part - there are no more pages
      : false;

  const nextPage = lastPagePagination ? lastPagePagination.page + 1 : 1;

  const variants: IPrototypeVariant[] = [];
  if (currentVariant) variants.push(currentVariant);
  if (recentVariants) variants.push(..._.differenceBy(recentVariants, variants, variant => variant.id));
  if (pages) variants.push(..._.differenceBy(pages.flatMap(page => page.items), variants, variant => variant.id));

  return (
    <Fragment>
    <Feed>
      {variants.map(variant => (
        <Feed.Event key={variant.id}>
          <Feed.Content>
            <Segment>
              <Feed.Summary>
                <Header size='medium' style={{ display: 'inline' }}>v{variant.version}&nbsp;&nbsp;</Header>
                <Feed.Meta>by {variant.createdBy.name} ({variant.createdBy.username})</Feed.Meta>
                <Feed.Date>at {variant.createdAt.toLocaleString()}</Feed.Date>
              </Feed.Summary>
              <Feed.Extra>
                <ShowMoreText text={variant.comment} limit={CommentMaxLength} />
              </Feed.Extra>
            </Segment>
          </Feed.Content>
        </Feed.Event>
      ))}
      {error && !isLoading &&
        <Feed.Event>
          <Message error style={{ width: '100%' }}>{error}</Message>
        </Feed.Event>
      }
      {isLoading &&
        <Feed.Event>
          <div style={{ width: '100%', padding: '1em' }}>
            <Loader active={true} style={{ position: 'relative', translateY: '50%' }} />
          </div>
        </Feed.Event>
      }
    </Feed>
    {hasMorePages &&
      <div style={{ display: 'flex', justifyContent: 'center' }}>
        <Button
          basic
          content='Show older'
          disabled={isLoading}
          onClick={() => { if (onAdditionalVariantsRequested) onAdditionalVariantsRequested(nextPage); }}
        />
      </div>
    }
    </Fragment>
  );
};

export default VariantsList;
