namespace FiniteStateMachine;

public interface IStateMachine
{
    IReadOnlySet<int> States { get; }
    IReadOnlySet<StateTransition> Transitions { get; }
    IReadOnlySet<char> Symbols { get; }
    int InitialState { get; }
    IReadOnlySet<int> FinalStates { get; }

    bool Simulate(string inputString);
}