using FiniteStateMachine;

namespace StateMachineVisualization;

public class ConsoleStateMachineWriter
{
    public static void WriteStateMachineToConsole(IStateMachine stateMachine)
    {
        Console.WriteLine("StateMachine Info:");
        Console.WriteLine("States:");
        foreach (var state in stateMachine.States)
        {
            Console.WriteLine(state);
        }

        Console.WriteLine();
        Console.WriteLine("Transitions:");

        foreach (var transition in stateMachine.Transitions)
        {
            Console.WriteLine($"{transition.InitialState} -> {transition.ResultState} with {transition.Input}");
        }

        Console.WriteLine();
        Console.WriteLine($"Initial: {stateMachine.InitialState}");
        Console.WriteLine($"Final: {stateMachine.FinalState}");

        Console.ReadKey();
    }
}