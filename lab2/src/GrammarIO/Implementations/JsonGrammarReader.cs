using Grammar;
using Newtonsoft.Json;

namespace GrammarIO.Implementations;

public class JsonGrammarReader : IGrammarReader
{
    public GrammarDescription ReadGrammarFromFile(string filePath)
    {
        var serializedGrammar = File.ReadAllText(filePath);

        return JsonConvert.DeserializeObject<GrammarDescription>(serializedGrammar)!;
    }
}