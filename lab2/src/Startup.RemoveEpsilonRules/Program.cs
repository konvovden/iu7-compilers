using Grammar;
using GrammarIO;
using GrammarIO.Implementations;
using GrammarTransformations.Rules;

namespace Startup.RemoveEpsilonRules;

public class Program
{
    public static void Main(string[] args)
    {
        var grammar = new GrammarDescription(["S", "A", "B", "C"],
            ["a", "b"],
            new Dictionary<string, List<List<string>>>
            {
                {"S", [["A", "B", "C"]]},
                {"A", [["B", "B"], [GrammarDescription.Epsilon]]},
                {"B", [["C", "C"], ["a"]]},
                {"C", [["A", "A"], ["b"]]}
            },
            "S");

        var writer = new JsonGrammarWriter();
        writer.WriteGrammarToFile("examples/2_4_11.json", grammar);

        /*IGrammarReader grammarReader = new JsonGrammarReader();
        var grammar = grammarReader.ReadGrammarFromFile("examples/4_11.json");*/
        
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