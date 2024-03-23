using FiniteStateMachine;

namespace StateMachineDeterminer;

public interface IStateMachineDeterminer
{
    IStateMachine DetermineStateMachine(IStateMachine stateMachine);
}