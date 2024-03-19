namespace FiniteStateMachine;

public class StateTransition
{
    public int InitialState { get; }
    public string Input { get; }
    public int ResultState { get; }

    public StateTransition(int initialState, string input, int resultState)
    {
        InitialState = initialState;
        Input = input;
        ResultState = resultState;
    }
}