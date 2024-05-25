namespace TextLexer;

public interface ILexer
{
    int NextTokenPosition { get; }
    string NextToken { get; }
    bool IsEnd { get; }

    void MoveNext(int count = 1);
}