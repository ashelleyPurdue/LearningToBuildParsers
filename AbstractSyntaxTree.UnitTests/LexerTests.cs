using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace AbstractSyntaxTree.UnitTests
{
  public class LexerTests
  {
    private void AssertTokenTypes(string src, params Type[] expectedTypes)
    {
      var lexer = new Lexer();
      var actualTypes = lexer
        .ToTokens(src)
        .Select(t => t.GetType())
        .ToArray();

      Assert.Equal(expectedTypes, actualTypes);
    }

    [Theory]
    [InlineData("foo", "foo")]
    [InlineData(" foo", "foo")]
    [InlineData("foo ", "foo")]
    [InlineData("  foo", "foo")]
    [InlineData("foo \nbar", "foo", "bar")]
    public void It_Can_Parse_Words_And_Ignore_Whitespace(string src, params string[] expectedTokens)
    {
      var lexer = new Lexer();
      IToken[] tokens = lexer
        .ToTokens(src)
        .ToArray();

      Assert.Equal(expectedTokens.Length, tokens.Length);

      for (int i = 0; i < expectedTokens.Length; i++)
      {
        var word = (WordToken)tokens[i];
        Assert.Equal(expectedTokens[i], word.Content);
      }
    }

    [Fact]
    public void Keywords_Are_Recognized_Separately_From_Normal_Words()
    {
      var keywords = new HashSet<string>
      {
        "kFoo",
        "kBar"
      };
      string src = "foo kFoo kBar bar";

      var expectedTokens = new (Type type, string content)[]
      {
        (typeof(WordToken), "foo"),
        (typeof(KeywordToken), "kFoo"),
        (typeof(KeywordToken), "kBar"),
        (typeof(WordToken), "bar")
      };

      // Generate tokens from the src
      var lexer = new Lexer(keywords);
      IToken[] tokens = lexer
        .ToTokens(src)
        .ToArray();

      // Assert that the actual tokens match the expected
      // ones.
      Assert.Equal(expectedTokens.Length, tokens.Length);

      for(int i = 0; i < expectedTokens.Length; i++)
      {
        var actual = (WordToken)tokens[i];
        var expected = expectedTokens[i];

        Assert.Equal(expected.type, actual.GetType());
        Assert.Equal(expected.content, actual.Content);
      }
    }

    [Fact]
    public void It_Can_Recognize_Open_And_Close_Curly_Brackets()
    {
      AssertTokenTypes(" { } foo ",
        typeof(OpenCurlyToken),
        typeof(CloseCurlyToken),
        typeof(WordToken)
      );
    }

    [Fact]
    public void Words_And_Curly_Brackets_Dont_Need_Whitespace_Between()
    {
      AssertTokenTypes("{}fo{o",
        typeof(OpenCurlyToken),
        typeof(CloseCurlyToken),
        typeof(WordToken),
        typeof(OpenCurlyToken),
        typeof(WordToken)
      );
    }

  }
}
