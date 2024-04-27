using Grammar;

namespace GrammarTransformations.Rules;

public class EpsilonRulesRemover : IEpsilonRulesRemover
{
    public void RemoveEpsilonRules(GrammarDescription grammar)
    {
        var epsilonNonTerminals = GetEpsilonGeneratingNonTerminals(grammar);

        DuplicateRulesWithEpsilonGeneratingNonTerminals(grammar, epsilonNonTerminals);

        RemoveImmediateEpsilonRules(grammar);
        
        if (epsilonNonTerminals.Contains(grammar.Axiom))
        {
            var newNonTerminal = grammar.Axiom + "'";
            
            grammar.NonTerminals.Add(newNonTerminal);
            grammar.Productions[newNonTerminal] = [[grammar.Axiom], [GrammarDescription.Epsilon]];
            grammar.Axiom = newNonTerminal;
        }
    }

    private void RemoveImmediateEpsilonRules(GrammarDescription grammar)
    {
        foreach (var nonTerminal in grammar.NonTerminals)
        {
            grammar.Productions[nonTerminal].RemoveAll(p => p is [GrammarDescription.Epsilon]);
        }
    }
    
    private void DuplicateRulesWithEpsilonGeneratingNonTerminals(GrammarDescription grammar,
        List<string> epsilonNonTerminals)
    {
        foreach (var nonTerminal in grammar.NonTerminals)
        {
            var productionsToDuplicate = grammar.Productions[nonTerminal]
                .Where(p => p.Intersect(epsilonNonTerminals).Any())
                .ToList();

            foreach (var production in productionsToDuplicate)
            {
                var newProductions = new List<List<string?>> { production.Cast<string?>().ToList() };

                for (var i = 0; i < production.Count; i++)
                {
                    var symbol = production[i];
                    
                    if(!epsilonNonTerminals.Contains(symbol))
                        continue;

                    foreach (var newProduction in newProductions.ToList())
                    {
                        var newProductionWithoutSymbol = newProduction.ToList();
                        newProductionWithoutSymbol[i] = null;
                        newProductions.Add(newProductionWithoutSymbol);
                    }
                    
                }

                var newNotNullableProductions = newProductions
                    .Select(p => p.Where(s => s is not null).Cast<string>().ToList())
                    .Where(p => p.Count > 0)
                    .Distinct<List<string>>(new SequenceEqualityComparer<string>());
                
                grammar.Productions[nonTerminal].Remove(production);
                grammar.Productions[nonTerminal].AddRange(newNotNullableProductions);
            }
        }
    }
    
    private List<string> GetEpsilonGeneratingNonTerminals(GrammarDescription grammar)
    {
        var result = grammar.Productions
            .Where(kv => kv.Value.Any(p => p is [GrammarDescription.Epsilon]))
            .Select(kv => kv.Key)
            .ToList();

        while (true)
        {
            var nonTerminalsToAdd = new List<string>();

            foreach (var nonTerminal in grammar.NonTerminals.Except(result))
            {
                if (grammar.Productions[nonTerminal].Any(productions => productions.All(p => result.Contains(p))))
                    nonTerminalsToAdd.Add(nonTerminal);
            }

            if (nonTerminalsToAdd.Count == 0)
                break;

            result.AddRange(nonTerminalsToAdd);
        }

        return result;
    }

    private class SequenceEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        public bool Equals(IEnumerable<T>? x, IEnumerable<T>? y)
        {
            return ReferenceEquals(x, y) || (x is not null && y is not null && x.SequenceEqual(y));
        }

        public int GetHashCode(IEnumerable<T> obj)
        {
            // Will not throw an OverflowException
            unchecked
            {
                return obj.Where(e => e != null).Select(e => e?.GetHashCode()).Aggregate(17, (a, b) => (int) (23 * a + b)!);
            }
        }
    }
}