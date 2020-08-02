using System;
using System.IO;
using System.Linq;
using Xunit;

namespace AbstractSyntaxTree.UnitTests
{
  public class RootParseTests
  {
    [Theory]
    [InlineData(0, 17, "class EmptyClass flabbergast {}")]
    [InlineData(1, 17, "class EmptyClass {}\nclass OtherClass flabbergast {})")]
    public void Unexpected_Word_After_Class_Name_Throws_Compile_Error(
      int expectedLine, 
      int expectedChar,
      string src
    )
    {
      var err = Assert.Throws<CompileErrorException>(() =>
      {
        var p = new Parser();
        p.Parse(src);
      });

      Assert.Equal(expectedLine, err.Position.LineNumber);
      Assert.Equal(expectedChar, err.Position.CharNumber);
    }

    [Fact]
    public void It_Can_Parse_A_Single_Empty_Class()
    {
      var p = new Parser();
      
      AstRoot root = p.Parse("class EmptyClass {}");
      Assert.Single(root.Classes);

      var c = root.Classes[0];
      Assert.Equal("EmptyClass", c.Name);
      Assert.Empty(c.Functions);
    }

    [Fact]
    public void It_Can_Parse_Multiple_Empty_Classes()
    {
      const string src =
      @"
        class FooClass {}
        class BarClass {}
        class FizzClass {}
        class BuzzClass {}
      ";

      var p = new Parser();
      AstRoot root = p.Parse(src);

      // All of the classes must be present, and *only* those classes
      string[] actualNames = root.Classes
        .Select(c => c.Name)
        .ToArray();

      string[] expectedNames = new[]
      {
        "FooClass",
        "BarClass",
        "FizzClass",
        "BuzzClass"
      };

      Utils.AssertEqualIgnoringOrder(expectedNames, actualNames);

      // All the classes must be empty
      foreach (var c in root.Classes)
        Assert.Empty(c.Functions);
    }
  
    [Fact]
    public void It_Can_Parse_Empty_Functions_In_A_Class()
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

      var p = new Parser();
      var root = p.Parse(src);

      Assert.Single(root.Classes);

      var actualFuncs = root
        .Classes[0]
        .Functions;

      // Assert that all of the function names are present, and no others.
      Utils.AssertEqualIgnoringOrder(expectedFuncNames, actualFuncs.Select(f => f.Name));

      // Assert that they're all empty
      foreach (var func in actualFuncs)
        Assert.Empty(func.Statements);
    }

    [Fact]
    public void It_Can_Parse_Simple_Variable_Decls_In_A_Function()
    {
      const string src =
      @"
        class ThingDoer
        {
          function DoThing() 
          {
            let fooVar;
            let barVar;
          }
        }
      ";

      string[] expectedVarNames = new[]
      {
        "fooVar",
        "barVar"
      };

      var p = new Parser();
      var root = p.Parse(src);

      var statements = root
        .Classes[0]
        .Functions[0]
        .Statements;

      // The order of the statements IS important this time around.
      for(int i = 0; i < expectedVarNames.Length; i++)
      {
        var statement = statements[i];
        var expectedName = expectedVarNames[i];

        Assert.IsAssignableFrom<VariableDeclarationStatement>(statement);
        var letStatement = (VariableDeclarationStatement)statement;

        Assert.Equal(expectedName, letStatement.Name);
      }
    }
  }
}
