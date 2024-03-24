using FiniteStateMachine;

namespace StateMachineMinimizer;

public interface IStateMachineMinimizer
{
    IStateMachine MinimizeStateMachine(IStateMachine stateMachine);
}