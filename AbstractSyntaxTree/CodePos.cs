using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public struct CodePos
  {
    public int LineNumber;
    public int CharNumber;

    public CodePos(int lineNumber, int charNumber)
    {
      LineNumber = lineNumber;
      CharNumber = charNumber;
    }
  }
}
