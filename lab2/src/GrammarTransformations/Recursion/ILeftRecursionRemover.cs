using Grammar;

namespace GrammarTransformations.Recursion;

public interface ILeftRecursionRemover
{
    void RemoveLeftRecursion(GrammarDescription grammar);
}