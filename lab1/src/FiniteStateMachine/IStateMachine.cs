namespace FiniteStateMachine;

public interface IStateMachine
{
    public IReadOnlyCollection<int> States { get; }
    public IReadOnlyCollection<StateTransition> Transitions { get; }
    public int InitialState { get; }
    public int FinalState { get; }
}