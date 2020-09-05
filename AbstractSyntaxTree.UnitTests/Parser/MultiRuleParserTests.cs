using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

using AbstractSyntaxTree.Parser;

namespace AbstractSyntaxTree.UnitTests.Parser
{
  public class MultiRuleParserTests
  {
    [Fact]
    public void It_Can_Complete_A_Single_Rule()
    {
      bool callbackInvoked = false;
      var rules = new MultiRuleParser()
        .AddRule<object>(ExpectsWords("A", "simple", "rule"), n => callbackInvoked = true);

      var tokens = WordTokens("A", "simple", "rule");
      var lastResult = FeedToCoroutine(rules, tokens);
      lastResult.AssertComplete();

      Assert.True(callbackInvoked);
    }

    [Fact]
    public void The_Whole_Thing_Fails_If_All_The_Rules_Fail()
    {
      var rules = new MultiRuleParser()
        .AddRule<object>(ExpectsWords("no", "more", "heros"), n => throw new Exception("There were heros."))
        .AddRule<object>(ExpectsWords("no", "way", "this"), n => throw new Exception("It matched"));

      var tokens = WordTokens("no", "way", "words");
      var lastResult = FeedToCoroutine(rules, tokens);
      lastResult.AssertFailed();
    }

    [Fact]
    public void The_First_Rule_To_Complete_Wins()
    {
      int ruleCompleted = -1;

      var rules = new MultiRuleParser()
        .AddRule<object>(ExpectsWords("not", "even", "close"), n => ruleCompleted = 0)
        .AddRule<object>(ExpectsWords("no", "more", "time"), n => ruleCompleted = 1)
        .AddRule<object>(ExpectsWords("no", "more", "time", "left"), n => ruleCompleted = 2);

      var tokens = WordTokens("no", "more", "time");
      var lastResult = FeedToCoroutine(rules, tokens);
      lastResult.AssertComplete();

      Assert.Equal(1, ruleCompleted);
    }

    [Fact]
    public void The_First_Rule_In_The_List_Wins_If_There_Is_A_Tie()
    {
      bool firstCallbackInvoked = false;
      bool secondCallbackInvoked = false;

      var rules = new MultiRuleParser()
        .AddRule<object>(ExpectsWords("foo", "bar", "baz"), n => firstCallbackInvoked = true)
        .AddRule<object>(ExpectsWords("foo", "bar", "baz"), n => secondCallbackInvoked = true);

      var tokens = WordTokens("foo", "bar", "baz");
      var lastResult = FeedToCoroutine(rules, tokens);
      lastResult.AssertComplete();

      Assert.True(firstCallbackInvoked);
      Assert.False(secondCallbackInvoked);
    }

    [Fact]
    public void It_Throws_An_Exception_If_You_Feed_It_A_Token_After_It_Has_Completed()
    {
      var rules = new MultiRuleParser()
        .AddRule<object>(ExpectsWords("foo", "bar", "baz"), n => { });

      var tokens = WordTokens("foo", "bar", "baz");
      var lastResult = FeedAll(rules, tokens);
      lastResult.AssertComplete();

      Assert.ThrowsAny<Exception>(() =>
      {
        FeedToCoroutine(rules, WordTokens("extra"));
      });
    }

    [Fact]
    public void It_Throws_An_Exception_If_You_Feed_It_A_Token_After_It_Has_Failed()
    {
      var rules = new MultiRuleParser()
        .AddRule<object>(ExpectsWords("foo", "bar", "baz"), n => { });

      var tokens = WordTokens("foo", "bar", "fizz");
      var lastResult = FeedAll(rules, tokens);
      lastResult.AssertFailed();

      Assert.ThrowsAny<Exception>(() =>
      {
        FeedToCoroutine(rules, WordTokens("extra"));
      });
    }

    /// <summary>
    /// Feeds all tokens to the parser and returns the last result.
    /// </summary>
    private RuleResult FeedAll(IRuleParser parser, IEnumerable<Token> tokens)
    {
      RuleResult lastResult = default;

      foreach (Token t in tokens)
        lastResult = parser.FeedToken(t);

      return lastResult;
    }

    /// <summary>
    /// Converts the multirule parser into a coroutine.
    /// Runs the coroutine until it stops, and returns the final result.
    /// </summary>
    /// <param name="tokens">The sequence of tokens that the current token callback will return.</param>
    private RuleResult FeedToCoroutine(MultiRuleParser rules, IEnumerable<Token> tokens)
    {
      var tokenMachine = tokens.GetEnumerator();
      return rules.ToCoroutine(CurrentToken).Last();

      Token CurrentToken()
      {
        tokenMachine.MoveNext();
        return tokenMachine.Current;
      }
    }

    private RuleCoroutine ExpectsWords(params string[] words)
    {
      return NamingIsHard;
      IEnumerable<RuleResult> NamingIsHard(CurrentTokenCallback currentToken)
      {
        // Expect all the words except the last one
        for (int i = 0; i < words.Length - 1; i++)
          yield return Expect.Word(currentToken(), words[i]);

        // The last one needs to include a non-null null value for the
        // node, so it will be marked as Complete instead of GoodSoFar
        yield return Expect.Word(currentToken(), words.Last(), true);
      };
    }

    private IEnumerable<Token> WordTokens(params string[] words)
    {
      int charCount = 0;
      foreach (string content in words)
      {
        var pos = new CodePos(0, charCount);
        charCount += content.Length + 1; // Add the length of the word, plus an imaginary space.

        yield return new Token(pos, TokenType.Word, content);
      }
    }
  }

}
