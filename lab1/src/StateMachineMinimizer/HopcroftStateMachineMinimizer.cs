using FiniteStateMachine;
using StateMachineUtils;

namespace StateMachineMinimizer;

public class HopcroftStateMachineMinimizer : IStateMachineMinimizer
{
    public IStateMachine MinimizeStateMachine(IStateMachine stateMachine)
    {
        var equivalentClasses = FindEquivalentClasses(stateMachine);

        var newStates = Enumerable.Range(0, equivalentClasses.Count);

        var newTransitions = stateMachine.Transitions
            .Select(t => new StateTransition(equivalentClasses.FindIndex(c => c.Contains(t.InitialState)),
                t.Input,
                equivalentClasses.FindIndex(c => c.Contains(t.ResultState))))
            .Distinct(new StateTransitionEqualityComparer());

        newStates = newStates
            .Where(s => newTransitions.Any(t => t.InitialState == s || t.ResultState == s));
        
        var initialState = equivalentClasses.FindIndex(c => c.Contains(stateMachine.InitialState));

        var finalStates = stateMachine.FinalStates
            .Select(s => equivalentClasses.FindIndex(c => c.Contains(s)))
            .Distinct();

        return new StateMachine(newStates,
            newTransitions,
            initialState,
            finalStates);
    }

    private List<HashSet<int>> FindEquivalentClasses(IStateMachine stateMachine)
    {
        var Inv = new Dictionary<int, Dictionary<char, int[]>>();

        foreach (var state in stateMachine.States)
        {
            Inv[state] = new Dictionary<char, int[]>();
            
            foreach (var symbol in stateMachine.Symbols)
            {
                Inv[state][symbol] = stateMachine.Transitions
                    .Where(t => t.Input == symbol && t.ResultState == state)
                    .Select(t => t.InitialState)
                    .ToArray();
            }
        }
        
        var P = new List<HashSet<int>>();

        var F = stateMachine.FinalStates.ToHashSet();
        
        var QwithoutF = stateMachine.States
            .Except(stateMachine.FinalStates)
            .ToHashSet();
        
        P.Add(F);
        P.Add(QwithoutF);

        var queue = new Queue<(HashSet<int>, char)>();
        
        foreach (var c in stateMachine.Symbols)
        {
            queue.Enqueue((F, c));
            queue.Enqueue((QwithoutF, c));
        }

        while (queue.Count != 0)
        {
            var (C, a) = queue.Dequeue();

            var Involved = new Dictionary<int, List<int>>();

            foreach (var q in C)
            {
                foreach (var r in Inv[q][a])
                {
                    var i = P.FindIndex(s => s.Contains(r));

                    if (!Involved.ContainsKey(i))
                        Involved[i] = [];

                    Involved[i].Add(r);
                }
            }

            foreach (var i in Involved.Keys)
            {
                if (Involved[i].Count < P[i].Count)
                {
                    P.Add([]);
                    
                    var j = P.Count - 1;

                    foreach (var r in Involved[i])
                    {
                        P[i].Remove(r);
                        P[j].Add(r);
                    }

                    if (P[j].Count > P[i].Count)
                        (P[j], P[i]) = (P[i], P[j]);

                    foreach (var symbol in stateMachine.Symbols)
                    {
                        queue.Enqueue((P[j], symbol));
                    }
                }
            }
        }

        return P;
    }
}