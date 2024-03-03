namespace RegularExpressionStateMachineBuilder.Exceptions;

public class InvalidRegularExpressionException : Exception
{
    public InvalidRegularExpressionException() : base()
    {
        
    }

    public InvalidRegularExpressionException(string message) : base(message)
    {
        
    }
}