using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  /// <summary>
  /// Represents a standalone call to a static function.
  /// A static function is one that does not belong to an object instance.
  /// Because this node is a statement(and not an expression), any return
  /// value from the function is discarded.
  /// </summary>
  public class StaticFunctionCallStatement : IStatement
  {
    public string FunctionName { get; set; }
    // TODO: Parameter values
  }
}
