using System.Text.Json.Serialization;
using Grammar;
using RegularExpressionStateMachineBuilder;
using StateMachineVisualization;

namespace Startup;

public static class Program
{
    public static void Main()
    {
        var alphabet = AlphabetDefinition.CreateDefault();
        
        Console.WriteLine("Input regular expression:");
        //var regularExpression = Console.ReadLine()!;
        var regularExpression = "abcd";
        
        Console.WriteLine($"Regular expression: '{regularExpression}'");
        
        var stateMachineBuilder = new ThompsonRegexStateMachineBuilder(alphabet);
        
        var stateMachine = stateMachineBuilder.BuildStateMachineFromRegularExpression(regularExpression);

        ConsoleStateMachineWriter.WriteStateMachineToConsole(stateMachine);
        GraphVizStateMachineVisualizer.SaveStateMachineGraphToFile(stateMachine, "test");

        Console.ReadKey();
    }
}