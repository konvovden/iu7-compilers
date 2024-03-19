namespace Grammar;

public static class AlphabetDefinitionExtensions
{
    public static bool IsLetter(this AlphabetDefinition alphabetDefinition, char symbol)
    {
        return alphabetDefinition.Letters.Contains(symbol);
    }

    public static bool IsConcatSymbol(this AlphabetDefinition alphabetDefinition, char symbol)
    {
        return symbol == alphabetDefinition.ConcatSymbol;
    }

    public static bool IsUnionSymbol(this AlphabetDefinition alphabetDefinition, char symbol)
    {
        return symbol == alphabetDefinition.UnionSymbol;
    }

    public static bool IsKleeneStarSymbol(this AlphabetDefinition alphabetDefinition, char symbol)
    {
        return symbol == alphabetDefinition.KleeneStarSymbol;
    }
    
    public static bool IsOpeningBracket(this AlphabetDefinition alphabetDefinition, char symbol)
    {
        return symbol == alphabetDefinition.OpeningBracketSymbol;
    }

    public static bool IsClosingBracket(this AlphabetDefinition alphabetDefinition, char symbol)
    {
        return symbol == alphabetDefinition.ClosingBracketSymbol;
    }
    
    public static bool IsPostfixOperator(this AlphabetDefinition alphabetDefinition, char symbol)
    {
        return alphabetDefinition.IsKleeneStarSymbol(symbol);
    }

    public static bool IsBinaryOperation(this AlphabetDefinition alphabetDefinition, char symbol)
    {
        return alphabetDefinition.IsConcatSymbol(symbol) ||
               alphabetDefinition.IsUnionSymbol(symbol);
    }
}