import { ToastContent, ToastOptions, toast } from 'react-toastify';

const trailingWhitespacePattern = /\s+$/;
const toastIdForbiddenCharactersPattern = /[^a-z0-9 _!#$%&*+\-,./:;<=>?@]/ig;

export function toastDistinctError(message?: string, error?: string, appendErrorToMessage = true)
{
  const [content, options] = createErrorToast(message, error, appendErrorToMessage);
  toast.error(content, options);
}

function createErrorToast(message?: string, error?: string, appendErrorToMessage = true)
  : [ToastContent, ToastOptions]
{
  let content = message ?? '';
  let toastIdText = content; // fallback if error is empty

  if (!content)
  {
    content = error ?? ''; // fallback if message is empty
  }
  else
  {
    if (error)
    {
      if (appendErrorToMessage)
      {
        if (!trailingWhitespacePattern.test(content)) content += ' ';
        content += error;
      }

      toastIdText = error;
    }
  }

  const toastId = generateToastId(toastIdText);
  const options = toastId ? { toastId: toastId } : {};
  return [content, options];
}

export function generateToastId(content?: string)
{
  if (!content) return '';

  let id = content.replaceAll(' ', '-');
  id = id.replaceAll('\t', ' ');
  id = id.replaceAll('\r\n', ' ');
  id = id.replaceAll('\r', ' ');
  id = id.replaceAll('\n', ' ');
  id = id.replaceAll(toastIdForbiddenCharactersPattern, '');
  id = id.toLowerCase();

  return id;
}
