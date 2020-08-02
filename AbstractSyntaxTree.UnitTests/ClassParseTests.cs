using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace AbstractSyntaxTree.UnitTests
{
  public class ClassParseTests
  {
    [Fact]
    public void It_Can_Parse_Multiple_Functions_In_A_Class()
    {
      const string src =
      @"
        class ThingDoer
        {
          function DoThing() { }
          function DoOtherThing()
          {
          }
        }
      ";

      string[] expectedFuncNames = new[]
      {
        "DoThing",
        "DoOtherThing"
      };

      var classDef = ParseSingleClass(src);

      // Assert that all of the function names are present, and no others.
      var actualFuncNames = classDef
        .Functions
        .Select(f => f.Name);
      Utils.AssertEqualIgnoringOrder(expectedFuncNames, actualFuncNames);
    }

    /// <summary>
    /// Accepts a source code fragment in the form of
    /// "class ClassName { [members] }"
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    private ClassDefinition ParseSingleClass(string src)
    {
      var p = new Parser();
      var root = p.Parse(src);

      Assert.Single(root.Classes);
      return root.Classes[0];
    }
  }
}
