using System.Text.RegularExpressions;
using DerivationTree;
using GrammarParser.Exceptions;
using TextLexer;

namespace GrammarParser;

public class GrammarParser : IGrammarParser
{
    private static Regex _identificatorRegex = new("^[a-z]+$");
    private const string EPSILON = "Eps";
    
    public Tree Parse(ILexer lexer)
    {
        var programNode = ParseExpression(lexer);

        return new Tree(programNode);
    }
    
    private TreeNode ParseExpression(ILexer lexer)
    {
        var firstArithmeticExpressionNode = ParseArithmeticExpression(lexer);

        if (!IsRelationshipOperation(lexer.NextToken))
            throw new ParsingException(lexer.NextTokenPosition);
        
        var relationshipOperationNode = ParseRelationshipOperation(lexer);
        
        var secondArithmeticExpressionNode = ParseArithmeticExpression(lexer);

        var attributeValue =
            $"{firstArithmeticExpressionNode.Attribute} {secondArithmeticExpressionNode.Attribute} {relationshipOperationNode.Attribute}";
        
        return new TreeNode("<expression>",
            attributeValue,
            [firstArithmeticExpressionNode, relationshipOperationNode, secondArithmeticExpressionNode]);
    }

    private TreeNode ParseArithmeticExpression(ILexer lexer)
    {
        TreeNode[] childNodes;
        string attributeValue;
        
        if (IsSumOperation(lexer.NextToken))
        {
            var sumOperationNode = ParseSumOperation(lexer);
            var termNode = ParseTerm(lexer);
            var dashArithmeticExpressionNode = ParseDashArithmeticExpression(lexer);

            attributeValue =
                $"{termNode.Attribute} {dashArithmeticExpressionNode.Attribute} {sumOperationNode.Attribute}";
            childNodes = [sumOperationNode, termNode, dashArithmeticExpressionNode];
        }
        else
        {
            var termNode = ParseTerm(lexer);
            var dashArithmeticExpressionNode = ParseDashArithmeticExpression(lexer);

            attributeValue = $"{termNode.Attribute} {dashArithmeticExpressionNode.Attribute}";
            childNodes = [termNode, dashArithmeticExpressionNode];
        }

        return new TreeNode("<arithmetical expression>", 
            attributeValue,
            childNodes);
    }

    private TreeNode ParseTerm(ILexer lexer)
    {
        var factorNode = ParseFactor(lexer);
        var dashTermNode = ParseDashTerm(lexer);

        return new TreeNode("<term>", 
            $"{factorNode.Attribute} {dashTermNode.Attribute}",
            [factorNode, dashTermNode]);
    }

    private TreeNode ParseFactor(ILexer lexer)
    {
        var primaryExpressionNode = ParsePrimaryExpression(lexer);
        var dashFactorNode = ParseDashFactor(lexer);

        return new TreeNode("<factor>", 
            $"{primaryExpressionNode.Attribute} {dashFactorNode.Attribute}",
            [primaryExpressionNode, dashFactorNode]);
    }

    private TreeNode ParsePrimaryExpression(ILexer lexer)
    {
        TreeNode[] childNodes;
        string attributeValue;
        
        if (lexer.NextToken == "(")
        {
            var openBracketNode = new TreeNode("(", string.Empty);
            lexer.MoveNext();

            var arithmeticExpressionNode = ParseArithmeticExpression(lexer);

            if (lexer.NextToken != ")")
                throw new ParsingException(lexer.NextTokenPosition);

            var closingBracketNode = new TreeNode(")", string.Empty);
            lexer.MoveNext();

            attributeValue = arithmeticExpressionNode.Attribute;
            childNodes = [openBracketNode, arithmeticExpressionNode, closingBracketNode];
        }
        else if (IsNumber(lexer.NextToken))
        {
            var numberNode = ParseNumber(lexer);

            attributeValue = numberNode.Attribute;
            childNodes = [numberNode];
        }
        else if (IsIdentificator(lexer.NextToken))
        {
            var identificatorNode = ParseIdentificator(lexer);

            attributeValue = identificatorNode.Attribute;
            childNodes = [identificatorNode];
        }
        else
            throw new ParsingException(lexer.NextTokenPosition);

        return new TreeNode("<primary expression>", 
            attributeValue,
            childNodes);
    }

