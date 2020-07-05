using System;

namespace AbstractSyntaxTree
{
  public interface IToken { }

  public class WordToken : IToken
  {
    public string Content { get; set; }
    public override string ToString() => Content;
  }

  public class KeywordToken : WordToken { }

  public class OpenCurlyToken : IToken
  {
    public override string ToString() => "{";
  }

  public class CloseCurlyToken : IToken
  {
    public override string ToString() => "}";
  }
}