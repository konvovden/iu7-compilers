using Grammar;

namespace GrammarTransformations.Recursion;

public class LeftRecursionRemover : ILeftRecursionRemover
{
    public void RemoveLeftRecursion(GrammarDescription grammar)
    {
        var oldNonTerminalsCount = grammar.NonTerminals.Count;
        
        for (var i = 0; i < oldNonTerminalsCount; i++)
        {
            var Ai = grammar.NonTerminals[i];

            for (var j = 0; j < i; j++)
            {
                var Aj = grammar.NonTerminals[j];

                var productionsToReplace = grammar.Productions[Ai]
                    .Where(p => p[0] == Aj)
                    .ToList();

                foreach (var iProduction in productionsToReplace)
                {
                    grammar.Productions[Ai].Remove(iProduction);
                    
                    foreach (var jProduction in grammar.Productions[Aj])
                    {
                        var newProduction = iProduction
                            .ToList();
                        
                        newProduction.RemoveAt(0);
                        newProduction.InsertRange(0, jProduction);
                        
                        grammar.Productions[Ai].Add(newProduction);
                    }
                }
            }

            RemoveImmediateLeftRecursion(grammar, Ai);
        }
    }

    private void RemoveImmediateLeftRecursion(GrammarDescription grammar, string nonTerminal)
    {
        var productions = grammar.Productions[nonTerminal];

        var recursiveProductions = productions
            .Where(p => p[0] == nonTerminal)
            .ToList();

        if (recursiveProductions.Count == 0)
            return;
        
        var nonRecursiveProductions = productions.Except(recursiveProductions).ToList();
        
        var newNonTerminal = nonTerminal + "'";
        
        nonRecursiveProductions.ForEach(p =>
        {
            if(p[0] == GrammarDescription.Epsilon)
                p.RemoveAt(0);
            
            p.Add(newNonTerminal);
        });
        
        grammar.Productions[nonTerminal] = nonRecursiveProductions;
        
        recursiveProductions.ForEach(p => p.RemoveAt(0));
        recursiveProductions.ForEach(p => p.Add(newNonTerminal));
        recursiveProductions.Add([GrammarDescription.Epsilon]);
        
        grammar.NonTerminals.Add(newNonTerminal);
        grammar.Productions[newNonTerminal] = recursiveProductions;
    }
}