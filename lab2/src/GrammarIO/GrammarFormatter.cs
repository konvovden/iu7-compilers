using System.Text;
using Grammar;

namespace GrammarIO;

public class GrammarFormatter
{
    public static string FormatGrammar(GrammarDescription grammar)
    {
        var builder = new StringBuilder();
        
        builder.AppendLine($"NonTerminals: {string.Join(", ", grammar.NonTerminals)}");

        builder.AppendLine($"Terminals: {string.Join(", ", grammar.Terminals)}");

        builder.AppendLine($"Axiom: {grammar.Axiom}");

        builder.AppendLine("Rules:");
        
        foreach (var kv in grammar.Productions)
        {
            var productionsStrings = kv.Value
                .Select(s => string.Join(" ", s));

            builder.AppendLine($"{kv.Key} -> {string.Join(" | ", productionsStrings)}");
        }

        return builder.ToString();
    }
}