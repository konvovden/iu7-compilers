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

        var stateMachine = CreateStateMachineFromExpression(expression); 

        return BeautifyStateMachine(stateMachine);
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
                throw new InvalidOperationException("Invalid symbol"); 
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
            new(states[0], letter, states[1])
        };

        return new StateMachine(states,
            transitions,
            states[0],
            states[1]);
    }

    private IStateMachine ConcatStateMachines(IStateMachine left, IStateMachine right)
    {
        var deduplicatedRight = RemoveDuplicateStates(right, left);
        
        /*var stateChanges = new Dictionary<int, int>
        {
            {deduplicatedRight.InitialState, left.FinalStates.First()}
        };
        
        var concatRight = ChangeStateMachineStates(deduplicatedRight, stateChanges);*/
        
        var states = left.States
            .Concat(deduplicatedRight.States)
            .Distinct()
            .ToList();
        
        var transitions = left.Transitions
            .Concat(deduplicatedRight.Transitions)
            .ToList();
        
        transitions.Add(new StateTransition(left.FinalStates.First(), _alphabetDefinition.EpsilonSymbol, deduplicatedRight.InitialState));

        return new StateMachine(states,
            transitions,
            left.InitialState,
            deduplicatedRight.FinalStates.First());
    }

    private IStateMachine UnionStateMachines(IStateMachine left, IStateMachine right)
    {
        var deduplicatedRight = RemoveDuplicateStates(right, left);

        var states = left.States
            .Concat(deduplicatedRight.States)
            .Order()
            .ToList();

        var initialState = states.Last() + 1;
        var finalState = initialState + 1;
        
        states.Add(initialState);
        states.Add(finalState);

        var transitions = left.Transitions
            .Concat(deduplicatedRight.Transitions)
            .ToList();

        transitions.Add(new StateTransition(initialState, 
            _alphabetDefinition.EpsilonSymbol,
            left.InitialState));
        
        transitions.Add(new StateTransition(initialState,
            _alphabetDefinition.EpsilonSymbol,
            deduplicatedRight.InitialState));
        
        transitions.Add(new StateTransition(left.FinalStates.First(),
            _alphabetDefinition.EpsilonSymbol,
            finalState));
        
        transitions.Add(new StateTransition(deduplicatedRight.FinalStates.First(),
            _alphabetDefinition.EpsilonSymbol,
            finalState));

        return new StateMachine(states,
            transitions,
            initialState,
            finalState);
    }

    private IStateMachine AddKleeneStarToStateMachine(IStateMachine stateMachine)
    {
        var states = stateMachine.States
            .Order()
            .ToList();

        var initialState = states.Last() + 1;
        var finalState = initialState + 1;

        states.Add(initialState);
        states.Add(finalState);
        
        var transitions = stateMachine.Transitions
            .ToList();
        
        transitions.Add(new StateTransition(stateMachine.FinalStates.First(), 
            _alphabetDefinition.EpsilonSymbol,
            stateMachine.InitialState));
        
        transitions.Add(new StateTransition(initialState, 
            _alphabetDefinition.EpsilonSymbol,
            stateMachine.InitialState));
        
        transitions.Add(new StateTransition(stateMachine.FinalStates.First(),
            _alphabetDefinition.EpsilonSymbol,
            finalState));
        
        transitions.Add(new StateTransition(initialState,
            _alphabetDefinition.EpsilonSymbol,
            finalState));

        return new StateMachine(states,
            transitions,
            initialState,
            finalState);
    }

    private static IStateMachine RemoveDuplicateStates(IStateMachine from, IStateMachine with)
    {
        var changes = new Dictionary<int, int>();

        var lastUsedState = with.States.Order().Last();
        
        foreach (var state in from.States)
        {
            if (with.States.Contains(state) || changes.ContainsValue(state))
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
        var finalState = changes.GetValueOrDefault(stateMachine.FinalStates.First(), stateMachine.FinalStates.First());

        return new StateMachine(states,
            transitions,
            initialState,
            finalState);
    }

    private IStateMachine BeautifyStateMachine(IStateMachine stateMachine)
    {
        var changes = new Dictionary<int, int>()
        {
            {stateMachine.InitialState, 0},
            {stateMachine.FinalStates.First(), stateMachine.States.Count + 1}
        };
        
        var innerStates = stateMachine.States
            .Order()
            .Where(s => s != stateMachine.InitialState && s != stateMachine.FinalStates.First())
            .ToList();

        for (var i = 0; i < innerStates.Count; i++)
        {
            changes[innerStates[i]] = i + 1;
        }

        return ChangeStateMachineStates(stateMachine, changes);
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