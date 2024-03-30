using FiniteStateMachine;

namespace StateMachineUtils;

public class StateTransitionEqualityComparer : IEqualityComparer<StateTransition>
{
    public bool Equals(StateTransition x, StateTransition y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.InitialState == y.InitialState && x.Input == y.Input && x.ResultState == y.ResultState;
    }

    public int GetHashCode(StateTransition obj)
    {
        return HashCode.Combine(obj.InitialState, obj.Input, obj.ResultState);
    }
}