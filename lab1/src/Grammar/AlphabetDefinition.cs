using System.Collections.Immutable;

namespace Grammar;

public class AlphabetDefinition
{
    public IReadOnlyCollection<char> Letters { get; }
    public char EpsilonSymbol { get; }
    public char UnionSymbol { get; }
    public char ConcatSymbol { get; }
    public char KleeneStarSymbol { get; }
    public char OpeningBracketSymbol { get; }
    public char ClosingBracketSymbol { get; }

    public AlphabetDefinition(IReadOnlyCollection<char> letters, 
        char epsilonSymbol, 
        char unionSymbol,
        char concatSymbol,
        char kleeneStarSymbol, 
        char openingBracketSymbol, 
        char closingBracketSymbol)
    {
        Letters = letters;
        EpsilonSymbol = epsilonSymbol;
        UnionSymbol = unionSymbol;
        KleeneStarSymbol = kleeneStarSymbol;
        OpeningBracketSymbol = openingBracketSymbol;
        ClosingBracketSymbol = closingBracketSymbol;
        ConcatSymbol = concatSymbol;
    }

    public static AlphabetDefinition CreateDefault()
    {
        var letters = Enumerable.Range('a', 'z' - 'a' + 1)
            .Select(num => (char) num)
            .ToImmutableArray();
        
        return new AlphabetDefinition(letters,
            epsilonSymbol: 'E',
            unionSymbol: '|',
            concatSymbol: '_',
            kleeneStarSymbol: '*',
            openingBracketSymbol: '(',
            closingBracketSymbol: ')');
    }
}