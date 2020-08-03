using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class RuleParser
  {
    public delegate bool IsNodeFinishedPredicate(TokenWalker walker);
    public delegate void ChildNodeParsedHandler<TNode>(TNode node);
    private delegate void ChildNodeParsedHandler(object node);

    private readonly List<IParseRule> _rules = new List<IParseRule>();
    private readonly Dictionary<IParseRule, ChildNodeParsedHandler> _handlers
      = new Dictionary<IParseRule, ChildNodeParsedHandler>();

    private IsNodeFinishedPredicate _nodeFinished;

    public void AddRule<TNode>(IParseRule<TNode> rule, ChildNodeParsedHandler<TNode> handler)
    {
      _rules.Add(rule);
      _handlers.Add(rule, WrappedHandler);

      void WrappedHandler(object node)
      {
        var castedNode = (TNode)node;
        handler(castedNode);
      }
    }

    public void FinishesWhen(IsNodeFinishedPredicate nodeFinished)
    {
      _nodeFinished = nodeFinished;
    }

    public TokenWalker ParseToCompletion(TokenWalker tokens)
    {
      while(!tokens.IsEmpty())
      {
        var token = tokens.Peek();

        if (_nodeFinished(tokens))
          break;

        var result = NextNode(tokens);

        if (!result.success)
        {
          throw new CompileErrorException(
            token.Position,
            $"Unexpected token {token.Content}"
          );
        }

        tokens = result.rest;
      }

      return tokens;
    }

    /// <summary>
    /// Invokes the callback of the first rule that matches.
    /// Returns true if anything matched, otherwise false.
    /// </summary>
    /// <param name="walker"></param>
    /// <returns></returns>
    private (bool success, TokenWalker rest) NextNode(TokenWalker walker)
    {
      // Find the first rule that matches and invoke its callback
      foreach (var rule in _rules)
      {
        if (!rule.IsStartOfNode(walker))
          continue;

        dynamic dynamicRule = rule;
        dynamic result = dynamicRule.ParseNode(walker);
        dynamic node = result.Item1;
        TokenWalker rest = result.Item2;

        _handlers[rule]?.Invoke(node);
        return (true, rest);
      }

      return (false, walker);
    }
  }
}
