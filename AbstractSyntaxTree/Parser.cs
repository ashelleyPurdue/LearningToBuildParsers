using System;
using System.Linq;
using System.Collections.Generic;

namespace AbstractSyntaxTree
{
  public class Parser
  {
    private readonly ISet<string> _keywords = new HashSet<string>
    {
      "class",
      "function"
    };

    public AstRoot Parse(IEnumerable<char> src)
    {
      var lexer = new Lexer(_keywords);
      var tokens = new TokenWalker(lexer.ToTokens(src));

      var root = new AstRoot();

      // Parse all the classes
      root.Classes = new List<ClassDefinition>();

      while (!tokens.IsEmpty())
      {
        IToken token = tokens.Peek();

        switch (token)
        {
          case KeywordToken k when k.Content == "class":
            root.Classes.Add(ParseClass(tokens, out tokens));
            break;

          default: throw new CompileErrorException(
            token.Position, 
            $"Unexpected token {token.ToString()}"
          );
        }
      }
      
      return root;
    }

    private ClassDefinition ParseClass(TokenWalker tokens, out TokenWalker rest)
    {
      // Look for the class keyword
      tokens = tokens.ConsumeKeyword("class");
      var classDef = new ClassDefinition();

      // Grab the name
      classDef.Name = tokens
        .Expect<WordToken>()
        .Content;

      tokens = tokens.Consume();

      // Parse the insides of the class
      classDef.Functions = new List<FunctionDefinition>();
      tokens = tokens.Consume<OpenCurlyToken>();

      while (!tokens.IsEmpty())
      {
        IToken token = tokens.Peek();

        switch (token)
        {
          case CloseCurlyToken cc: goto exitClass; // Betcha didn't know C# has "goto".  Mwahahaha!

          case KeywordToken k when k.Content == "function":
            classDef.Functions.Add(ParseFunction(tokens, out tokens));
            break;

          default: throw new CompileErrorException(
            token.Position,
            $"Unexpected token {token.ToString()}"
          );
        }
      }

      exitClass:
      rest = tokens.Consume<CloseCurlyToken>();
      return classDef;
    }

    private FunctionDefinition ParseFunction(TokenWalker tokens, out TokenWalker rest)
    {
      // Look for the function keyword
      tokens = tokens.ConsumeKeyword("function");
      var funcDef = new FunctionDefinition();

      // Grab the name
      funcDef.Name = tokens.Expect<WordToken>().Content;
      tokens = tokens.Consume();

      // TODO: Parse the parameters.  For now, just enforce
      // that there are none.
      tokens = tokens
        .Consume<OpenParenToken>()
        .Consume<CloseParenToken>();

      // TODO: Parse the statements.  For now, just enforce that
      // there are none
      tokens = tokens
        .Consume<OpenCurlyToken>()
        .Consume<CloseCurlyToken>();

      funcDef.Statements = new List<IStatement>();

      rest = tokens;
      return funcDef;
    }
  }
}