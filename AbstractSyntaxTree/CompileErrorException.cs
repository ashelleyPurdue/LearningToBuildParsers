using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class CompileErrorException : Exception
  {
    public CodePos Position { get; private set; }

    public CompileErrorException(CodePos pos, string message)
      : base($"Line {pos.LineNumber}, char {pos.CharNumber}: {message}")
    {
      Position = pos;
    }
  }
}
