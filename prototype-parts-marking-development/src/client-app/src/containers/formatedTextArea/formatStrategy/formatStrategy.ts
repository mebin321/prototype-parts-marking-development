import { Format } from './Format';

const LINE_LENGTH = 23;
const ROWS = 3;
export const multipleNewLineRegex = /\n+/g;

const formatStrategy = (text: string) =>
{
  const format = new Format(LINE_LENGTH, ROWS);
  const modifiedText = text.trim().replaceAll(multipleNewLineRegex, ' ');
  const words = modifiedText.split(/ |\n/);

  for (const word of words)
  {
    if (format.isBufferEmpty()) break;
    if (format.isLastLine() && word.length > format.letterBuffer) break;

    if (format.isSpace(word))
    {
      format.handleShorterWord(word.length);
    }
    else if (!format.fitsOnLine(word))
    {
      format.handleLongerWord(word.length);
    }
    else if (word.length === format.lineLettersLeft)
    {
      format.handleEqualWord(word.length);
    }
    else
    {
      format.handleShorterWord(word.length);
    }

    format.handleEOL();
  }

  return format.index;
};

export default formatStrategy;
