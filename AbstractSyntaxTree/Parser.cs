using System;
using System.Linq;
using System.Collections.Generic;

namespace AbstractSyntaxTree
{
  public class Parser
  {
    public AstRoot Parse(IEnumerable<char> src)
    {
      var root = new AstRoot();

      // Parse all the classes
      root.Classes = new List<ClassDefinition>();

      while (src.SkipWhitespace().Any())
      {
        root.Classes.Add(ParseClass(src, out src));
      }
      
      return root;
    }

    private ClassDefinition ParseClass(IEnumerable<char> src, out IEnumerable<char> rest)
    {
      // Look for the class keyword
      src = src
        .SkipWhitespace()
        .ExpectNext("class")
        .SkipWhitespace();

      // Grab the name
      var classDef = new ClassDefinition();
      classDef.Name = src
        .SkipWhitespace()
        .ChopWord(out src)
        .AsString();

      // TODO: Parse the functions.  For now, just enforce them to be empty.
      src = src
        .SkipWhitespace()
        .ExpectNext("{")
        .SkipWhitespace()
        .ExpectNext("}");

      classDef.Functions = new List<FunctionDefinition>();

      rest = src;
      return classDef;
    }
  }
}