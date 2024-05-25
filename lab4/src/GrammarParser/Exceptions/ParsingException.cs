namespace GrammarParser.Exceptions;

public class ParsingException : Exception
{
    public ParsingException() : base()
    {
        
    }

    public ParsingException(int symbolNumber) : base($"Error while parsing symbol at position {symbolNumber}")
    {
        
    }
}