namespace FiniteStateMachine;

public interface IStateMachine
{
    public IReadOnlyCollection<string> States { get; }
    public IReadOnlyCollection<StateTransition> Transitions { get; }
    public string InitialState { get; }
    public string FinalState { get; }
}