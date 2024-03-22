namespace FiniteStateMachine;

public class StateTransition
{
    public int InitialState { get; }
    public char Input { get; }
    public int ResultState { get; }

    public StateTransition(int initialState, char input, int resultState)
    {
        InitialState = initialState;
        Input = input;
        ResultState = resultState;
    }
}