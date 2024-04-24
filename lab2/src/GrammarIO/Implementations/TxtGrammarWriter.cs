using System.Text;
using Grammar;

namespace GrammarIO.Implementations;

public class TxtGrammarWriter : IGrammarWriter
{
    public void WriteGrammarToFile(string filePath, GrammarDescription grammar)
    {
        var fileContent = GrammarFormatter.FormatGrammar(grammar);
        
        File.WriteAllText(filePath, fileContent);
    }
}