using Grammar;

namespace RegularExpressionStateMachineBuilder.Converters;

internal class ConcatSymbolsConverter
{
    private readonly AlphabetDefinition _alphabetDefinition;

    public ConcatSymbolsConverter(AlphabetDefinition alphabetDefinition)
    {
        _alphabetDefinition = alphabetDefinition;
    }

    public string InsertConcatSymbols(string expression)
    {
        var result = expression.ToList();
        
        var symbolsToConcatAfter = _alphabetDefinition.Letters
            .Append(_alphabetDefinition.KleeneStarSymbol)
            .Append(_alphabetDefinition.ClosingBracketSymbol)
            .ToList();

        var symbolsNotToConcatBefore = new List<char>
        {
            _alphabetDefinition.KleeneStarSymbol,
            _alphabetDefinition.ClosingBracketSymbol,
            _alphabetDefinition.UnionSymbol,
            _alphabetDefinition.ConcatSymbol
        };

        for (var i = 0; i < result.Count - 1; i++)
        {
            if (symbolsToConcatAfter.Contains(result[i]) && !symbolsNotToConcatBefore.Contains(result[i + 1])) 
                result.Insert(i + 1, _alphabetDefinition.ConcatSymbol);
        }

        return new string(result.ToArray());
    }
}