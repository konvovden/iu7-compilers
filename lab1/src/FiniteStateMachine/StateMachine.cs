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
        int finalState)
    {
        States = states.ToHashSet();
        Transitions = transitions.ToHashSet();
        InitialState = initialState;
        FinalStates = new HashSet<int> {finalState};

        Symbols = Transitions
            .Select(t => t.Input)
            .Distinct()
            .ToHashSet();
    }
}