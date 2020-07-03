using System;
using System.IO;
using System.Linq;
using Xunit;

namespace AbstractSyntaxTree.UnitTests
{
  public class RootParseTests
  {
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

      Assert.Equal(expectedNames.Length, actualNames.Length);

      foreach (string name in expectedNames)
        Assert.Contains(name, actualNames);

      // All the classes must be empty
      foreach (var c in root.Classes)
        Assert.Empty(c.Functions);
    }
  }
}
