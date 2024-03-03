using Grammar;
using RegularExpressionStateMachineBuilder;

namespace Startup;

public static class Program
{
    public static void Main()
    {
        var alphabet = AlphabetDefinition.CreateDefault();
        
        Console.WriteLine("Input regular expression:");
        //var regularExpression = Console.ReadLine()!;
        var regularExpression = "(a|(d(fe)))c";
        
        Console.WriteLine($"Regular expression: '{regularExpression}'");
        
        var stateMachineBuilder = new ThompsonRegexStateMachineBuilder(alphabet);
        
        var stateMachine = stateMachineBuilder.BuildStateMachineFromRegularExpression(regularExpression);
    }
}