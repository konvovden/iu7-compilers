using Grammar;
using GrammarIO;
using GrammarIO.Implementations;
using GrammarTransformations.Rules;

namespace Startup.RemoveEpsilonRules;

public class Program
{
    public static void Main(string[] args)
    {
        /*var grammar = new GrammarDescription(["A"],
            ["a", "b", "c", "d"],
            new Dictionary<string, List<List<string>>>
            {
                {"A", [["A", "c"], ["A", "a", "d"], ["b", "d"], [GrammarDescription.Epsilon]]},
            },
            "A");

        var writer = new JsonGrammarWriter();
        writer.WriteGrammarToFile("examples/3.json", grammar);*/

        IGrammarReader grammarReader = new JsonGrammarReader();
        var grammar = grammarReader.ReadGrammarFromFile("examples/2_4_11.json");
        
        IGrammarPrinter grammarPrinter = new ConsoleGrammarPrinter();
        
        Console.WriteLine();
        Console.WriteLine("Initial grammar:");
        grammarPrinter.PrintGrammar(grammar);

        Console.WriteLine();
        Console.WriteLine("Removing epsilon rules...");

        IEpsilonRulesRemover epsilonRulesRemover = new EpsilonRulesRemover();
        epsilonRulesRemover.RemoveEpsilonRules(grammar);

        Console.WriteLine("Result grammar:");
        grammarPrinter.PrintGrammar(grammar);
    }
}