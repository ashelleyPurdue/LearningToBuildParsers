using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class ClassParseRule : IParseRule<ClassDefinition>
  {
    public bool IsStartOfNode(TokenWalker walker)
    {
      var t = walker.Peek();
      return t.Type == TokenType.Keyword && t.Content == "class";
    }

    public (ClassDefinition node, TokenWalker rest) ParseNode(TokenWalker tokens)
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
      return (classDef, tokens);
    }
  }
}