    private TreeNode ParseDashArithmeticExpression(ILexer lexer)
    {
        TreeNode[] childNodes;
        string attributeValue;
        
        if (!lexer.IsEnd && IsSumOperation(lexer.NextToken))
        {
            var sumOperationNode = ParseSumOperation(lexer);
            var termNode = ParseTerm(lexer);
            var dashArithmeticExpressions = ParseDashArithmeticExpression(lexer);

            attributeValue = $"{termNode.Attribute} {dashArithmeticExpressions.Attribute} {sumOperationNode.Attribute}";
            childNodes = [sumOperationNode, termNode, dashArithmeticExpressions];
        }
        else
        {
            attributeValue = string.Empty;
            childNodes = [new TreeNode(EPSILON, string.Empty)];
        }

        return new TreeNode("<arithmetical expression>'", attributeValue, childNodes);
    }
    
    private TreeNode ParseDashTerm(ILexer lexer)
    {
        TreeNode[] childNodes;
        string attributeValue;
        
        if (!lexer.IsEnd && IsMultiplyOperation(lexer.NextToken))
        {
            var multiplyOperationNode = ParseMultiplyOperation(lexer);
            var factorNode = ParseFactor(lexer);
            var dashTermNode = ParseDashTerm(lexer);

            attributeValue = $"{factorNode.Attribute} {dashTermNode.Attribute} {multiplyOperationNode.Attribute}";
            childNodes = [multiplyOperationNode, factorNode, dashTermNode];
        }
        else
        {
            attributeValue = string.Empty;
            childNodes = [new TreeNode(EPSILON, string.Empty)];
        }

        return new TreeNode("<term>'", attributeValue, childNodes);
    }
    
    private TreeNode ParseDashFactor(ILexer lexer)
    {
        TreeNode[] childNodes;
        string attributeValue;
        
        if (!lexer.IsEnd && lexer.NextToken == "^")
        {
            var arrowNode = new TreeNode("^", "^");
            lexer.MoveNext();

            var primaryExpressionNode = ParsePrimaryExpression(lexer);
            var dashFactorNode = ParseDashFactor(lexer);

            attributeValue = $"{primaryExpressionNode.Attribute} {dashFactorNode.Attribute} {arrowNode.Attribute}";
            childNodes = [arrowNode, primaryExpressionNode, dashFactorNode];
        }
        else
        {
            attributeValue = string.Empty;
            childNodes = [new TreeNode(EPSILON, string.Empty)];
        }

        return new TreeNode("<factor>'", attributeValue, childNodes);
    }
    
    private TreeNode ParseIdentificator(ILexer lexer)
    {
        var identificator = lexer.NextToken;
        
        lexer.MoveNext();
        
        return new TreeNode("<id>", identificator, [new TreeNode(identificator, identificator)]);
    }

    private TreeNode ParseSumOperation(ILexer lexer)
    {
        var sumOperation = lexer.NextToken;

        lexer.MoveNext();

        return new TreeNode("<sum operation symbol>", sumOperation, [new TreeNode(sumOperation, sumOperation)]);
    }

    private TreeNode ParseMultiplyOperation(ILexer lexer)
    {
        var multiplyOperation = lexer.NextToken;

        lexer.MoveNext();

        return new TreeNode("<multiply operation symbol>", multiplyOperation, [new TreeNode(multiplyOperation, multiplyOperation)]);
    }

    private TreeNode ParseRelationshipOperation(ILexer lexer)
    {
        var relationshipOperation = lexer.NextToken;

        lexer.MoveNext();

        return new TreeNode("<relationship operation symbol>", relationshipOperation, [new TreeNode(relationshipOperation, relationshipOperation)]);
    }

    private TreeNode ParseNumber(ILexer lexer)
    {
        var number = int.Parse(lexer.NextToken);

        lexer.MoveNext();

        var numberString = number.ToString();
        
        return new TreeNode("<number>", numberString, [new TreeNode(numberString, numberString)]);
    }
    
    private bool IsIdentificator(string text)
    {
        return _identificatorRegex.IsMatch(text);
    }

    private bool IsRelationshipOperation(string text)
    {
        string[] relationshipOperations = ["<", "<=", "==", ">=", ">", "<>"];

        return relationshipOperations.Contains(text);
    }

    private bool IsSumOperation(string text)
    {
        string[] sumOperations = ["+", "-"];

        return sumOperations.Contains(text);
    }

    private bool IsMultiplyOperation(string text)
    {
        string[] multiplyOperations = ["*", "/", "%"];

        return multiplyOperations.Contains(text);
    }

    private bool IsNumber(string text)
    {
        return int.TryParse(text, out _);
    }
}