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
        var programNode = ParseProgram(lexer);

        return new Tree(programNode);
    }

    private TreeNode ParseProgram(ILexer lexer)
    {
        var childNode = ParseBlock(lexer);

        return new TreeNode("<program>", [childNode]);
    }

    private TreeNode ParseBlock(ILexer lexer)
    {
        var childNodes = new TreeNode[3];
        
        if (lexer.NextToken != "{")
            throw new ParsingException(lexer.NextTokenPosition);
        
        lexer.MoveNext();
        childNodes[0] = new TreeNode("{");
            
        
        childNodes[1] = ParseOperatorsList(lexer);

        if (lexer.NextToken != "}")
            throw new ParsingException(lexer.NextTokenPosition);

        lexer.MoveNext();
        childNodes[2] = new TreeNode("}");
        
        return new TreeNode("<block>", childNodes);
    }

    private TreeNode ParseOperatorsList(ILexer lexer)
    {
        var childNodes = new TreeNode[2];

        childNodes[0] = ParseOperator(lexer);
        childNodes[1] = ParseTail(lexer);

        return new TreeNode("<operators list>", childNodes);
    }

    private TreeNode ParseTail(ILexer lexer)
    {
        TreeNode[] childNodes;

        if (lexer.NextToken == ";")
        {
            var pointDotNode = new TreeNode(";");
            lexer.MoveNext();

            var operatorNode = ParseOperator(lexer);
            var tailNode = ParseTail(lexer);

            childNodes = [pointDotNode, operatorNode, tailNode];
        }
        else
            childNodes = [new TreeNode(EPSILON)];

        return new TreeNode("<tail>", childNodes);
    }

    private TreeNode ParseOperator(ILexer lexer)
    {
        TreeNode[] childNodes;

        var next = lexer.NextToken;

        if (IsIdentificator(next))
        {
            var identificatorNode = ParseIdentificator(lexer);

            if (lexer.NextToken != "=")
                throw new ParsingException(lexer.NextTokenPosition);

            var equalsNode = new TreeNode("=");
            lexer.MoveNext();

            var expressionNode = ParseExpression(lexer);

            childNodes = [identificatorNode, equalsNode, expressionNode];
        }
        else
        {
            childNodes = [ParseBlock(lexer)];
        }

        return new TreeNode("<operator>", childNodes);
    }

    private TreeNode ParseExpression(ILexer lexer)
    {
        var firstArithmeticExpressionNode = ParseArithmeticExpression(lexer);

        if (!IsRelationshipOperation(lexer.NextToken))
            throw new ParsingException(lexer.NextTokenPosition);
        
        var relationshipOperationNode = ParseRelationshipOperation(lexer);
        
        var secondArithmeticExpressionNode = ParseArithmeticExpression(lexer);

        return new TreeNode("<expression>",
            [firstArithmeticExpressionNode, relationshipOperationNode, secondArithmeticExpressionNode]);
    }

    private TreeNode ParseArithmeticExpression(ILexer lexer)
    {
        TreeNode[] childNodes;
        
        if (IsSumOperation(lexer.NextToken))
        {
            var sumOperationNode = ParseSumOperation(lexer);
            var termNode = ParseTerm(lexer);
            var dashArithmeticExpressionNode = ParseDashArithmeticExpression(lexer);

            childNodes = [sumOperationNode, termNode, dashArithmeticExpressionNode];
        }
        else
        {
            var termNode = ParseTerm(lexer);
            var dashArithmeticExpressionNode = ParseDashArithmeticExpression(lexer);

            childNodes = [termNode, dashArithmeticExpressionNode];
        }

        return new TreeNode("<arithmetical expression>", childNodes);
    }

    private TreeNode ParseTerm(ILexer lexer)
    {
        var factorNode = ParseFactor(lexer);
        var dashTermNode = ParseDashTerm(lexer);

        return new TreeNode("<term>", [factorNode, dashTermNode]);
    }

    private TreeNode ParseFactor(ILexer lexer)
    {
        var primaryExpressionNode = ParsePrimaryExpression(lexer);
        var dashFactorNode = ParseDashFactor(lexer);

        return new TreeNode("<factor>", [primaryExpressionNode, dashFactorNode]);
    }

    private TreeNode ParsePrimaryExpression(ILexer lexer)
    {
        TreeNode[] childNodes;
        
        if (lexer.NextToken == "(")
        {
            var openBracketNode = new TreeNode("(");
            lexer.MoveNext();

            var arithmeticExpressionNode = ParseArithmeticExpression(lexer);

            if (lexer.NextToken != ")")
                throw new ParsingException(lexer.NextTokenPosition);

            var closingBracketNode = new TreeNode(")");
            lexer.MoveNext();

            childNodes = [openBracketNode, arithmeticExpressionNode, closingBracketNode];
        }
        else if (IsNumber(lexer.NextToken))
        {
            var numberNode = ParseNumber(lexer);

            childNodes = [numberNode];
        }
        else if (IsIdentificator(lexer.NextToken))
        {
            var identificatorNode = ParseIdentificator(lexer);

            childNodes = [identificatorNode];
        }
        else
            throw new ParsingException(lexer.NextTokenPosition);

        return new TreeNode("<primary expression>", childNodes);
    }

    private TreeNode ParseDashArithmeticExpression(ILexer lexer)
    {
        TreeNode[] childNodes;
        
        if (IsSumOperation(lexer.NextToken))
        {
            var sumOperationNode = ParseSumOperation(lexer);
            var termNode = ParseTerm(lexer);
            var dashArithmeticExpressions = ParseDashArithmeticExpression(lexer);

            childNodes = [sumOperationNode, termNode, dashArithmeticExpressions];
        }
        else
        {
            childNodes = [new TreeNode(EPSILON)];
        }

        return new TreeNode("<arithmetical expression>'", childNodes);
    }
    
    private TreeNode ParseDashTerm(ILexer lexer)
    {
        TreeNode[] childNodes;
        
        if (IsMultiplyOperation(lexer.NextToken))
        {
            var multiplyOperationNode = ParseMultiplyOperation(lexer);
            var factorNode = ParseFactor(lexer);
            var dashTermNode = ParseDashTerm(lexer);

            childNodes = [multiplyOperationNode, factorNode, dashTermNode];
        }
        else
        {
            childNodes = [new TreeNode(EPSILON)];
        }

        return new TreeNode("<term>'", childNodes);
    }
    
    private TreeNode ParseDashFactor(ILexer lexer)
    {
        TreeNode[] childNodes;
        
        if (lexer.NextToken == "^")
        {
            var arrowNode = new TreeNode("^");
            lexer.MoveNext();

            var primaryExpressionNode = ParsePrimaryExpression(lexer);
            var dashFactorNode = ParseDashFactor(lexer);

            childNodes = [arrowNode, primaryExpressionNode, dashFactorNode];
        }
        else
        {
            childNodes = [new TreeNode(EPSILON)];
        }

        return new TreeNode("<factor>'", childNodes);
    }
    
    private TreeNode ParseIdentificator(ILexer lexer)
    {
        var identificator = lexer.NextToken;
        
        lexer.MoveNext();
        
        return new TreeNode("<id>", [new TreeNode(identificator)]);
    }

    private TreeNode ParseSumOperation(ILexer lexer)
    {
        var sumOperation = lexer.NextToken;

        lexer.MoveNext();

        return new TreeNode("<sum operation symbol>", [new TreeNode(sumOperation)]);
    }

    private TreeNode ParseMultiplyOperation(ILexer lexer)
    {
        var multiplyOperation = lexer.NextToken;

        lexer.MoveNext();

        return new TreeNode("<multiply operation symbol>", [new TreeNode(multiplyOperation)]);
    }

    private TreeNode ParseRelationshipOperation(ILexer lexer)
    {
        var relationshipOperation = lexer.NextToken;

        lexer.MoveNext();

        return new TreeNode("<relationship operation symbol>", [new TreeNode(relationshipOperation)]);
    }

    private TreeNode ParseNumber(ILexer lexer)
    {
        var number = int.Parse(lexer.NextToken);

        lexer.MoveNext();

        return new TreeNode("<number>", [new TreeNode(number.ToString())]);
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