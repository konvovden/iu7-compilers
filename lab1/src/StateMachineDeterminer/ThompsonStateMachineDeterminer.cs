using FiniteStateMachine;
using Grammar;

namespace StateMachineDeterminer;

public class ThompsonStateMachineDeterminer : IStateMachineDeterminer
{
    private readonly AlphabetDefinition _alphabetDefinition;

    public ThompsonStateMachineDeterminer(AlphabetDefinition alphabetDefinition)
    {
        _alphabetDefinition = alphabetDefinition;
    }

    public IStateMachine DetermineStateMachine(IStateMachine stateMachine)
    {
        var newStatesSets = new List<HashSet<int>>();
        var newTransitionsSets = new HashSet<(HashSet<int>, char, HashSet<int>)>();

        var queue = new Queue<HashSet<int>>();

        var initialStateSet = CalculateEClosure(stateMachine, stateMachine.InitialState);

        queue.Enqueue(initialStateSet);

        while (queue.Count != 0)
        {
            var currentState = queue.Dequeue();

            newStatesSets.Add(currentState);

            foreach (var symbol in stateMachine.Symbols.Where(s => s != _alphabetDefinition.EpsilonSymbol))
            {
                var moveForCurrentState = CalculateMove(stateMachine, currentState, symbol);
                
                if (moveForCurrentState.Count != 0)
                {
                    var possibleStates = CalculateEClosure(stateMachine, moveForCurrentState);
                
                    if(!newStatesSets.Any(s => s.SetEquals(possibleStates)))
                        queue.Enqueue(possibleStates);

                    newTransitionsSets.Add((currentState, symbol, possibleStates));
                }
            }
        }

        var newStates = Enumerable.Range(0, newStatesSets.Count);

        var transitions = newTransitionsSets
            .Select(t =>
                new StateTransition(newStatesSets.FindIndex(s => s.SetEquals(t.Item1)),
                    t.Item2,
                    newStatesSets.FindIndex(s => s.SetEquals(t.Item3))))
            .Distinct(new StateTransitionComparer());

        newStates = newStates
            .Where(s => transitions.Any(t => t.InitialState == s || t.ResultState == s));
        
        var initialState = newStatesSets
            .Where(s => s.Contains(stateMachine.InitialState))
            .Select(s => newStatesSets.IndexOf(s))
            .First();

        var finalStates = newStatesSets
            .Where(s => s.Overlaps(stateMachine.FinalStates))
            .Select(s => newStatesSets.IndexOf(s));

        return new StateMachine(newStates,
            transitions,
            initialState,
            finalStates);
    }

    private HashSet<int> CalculateEClosure(IStateMachine stateMachine, int state)
    {
        return CalculateEClosure(stateMachine, [state]);
    }

    private HashSet<int> CalculateEClosure(IStateMachine stateMachine, HashSet<int> states)
    {
        var result = new HashSet<int>();
        var stack = new Stack<int>();

        foreach (var state in states)
        {
            stack.Push(state);
            result.Add(state);
        }

        while (stack.Count != 0)
        {
            var currentState = stack.Pop();

            var transitionStates = stateMachine
                .Transitions
                .Where(t => t.InitialState == currentState && t.Input == _alphabetDefinition.EpsilonSymbol)
                .Select(t => t.ResultState);

            foreach (var state in transitionStates)
            {
                if (!result.Contains(state))
                {
                    result.Add(state);
                    stack.Push(state);
                }
            }
        }

        return result;
    }

    private HashSet<int> CalculateMove(IStateMachine stateMachine,
        HashSet<int> states,
        char symbol)
    {
        return stateMachine.Transitions
            .Where(t => states.Contains(t.InitialState) && t.Input == symbol)
            .Select(t => t.ResultState)
            .ToHashSet();
    }
    
    private class StateTransitionComparer : IEqualityComparer<StateTransition>
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
}