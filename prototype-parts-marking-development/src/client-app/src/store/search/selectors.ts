import { IApplicationState } from '../index';

export function selectSearchStarted(state: IApplicationState)
{
  const isLoading =
    state.search.packages.loading ||
    state.search.prototypes.loading ||
    state.search.variants.loading;
  const hasAnyResults =
    state.search.packages.results ||
    state.search.prototypes.results ||
    state.search.variants.results;

  return isLoading && !hasAnyResults;
}

export function selectIsSearching(state: IApplicationState)
{
  const isSearchingPackages = state.search.packages.loading && !state.search.packages.results;
  const isSearchingPrototypes = state.search.prototypes.loading && !state.search.prototypes.results;
  const isSearchingVariants = state.search.variants.loading && !state.search.variants.results;

  return isSearchingPackages || isSearchingPrototypes || isSearchingVariants;
}
