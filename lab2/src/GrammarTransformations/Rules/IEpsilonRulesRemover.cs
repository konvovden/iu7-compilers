using Grammar;

namespace GrammarTransformations.Rules;

public interface IEpsilonRulesRemover
{
    void RemoveEpsilonRules(GrammarDescription grammar);
}