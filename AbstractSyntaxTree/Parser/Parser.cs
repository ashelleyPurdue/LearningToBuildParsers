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
      "function",
      "let"
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
        Token token = tokens.Peek();

        switch (token.Type)
        {
          case TokenType.Keyword when token.Content == "class":
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
        .Expect(TokenType.Word)
        .Content;

      tokens = tokens.Consume();

      // Parse the insides of the class
      classDef.Functions = new List<FunctionDefinition>();
      tokens = tokens.ConsumeSymbol("{");

      var rules = new RuleParser();
      rules.FinishesWhen(t => t.Peek().Content == "}");
      rules.AddRule(new FunctionDefinitionRule(), classDef.Functions.Add);

      tokens = rules.ParseToCompletion(tokens);

      tokens = tokens.ConsumeSymbol("}");
      rest = tokens;
      return classDef;
    }
  }
}