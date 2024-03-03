namespace FiniteStateMachine;

public class StateTransition
{
    public string InitialState { get; }
    public string Input { get; }
    public string ResultState { get; }

    public StateTransition(string initialState, string input, string resultState)
    {
        InitialState = initialState;
        Input = input;
        ResultState = resultState;
    }
}