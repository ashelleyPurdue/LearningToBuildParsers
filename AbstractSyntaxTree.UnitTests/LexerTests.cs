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
      var actualTypes = RunLexer(src)
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
      IToken[] tokens = RunLexer(src);
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

    [Theory]
    [InlineData("hello world", @"""hello world""")]

    // Things should be escapable with backslashes
    [InlineData(" \n \" \r \\ ", @""" \n \"" \r \\ """)]

    // If there's a token inside the string, it should be ignored.  EVERYTHING
    // until the closing quote must be considered part of the string
    [InlineData("{}() 123-_*.", @"""{}() 123-_*.""")] 
    public void Text_Surrounded_By_Quotes_Becomes_A_String(string expectedStr, string src)
    {
      IToken token = RunLexer(src)[0];
      Assert.IsType<StringToken>(token);

      var strTok = (StringToken)token;
      Assert.Equal(expectedStr, strTok.Content);
    }
    
    [Theory]
    [InlineData(0, 0, 0, "lineZero")]
    [InlineData(0, 1, 0, "\nlineOne")]
    [InlineData(0, 2, 0, "\n\nlineTwo")]
    [InlineData(1, 2, 0, "\nlineOne\nlineTwo")]
    [InlineData(1, 0, 8, "wordOne wordTwo")]
    public void It_Properly_Keeps_Track_Of_The_Line_And_Char_Nums(
      int tokenNumber,
      int expectedLine,
      int expectedChar,
      string src
    )
    {
      IToken token = RunLexer(src)[tokenNumber];
      Assert.Equal(expectedLine, token.Position.LineNumber);
      Assert.Equal(expectedChar, token.Position.CharNumber);
    }
  
    private IToken[] RunLexer(string src)
    {
      var lexer = new Lexer();
      return lexer
        .ToTokens(src)
        .ToArray();
    }
  }
}
