using System;
using System.Collections.Generic;
using System.Text;

namespace AbstractSyntaxTree
{
  public class RuleParser
  {
    public delegate void NodeParsedHandler<TNode>(TNode node, TokenWalker rest);
    private delegate void NodeParsedHandler(object node, TokenWalker rest);

    private readonly List<IParseRule> _rules = new List<IParseRule>();
    private readonly Dictionary<IParseRule, NodeParsedHandler> _handlers
      = new Dictionary<IParseRule, NodeParsedHandler>();

    public void AddRule<TNode>(IParseRule<TNode> rule, NodeParsedHandler<TNode> handler)
    {
      _rules.Add(rule);
      _handlers.Add(rule, WrappedHandler);

      void WrappedHandler(object node, TokenWalker rest)
      {
        var castedNode = (TNode)node;
        handler(castedNode, rest);
      }
    }

    /// <summary>
    /// Invokes the callback of the first rule that matches.
    /// Returns true if anything matched, otherwise false.
    /// </summary>
    /// <param name="walker"></param>
    /// <returns></returns>
    public bool NextNode(TokenWalker walker)
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

        _handlers[rule]?.Invoke(node, rest);
        return true;
      }

      return false;
    }
  }
}
