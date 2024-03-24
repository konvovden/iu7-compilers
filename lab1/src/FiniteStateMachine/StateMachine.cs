namespace FiniteStateMachine;

public class StateMachine : IStateMachine
{
    public IReadOnlySet<int> States { get; }
    public IReadOnlySet<StateTransition> Transitions { get; }
    public IReadOnlySet<char> Symbols { get; }
    public int InitialState { get; }
    public IReadOnlySet<int> FinalStates { get; }

    public StateMachine(IEnumerable<int> states,
        IEnumerable<StateTransition> transitions,
        int initialState,
        IEnumerable<int> finalStates)
    {
        States = states.ToHashSet();
        Transitions = transitions.ToHashSet();
        InitialState = initialState;
        FinalStates = finalStates.ToHashSet();

        Symbols = Transitions
            .Select(t => t.Input)
            .Distinct()
            .ToHashSet();
    }
    
    public StateMachine(IEnumerable<int> states,
        IEnumerable<StateTransition> transitions,
        int initialState,
        int finalState) : this(states, transitions, initialState, [finalState])
    {
        
    }
    
    public bool Simulate(string inputString)
    {
        var currentState = InitialState;
        
        foreach (var symbol in inputString)
        {
            var transition = Transitions
                .FirstOrDefault(t => t.InitialState == currentState && t.Input == symbol);

            if (transition is null)
                return false;

            currentState = transition.ResultState;
        }

        return FinalStates.Contains(currentState);
    }
}