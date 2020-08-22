using System;
using System.Collections.Generic;

namespace AbstractSyntaxTree
{
  public class ClassDefinition
  {
    public string Name { get; set; }
    public List<FunctionDefinition> Functions { get; set; }
  }
}
