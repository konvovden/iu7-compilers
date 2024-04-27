using Grammar;

namespace GrammarIO;

public interface IGrammarReader
{
    GrammarDescription ReadGrammarFromFile(string filePath);
}