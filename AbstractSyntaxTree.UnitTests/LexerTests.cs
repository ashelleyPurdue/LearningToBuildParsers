using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;

namespace AbstractSyntaxTree.UnitTests
{
  public class LexerTests
  {
    private void AssertTokenTypes(string src, params TokenType[] expectedTypes)
    {
      var lexer = new Lexer();
      var actualTypes = RunLexer(src)
        .Select(t => t.Type)
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
      Token[] tokens = RunLexer(src);
      Assert.Equal(expectedTokens.Length, tokens.Length);
      
      for (int i = 0; i < expectedTokens.Length; i++)
      {
        var word = tokens[i];
        Assert.Equal(TokenType.Word, word.Type);
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

      var expectedTokens = new (TokenType type, string content)[]
      {
        (TokenType.Word, "foo"),
        (TokenType.Keyword, "kFoo"),
        (TokenType.Keyword, "kBar"),
        (TokenType.Word, "bar")
      };

      // Generate tokens from the src
      var lexer = new Lexer(keywords);
      Token[] tokens = lexer
        .ToTokens(src)
        .ToArray();

      // Assert that the actual tokens match the expected
      // ones.
      Assert.Equal(expectedTokens.Length, tokens.Length);

      for(int i = 0; i < expectedTokens.Length; i++)
      {
        var actual = tokens[i];
        var expected = expectedTokens[i];

        Assert.Equal(expected.type, actual.Type);
        Assert.Equal(expected.content, actual.Content);
      }
    }

    [Fact]
    public void It_Can_Recognize_Open_And_Close_Curly_Brackets()
    {
      var tokens = RunLexer(" { } foo ");
      new TokenExpecter(tokens)
        .FollowedBy(TokenType.Symbol, "{")
        .FollowedBy(TokenType.Symbol, "}")
        .FollowedBy(TokenType.Word, "foo")
        .AndNoOthers();
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("1.23")]
    [InlineData("-1.2")]
    [InlineData("-1")]
    public void It_Can_Recognize_Numbers(string src)
    {
      var tokens = RunLexer(src);
      new TokenExpecter(tokens)
        .FollowedBy(TokenType.Number)
        .AndNoOthers();
    }

    [Fact]
    public void It_Can_Disambiguate_Between_Subtraction_And_Negative_Numbers()
    {

      new TokenExpecter(RunLexer("-"))
        .FollowedBy(TokenType.Symbol, "-")
        .AndNoOthers();

      new TokenExpecter(RunLexer("-32"))
        .FollowedBy(TokenType.Number, "-32")
        .AndNoOthers();

      new TokenExpecter(RunLexer("32 - 32"))
        .FollowedBy(TokenType.Number, "32")
        .FollowedBy(TokenType.Symbol, "-")
        .FollowedBy(TokenType.Number, "32")
        .AndNoOthers();

      new TokenExpecter(RunLexer("32-32"))
        .FollowedBy(TokenType.Number, "32")
        .FollowedBy(TokenType.Symbol, "-")
        .FollowedBy(TokenType.Number, "32")
        .AndNoOthers();

      new TokenExpecter(RunLexer("32 - -32"))
        .FollowedBy(TokenType.Number, "32")
        .FollowedBy(TokenType.Symbol, "-")
        .FollowedBy(TokenType.Number, "-32")
        .AndNoOthers();

      new TokenExpecter(RunLexer("foo-32"))
        .FollowedBy(TokenType.Word, "foo")
        .FollowedBy(TokenType.Symbol, "-")
        .FollowedBy(TokenType.Number, "32")
        .AndNoOthers();

      new TokenExpecter(RunLexer("32-foo-32 - -32"))
        .FollowedBy(TokenType.Number, "32")
        .FollowedBy(TokenType.Symbol, "-")
        .FollowedBy(TokenType.Word, "foo")
        .FollowedBy(TokenType.Symbol, "-")
        .FollowedBy(TokenType.Number, "32")
        .FollowedBy(TokenType.Symbol, "-")
        .FollowedBy(TokenType.Number, "-32")
        .AndNoOthers();
    }

    [Fact]
    public void Words_Can_Have_Numbers_In_The_Middle_But_Not_At_The_Start()
    {
      string src = "123foo123";
      var tokens = RunLexer(src);
      new TokenExpecter(tokens)
        .FollowedBy(TokenType.Number, "123")
        .FollowedBy(TokenType.Word, "foo123")
        .AndNoOthers();
    }

    [Fact]
    public void Words_And_Curly_Brackets_Dont_Need_Whitespace_Between()
    {
      var tokens = RunLexer("{}fo{o");
      new TokenExpecter(tokens)
        .FollowedBy(TokenType.Symbol, "{")
        .FollowedBy(TokenType.Symbol, "}")
        .FollowedBy(TokenType.Word, "fo")
        .FollowedBy(TokenType.Symbol, "{")
        .FollowedBy(TokenType.Word, "o")
        .AndNoOthers();
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
      Token token = RunLexer(src)[0];
      Assert.Equal(expectedStr, token.Content);
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
      Token token = RunLexer(src)[tokenNumber];
      Assert.Equal(expectedLine, token.Position.LineNumber);
      Assert.Equal(expectedChar, token.Position.CharNumber);
    }
  
    private Token[] RunLexer(string src)
    {
      var lexer = new Lexer();
      return lexer
        .ToTokens(src)
        .ToArray();
    }
  }
}
