using Grammar;

namespace GrammarIO;

public interface IGrammarWriter
{
    void WriteGrammarToFile(string filePath, GrammarDescription grammar);
}