using System;
using System.Collections.Generic;

namespace AbstractSyntaxTree
{
  public class FunctionDefinition
  {
    public string Name { get; set; }
    public List<IStatement> Statements { get; set; }
    // TODO: Parameters
    // TODO: Return type
  }
}