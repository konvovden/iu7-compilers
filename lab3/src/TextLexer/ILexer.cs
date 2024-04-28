namespace TextLexer;

public interface ILexer
{
    int NextTokenPosition { get; }
    string NextToken { get; }
    
    void MoveNext(int count = 1);
}