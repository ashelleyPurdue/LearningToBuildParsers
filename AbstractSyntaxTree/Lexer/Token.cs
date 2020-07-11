using System;

// TODO: There's some copy-pasta in here.  Refactor somehow.
namespace AbstractSyntaxTree
{
  public interface IToken 
  {
    CodePos Position { get; }
  }

  public class StringToken : IToken
  {
    public CodePos Position { get; private set; }
    public string Content { get; private set; }

    public StringToken(CodePos position, string content)
    {
      Position = position;
      Content = content;
    }

    public override string ToString() => Content;
  }

  public class WordToken : IToken
  {
    public CodePos Position { get; private set; }
    public string Content { get; private set; }

    public WordToken(CodePos position, string content)
    {
      Position = position;
      Content = content;
    }

    public override string ToString() => Content;
  }

  public class KeywordToken : WordToken 
  {
    public KeywordToken(CodePos position, string content)
      : base(position, content) { }
  }

  public class OpenCurlyToken : IToken
  {
    public CodePos Position { get; private set; }
    public OpenCurlyToken(CodePos position)
    {
      Position = position;
    }

    public override string ToString() => "{";
  }

  public class CloseCurlyToken : IToken
  {
    public CodePos Position { get; private set; }

    public CloseCurlyToken(CodePos position)
    {
      Position = position;
    }

    public override string ToString() => "}";
  }

  public class OpenParenToken : IToken
  {
    public CodePos Position { get; private set; }

    public OpenParenToken(CodePos position)
    {
      Position = position;
    }

    public override string ToString() => "(";
  }

  public class CloseParenToken : IToken
  {
    public CodePos Position { get; private set; }

    public CloseParenToken(CodePos position)
    {
      Position = position;
    }

    public override string ToString() => ")";
  }
}