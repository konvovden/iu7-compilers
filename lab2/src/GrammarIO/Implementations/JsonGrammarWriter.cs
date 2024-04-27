using Grammar;
using Newtonsoft.Json;

namespace GrammarIO.Implementations;

public class JsonGrammarWriter : IGrammarWriter
{
    public void WriteGrammarToFile(string filePath, GrammarDescription grammar)
    {
        var serializedGrammar = JsonConvert.SerializeObject(grammar, Formatting.Indented);

        File.WriteAllText(filePath, serializedGrammar);
    }
}