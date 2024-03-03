using Grammar;
using RegularExpressionStateMachineBuilder.Exceptions;

namespace RegularExpressionStateMachineBuilder.Converters;

internal class PostfixNotationConverter
{
    private readonly AlphabetDefinition _alphabetDefinition;

    private readonly IReadOnlyDictionary<char, int> _binaryOperationsPriority;

    public PostfixNotationConverter(AlphabetDefinition alphabetDefinition)
    {
        _alphabetDefinition = alphabetDefinition;

        _binaryOperationsPriority = new Dictionary<char, int>()
        {
            {_alphabetDefinition.ConcatSymbol, 1},
            {_alphabetDefinition.UnionSymbol, 0}
        };
    }

    public string ConvertToPostfixNotation(string expression)
    {
        var stack = new Stack<char>();

        var output = new List<char>();

        for (var i = 0; i < expression.Length; i++)
        {
            try
            {
                var symbol = expression[i];
            
                if (IsLetter(symbol) || IsPostfixOperator(symbol))
                    output.Add(symbol);
                else if (IsOpeningBracket(symbol))
                    stack.Push(symbol);
                else if (IsClosingBracket(symbol))
                {
                    while (!IsOpeningBracket(stack.Peek()))
                    {
                        var stackSymbol = stack.Pop();

                        output.Add(stackSymbol);
                    }

                    stack.Pop();
                }
                else if (IsBinaryOperation(symbol))
                {
                    while (stack.Count != 0 
                           && IsBinaryOperation(stack.Peek())
                           && GetBinaryOperationPriority(stack.Peek()) >= GetBinaryOperationPriority(symbol))
                    {
                        var stackSymbol = stack.Pop();
                    
                        output.Add(stackSymbol);
                    }
                
                    stack.Push(symbol);
                }
                else
                {
                    throw new InvalidOperationException("Invalid symbol");
                }
            }
            catch (Exception)
            {
                throw new InvalidRegularExpressionException($"Invalid regular expression passed. Symbol position: {i + 1}");
            }
        }

        while (stack.Count != 0)
        {
            var stackSymbol = stack.Pop();

            if (!IsPostfixOperator(stackSymbol) && !IsBinaryOperation(stackSymbol))
                throw new InvalidRegularExpressionException("Invalid regular expression passed. Mismatched brackets");

            
            output.Add(stackSymbol);
        }

        return new string(output.ToArray());
    }

    private bool IsLetter(char symbol)
    {
        return _alphabetDefinition.Letters.Contains(symbol);
    }

    private bool IsPostfixOperator(char symbol)
    {
        return symbol == _alphabetDefinition.KleeneStarSymbol;
    }

    private bool IsOpeningBracket(char symbol)
    {
        return symbol == _alphabetDefinition.OpeningBracketSymbol;
    }

    private bool IsClosingBracket(char symbol)
    {
        return symbol == _alphabetDefinition.ClosingBracketSymbol;
    }

    private bool IsBinaryOperation(char symbol)
    {
        return symbol == _alphabetDefinition.ConcatSymbol ||
               symbol == _alphabetDefinition.UnionSymbol;
    }

    private int GetBinaryOperationPriority(char operationSymbol)
    {
        return _binaryOperationsPriority[operationSymbol];
    }
}