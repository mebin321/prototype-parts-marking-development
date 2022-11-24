type UserActiveStateType =
{
  readonly All: string;
  readonly Active: string;
  readonly Deleted: string;
};

export const UserActiveState: UserActiveStateType =
{
  All: 'All',
  Active: 'Active',
  Deleted: 'Deleted',
};

export const UserActiveStateOptions = Object.getOwnPropertyNames(UserActiveState).map(item =>
{
  return { key: item, text: item, value: item };
});

export function convertUserActiveStateToTristateBoolean(value: string)
{
  switch (value)
  {
    case UserActiveState.Active:
      return true;
    case UserActiveState.Deleted:
      return false;
    default:
      return undefined;
  }
}
