using Grammar;

namespace GrammarIO.Implementations;

public class ConsoleGrammarPrinter : IGrammarPrinter
{
    public void PrintGrammar(GrammarDescription grammar)
    {
        var formatterGrammar = GrammarFormatter.FormatGrammar(grammar);

        Console.WriteLine(formatterGrammar);
    }
}