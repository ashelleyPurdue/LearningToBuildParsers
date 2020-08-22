using System;

// TODO: There's some copy-pasta in here.  Refactor somehow.
namespace AbstractSyntaxTree
{
  public enum TokenType
  {
    Word,
    Keyword,
    String,
    Number,
    Symbol,
  }
  
  public class Token
  {
    public CodePos Position { get; private set; }
    public TokenType Type { get; private set; }
    public string Content { get; private set; }

    public Token(CodePos position, TokenType type, string content)
    {
      Position = position;
      Type = type;
      Content = content;
    }

    public bool IsSymbol(string content)
    {
      return Type == TokenType.Symbol && Content == content;
    }

    public bool IsKeyword(string content)
    {
      return Type == TokenType.Keyword && Content == content;
    }
  }
}