namespace Grammar;

public class GrammarDescription
{
    public const string Epsilon = "Eps";
    public List<string> NonTerminals { get; set; }
    public List<string> Terminals { get; set; }
    public Dictionary<string, List<List<string>>> Productions { get; set; }
    public string Axiom { get; set; }

    public GrammarDescription(List<string> nonTerminals, 
        List<string> terminals,
        Dictionary<string, List<List<string>>> productions,
        string axiom)
    {
        NonTerminals = nonTerminals;
        Terminals = terminals;
        Productions = productions;
        Axiom = axiom;
    }
}