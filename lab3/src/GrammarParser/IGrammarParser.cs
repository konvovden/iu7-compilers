using DerivationTree;
using TextLexer;

namespace GrammarParser;

public interface IGrammarParser
{
    Tree Parse(ILexer lexer);
}