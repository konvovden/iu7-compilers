namespace FiniteStateMachine;

public interface IStateMachine
{
    public IReadOnlySet<int> States { get; }
    public IReadOnlySet<StateTransition> Transitions { get; }
    public IReadOnlySet<char> Symbols { get; }
    public int InitialState { get; }
    public IReadOnlySet<int> FinalStates { get; }
}