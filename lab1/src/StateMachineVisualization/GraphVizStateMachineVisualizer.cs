using csdot;
using csdot.Attributes.DataTypes;
using csdot.Attributes.Types;
using FiniteStateMachine;
using GraphVizNet;

namespace StateMachineVisualization;

public static class GraphVizStateMachineVisualizer
{
    public static void SaveStateMachineGraphToFile(IStateMachine stateMachine,
        string filename)
    {
        var graphString = ConvertStateMachineToGraphVizString(stateMachine);

        File.WriteAllText(filename + ".viz", graphString);
        
        var fileBytes = ConvertGraphVizToPng(graphString);
        
        File.WriteAllBytes(filename + ".png", fileBytes);
    }

    private static string ConvertStateMachineToGraphVizString(IStateMachine stateMachine)
    {
        var graph = new Graph("state_machine");
        graph.type = "digraph";

        foreach (var state in stateMachine.States)
        {
            if (state == stateMachine.InitialState)
            {
                
            }
            
            graph.AddElement(new Node(state.ToString()));
        }
        
        foreach (var transition in stateMachine.Transitions)
        {
            var edge = new Edge()
            {
                Transition = new List<Transition>()
                {
                    new Transition(transition.InitialState.ToString(), EdgeOp.directed),
                    new Transition(transition.ResultState.ToString(), EdgeOp.unspecified)
                }
            };
            
            edge.Attribute.label = new Label()
            {
                Value = " " + transition.Input
            };

            graph.AddElement(edge);
        }
        
        
        return graph.ElementToString();
    }

    private static byte[] ConvertGraphVizToPng(string graphVizString)
    {
        var graphViz = new GraphViz
        {
            Config =
            {
                GraphVizBinariesDirectory = "/bin/"
            }
        };

        return graphViz.LayoutAndRenderDotGraph(graphVizString, "png");
    }
}