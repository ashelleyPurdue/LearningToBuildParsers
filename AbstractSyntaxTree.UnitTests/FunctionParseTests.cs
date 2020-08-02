using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AbstractSyntaxTree.UnitTests
{
  public class FunctionParseTests
  {

    [Fact]
    public void It_Can_Parse_An_Empty_Function()
    {
      var func = ParseSingleFunction("function DoThing() { }");
      Assert.Equal("DoThing", func.Name);
      Assert.Empty(func.Statements);
    }

    [Fact]
    public void It_Can_Parse_Simple_Variable_Decls_In_A_Function()
    {
      const string src =
      @"
        function DoThing() 
        {
          let fooVar;
          let barVar;
        }
      ";

      string[] expectedVarNames = new[]
      {
        "fooVar",
        "barVar"
      };

      var func = ParseSingleFunction(src);

      // The order of the statements IS important this time around.
      for (int i = 0; i < expectedVarNames.Length; i++)
      {
        var statement = func.Statements[i];
        var expectedName = expectedVarNames[i];

        Assert.IsAssignableFrom<LetStatement>(statement);
        var letStatement = (LetStatement)statement;

        Assert.Equal(expectedName, letStatement.Name);
      }
    }

    /// <summary>
    /// Accepts a source code fragement representing a function:
    /// "function FuncName([params here]) { [statements here] }".
    /// Returns the function node that would be parsed from that.
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    private FunctionDefinition ParseSingleFunction(string src)
    {
      var p = new Parser();
      string biggerSrc =
      $@"
        class ThingDoer
        {{
          {src}
        }}
      ";

      AstRoot root = p.Parse(biggerSrc);
      Assert.Single(root.Classes);

      var funcs = root.Classes[0].Functions;
      Assert.Single(funcs);

      return funcs[0];
    }
  }
}
