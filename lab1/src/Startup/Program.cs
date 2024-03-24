using Grammar;
using RegularExpressionStateMachineBuilder;
using StateMachineDeterminer;
using StateMachineMinimizer;
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

        var stateMachineMinimizer = new HopcroftStateMachineMinimizer();

        var minDfa = stateMachineMinimizer.MinimizeStateMachine(dfa);
        
        Console.WriteLine("Created minimal DFA from DFA.");
        
        GraphVizStateMachineVisualizer.SaveStateMachineGraphToFile(minDfa, "minDfa");

        while (true)
        {
            Console.WriteLine("Input string to check:");

            var inputString = Console.ReadLine()!;

            var result = minDfa.Simulate(inputString);
            
            Console.WriteLine($"Result: {result}");
        }
    }
}