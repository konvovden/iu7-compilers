namespace FiniteStateMachine;

public class StateMachine : IStateMachine
{
    public IReadOnlyCollection<int> States { get; }
    public IReadOnlyCollection<StateTransition> Transitions { get; }
    public int InitialState { get; }
    public int FinalState { get; }

    public StateMachine(IReadOnlyCollection<int> states,
        IReadOnlyCollection<StateTransition> transitions,
        int initialState,
        int finalState)
    {
        States = states;
        Transitions = transitions;
        InitialState = initialState;
        FinalState = finalState;
    }
}