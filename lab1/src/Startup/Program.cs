using Grammar;
using RegularExpressionStateMachineBuilder;
using StateMachineDeterminer;
using StateMachineVisualization;

namespace Startup;

public static class Program
{
    // https://cyberzhg.github.io/toolbox/regex2nfa
    // (a|b)*cd
        
    // (a|b)*_(c|d)
        
    // (a|b)*cd*(f|e)
        
    // (a|b((() - ERROR
        
    // (a|(d(fe)))c
    
    // (a|b)*abb(a|b)*
    
    public static void Main()
    {
        var alphabet = AlphabetDefinition.CreateDefault();
        
        Console.WriteLine("Input regular expression:");
        //var regularExpression = Console.ReadLine()!;
        var regularExpression = "(a|b)*abb(a|b)*";
        
        Console.WriteLine($"Regular expression: '{regularExpression}'");
        
        var stateMachineBuilder = new ThompsonRegexStateMachineBuilder(alphabet);
        
        var nfa = stateMachineBuilder.BuildStateMachineFromRegularExpression(regularExpression);

        Console.WriteLine("Created NFA from regular expression.");

        // ConsoleStateMachineWriter.WriteStateMachineToConsole(nfa);
        GraphVizStateMachineVisualizer.SaveStateMachineGraphToFile(nfa, "nfa");

        var stateMachineDeterminer = new ThompsonStateMachineDeterminer(alphabet);

        var dfa = stateMachineDeterminer.DetermineStateMachine(nfa);
        
        Console.WriteLine("Created DFA from NFA.");
        
        GraphVizStateMachineVisualizer.SaveStateMachineGraphToFile(dfa, "dfa");
        
        Console.ReadKey();
    }
}