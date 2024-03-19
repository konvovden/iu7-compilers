using FiniteStateMachine;
using Grammar;
using RegularExpressionStateMachineBuilder.Converters;

namespace RegularExpressionStateMachineBuilder;

public class ThompsonRegexStateMachineBuilder : IRegexStateMachineBuilder
{
    private readonly AlphabetDefinition _alphabetDefinition;
    
    public ThompsonRegexStateMachineBuilder(AlphabetDefinition alphabetDefinition)
    {
        _alphabetDefinition = alphabetDefinition;
    }

    public IStateMachine BuildStateMachineFromRegularExpression(string regularExpression)
    {
        var expression = PrepareExpression(regularExpression);
        
        // (a|b)*cd
        // Postfix: a b | * c _ d _
        
        // (a|b)*_(c|d)
        // Postfix: a b | * c d | _
        
        // (a|b)*cd*(f|e)
        // Postfix: a b | * c _ d * _ f e | _
        
        // (a|b((()
        // Error
        
        // (a|(d(fe)))c
        // Postfix: a d f e _ _ | c _

        var stateMachine = CreateStateMachineFromExpression(expression); // TOOD: Exceptions handling

        return stateMachine;
    }

    private IStateMachine CreateStateMachineFromExpression(string expression)
    {
        var stack = new Stack<IStateMachine>();

        foreach (var symbol in expression)
        {
            if (_alphabetDefinition.IsLetter(symbol))
            {
                var letterStateMachine = CreateStateMachineFromLetter(symbol);
                stack.Push(letterStateMachine);
            }
            else if(_alphabetDefinition.IsConcatSymbol(symbol))
            {
                var right = stack.Pop();
                var left = stack.Pop();

                var concatStateMachine = ConcatStateMachines(left, right);
                
                stack.Push(concatStateMachine);
            }
            else if (_alphabetDefinition.IsUnionSymbol(symbol))
            {
                var right = stack.Pop();
                var left = stack.Pop();

                var unionStateMachine = UnionStateMachines(left, right);
                
                stack.Push(unionStateMachine);
            }
            else if (_alphabetDefinition.IsKleeneStarSymbol(symbol))
            {
                var stateMachine = stack.Pop();

                var kleeneStarStateMachine = AddKleeneStarToStateMachine(stateMachine);

                stack.Push(kleeneStarStateMachine);
            }
            else
                throw new InvalidOperationException("Invalid symbol"); // TODO:
        }

        return stack.Pop();
    }
    
    private IStateMachine CreateStateMachineFromLetter(char letter)
    {
        var states = new List<int>
        {
            0,
            1
        };

        var transitions = new List<StateTransition>
        {
            new(states[0], letter.ToString(), states[1])
        };

        return new StateMachine(states,
            transitions,
            states[0],
            states[1]);
    }

    private IStateMachine ConcatStateMachines(IStateMachine left, IStateMachine right)
    {
        var deduplicatedRight = RemoveDuplicateStates(right, left);
        
        var stateChanges = new Dictionary<int, int>
        {
            {deduplicatedRight.InitialState, left.FinalState}
        };
        
        var concatRight = ChangeStateMachineStates(deduplicatedRight, stateChanges);
        
        var states = left.States
            .Concat(concatRight.States)
            .Distinct()
            .ToList();

        var transitions = left.Transitions
            .Concat(concatRight.Transitions)
            .ToList();

        return new StateMachine(states,
            transitions,
            left.InitialState,
            concatRight.FinalState);
    }

    private IStateMachine UnionStateMachines(IStateMachine left, IStateMachine right)
    {
        throw new NotImplementedException();
    }

    private IStateMachine AddKleeneStarToStateMachine(IStateMachine stateMachine)
    {
        throw new NotImplementedException();
    }

    private static IStateMachine RemoveDuplicateStates(IStateMachine from, IStateMachine with)
    {
        var changes = new Dictionary<int, int>();

        var lastUsedState = with.States.Order().Last();
        
        foreach (var state in from.States)
        {
            if (with.States.Contains(state))
                changes[state] = ++lastUsedState;
        }

        return ChangeStateMachineStates(from, changes);
    }
    
    private static IStateMachine ChangeStateMachineStates(IStateMachine stateMachine, IReadOnlyDictionary<int, int> changes)
    {
        var states = stateMachine.States
            .Select(s => changes.GetValueOrDefault(s, s))
            .ToList();

        var transitions = stateMachine.Transitions
            .Select(t =>
                new StateTransition(changes.GetValueOrDefault(t.InitialState, t.InitialState),
                    t.Input,
                    changes.GetValueOrDefault(t.ResultState, t.ResultState)))
            .ToList();

        var initialState = changes.GetValueOrDefault(stateMachine.InitialState, stateMachine.InitialState);
        var finalState = changes.GetValueOrDefault(stateMachine.FinalState, stateMachine.FinalState);

        return new StateMachine(states,
            transitions,
            initialState,
            finalState);
    }
    
    private string PrepareExpression(string expression)
    {
        Console.WriteLine("Preparing expression...");
        
        var concatSymbolsConverter = new ConcatSymbolsConverter(_alphabetDefinition);
        var expressionWithConcatSymbols = concatSymbolsConverter.InsertConcatSymbols(expression);
        
        Console.WriteLine($"Added concat symbols: '{expressionWithConcatSymbols}'");

        var postfixNotationConverter = new PostfixNotationConverter(_alphabetDefinition);
        var postfixExpression = postfixNotationConverter.ConvertToPostfixNotation(expressionWithConcatSymbols);
        
        Console.WriteLine($"Converted to postfix form: '{postfixExpression}'");

        return postfixExpression;
    }
}