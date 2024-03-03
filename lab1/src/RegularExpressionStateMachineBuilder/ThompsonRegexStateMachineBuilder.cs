using FiniteStateMachine;
using Grammar;
using RegularExpressionStateMachineBuilder.Converters;

namespace RegularExpressionStateMachineBuilder;

public class ThompsonRegexStateMachineBuilder : IRegexStateMachineBuilder
{
    private readonly AlphabetDefinition _alphabetDefinition;
    
    public ThompsonRegexStateMachineBuilder(AlphabetDefinition alphabetDefinition)
    {
        _alphabetDefinition = alphabetDefinition;
    }

    public IStateMachine BuildStateMachineFromRegularExpression(string regularExpression)
    {
        var expression = PrepareExpression(regularExpression);

        
        // (a|b)*cd
        // Postfix: a b | * c _ d _
        
        // (a|b)*_(c|d)
        // Postfix: a b | * c d | _
        
        // (a|b)*cd*(f|e)
        // Postfix: a b | * c _ d * _ f e | _
        
        // (a|b((()
        // Error
        
        // (a|(d(fe)))c
        // Postfix: a d f e _ _ | c _
        throw new NotImplementedException();
    }

    private string PrepareExpression(string expression)
    {
        Console.WriteLine("Preparing expression...");
        
        var concatSymbolsConverter = new ConcatSymbolsConverter(_alphabetDefinition);
        var expressionWithConcatSymbols = concatSymbolsConverter.InsertConcatSymbols(expression);
        
        Console.WriteLine($"Added concat symbols: '{expressionWithConcatSymbols}'");

        var postfixNotationConverter = new PostfixNotationConverter(_alphabetDefinition);
        var postfixExpression = postfixNotationConverter.ConvertToPostfixNotation(expressionWithConcatSymbols);
        
        Console.WriteLine($"Converted to postfix form: '{postfixExpression}'");

        return postfixExpression;
    }
}