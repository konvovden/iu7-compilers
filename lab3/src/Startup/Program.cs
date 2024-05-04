using GrammarParser;
using TextLexer;
using TreeVisualization;

namespace Startup;

class Program
{
    static void Main(string[] args)
    {
        var exampleName = "3";

        var exampleFile = $"examples/{exampleName}.txt";

        Console.WriteLine($"Reading text from file '{exampleFile}'...");

        var exampleText = File.ReadAllText(exampleFile);

        Console.WriteLine("Example text:");
        Console.WriteLine(exampleText);

        Console.WriteLine();
        Console.WriteLine("Parsing text...");

        ILexer lexer = new Lexer(exampleText);
        IGrammarParser parser = new GrammarParser.GrammarParser();

        var tree = parser.Parse(lexer);

        Console.WriteLine("Saving results to file...");

        var resultFile = $"out/{exampleName}";
        
        GraphvizTreeVisualizer.SaveTreeToFile(tree, resultFile);
    }
}
