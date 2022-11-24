export class Format
{
  lineLength: number;
  letterBuffer: number;
  lineLettersLeft: number;
  index = 0;
  isNextBlock = false;

  constructor(lineLength: number, rows: number)
  {
    this.letterBuffer = lineLength * rows;
    this.lineLettersLeft = lineLength;
    this.lineLength = lineLength;
  }

  isBufferEmpty = () => this.letterBuffer < 1;
  isSpace = (word: string) => word.length === 0;
  isNewLine = () => this.lineLength === this.lineLettersLeft;
  handleEOL = () => { if (this.isEOL()) this.newLine(); };
  isEOL = () => this.lineLettersLeft === 0;
  newLine = () => { this.lineLettersLeft = this.lineLength; };
  fitsOnLine = (word: string) => word.length <= this.lineLettersLeft;
  isLastLine = () => this.letterBuffer < this.lineLength;

  handleShorterWord(wordLength: number)
  {
    this.letterBuffer -= wordLength + 1;
    this.lineLettersLeft -= wordLength + 1;
    this.index += wordLength + 1;
  }

  handleEqualWord(wordLength: number)
  {
    this.letterBuffer -= wordLength;
    this.lineLettersLeft = this.lineLength;
    this.index += wordLength + 1;
  }

  handleLongerWord(wordLength: number)
  {
    if (this.isNewLine())
    {
      this.letterBuffer -= wordLength + 1;
    }
    else
    {
      this.letterBuffer -= this.lineLettersLeft + wordLength + 1;
    }

    if (this.letterBuffer < 1)
    {
      this.index += this.letterBuffer * -1 < wordLength ? wordLength + this.letterBuffer + 1 : 0;
    }
    else
    {
      this.index += wordLength + 1;
      this.lineLettersLeft = this.lineLength - (wordLength % this.lineLength) - 1;
    }
  }
}
