using Grammar;
using GrammarIO;
using GrammarIO.Implementations;
using GrammarTransformations.Recursion;

namespace Startup.RemoveRecursion;

public class Program
{
    // https://cyberzhg.github.io/toolbox/left_rec
    public static void Main(string[] args)
    {
        /*var grammar = new GrammarDescription(["S"],
            ["+", "(", ")", "*", "a"],
            new Dictionary<string, List<List<string>>>
            {
                {"S", [["S", "+", "S"], ["S", "S"], ["(", "S", ")"], ["S", "*"], ["a"]]}
            },
            "S");

        var writer = new JsonGrammarWriter();
        writer.WriteGrammarToFile("examples/3.json", grammar);*/

        IGrammarReader grammarReader = new JsonGrammarReader();
        var grammar = grammarReader.ReadGrammarFromFile("examples/1.json");
        
        IGrammarPrinter grammarPrinter = new ConsoleGrammarPrinter();
        
        Console.WriteLine();
        Console.WriteLine("Initial grammar:");
        grammarPrinter.PrintGrammar(grammar);

        Console.WriteLine();
        Console.WriteLine("Removing left recursion from grammar...");
        Console.WriteLine("Result grammar:");
        
        ILeftRecursionRemover leftRecursionRemover = new LeftRecursionRemover();
        leftRecursionRemover.RemoveLeftRecursion(grammar);
        grammarPrinter.PrintGrammar(grammar);
    }
}