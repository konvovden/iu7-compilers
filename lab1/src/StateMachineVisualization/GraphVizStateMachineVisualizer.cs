using csdot;
using csdot.Attributes.DataTypes;
using csdot.Attributes.Types;
using FiniteStateMachine;
using GraphVizNet;

namespace StateMachineVisualization;

public static class GraphVizStateMachineVisualizer
{
    private const string BEGIN_NODE_ID = "BEGIN";
    
    public static void SaveStateMachineGraphToFile(IStateMachine stateMachine,
        string filename)
    {
        var graphString = ConvertStateMachineToGraphVizString(stateMachine);

        var graphStringPath = filename + ".viz";
        File.WriteAllText(graphStringPath, graphString);

        Console.WriteLine($"Saved StateMachine to file '{graphStringPath}'");
        
        var fileBytes = ConvertGraphVizToPng(graphString);

        var pngPath = filename + ".png";
        File.WriteAllBytes(pngPath, fileBytes);

        Console.WriteLine($"Saved StateMachine image to file '{pngPath}'");
    }

    private static string ConvertStateMachineToGraphVizString(IStateMachine stateMachine)
    {
        var graph = new Graph("state_machine");
        graph.type = "digraph";
        graph.Attribute.rankdir.Value = "LR";

        var beginNode = new Node(BEGIN_NODE_ID);
        beginNode.Attribute.shape.Value = "point";
        beginNode.Attribute.label.Value = "";
        
        graph.AddElement(beginNode);
        
        foreach (var state in stateMachine.States.Order())
        {
            var node = new Node(state.ToString());

            if (state == stateMachine.InitialState || stateMachine.FinalStates.Contains(state))
                node.Attribute.shape.Value = "doublecircle";
            else
                node.Attribute.shape.Value = "circle";
            
            graph.AddElement(node);
        }
        
        graph.AddElement(new Edge()
        {
            Transition = new List<Transition>()
            {
                new Transition(BEGIN_NODE_ID, EdgeOp.directed),
                new Transition(stateMachine.InitialState.ToString(), EdgeOp.unspecified)
            }
        });
        
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
                //GraphVizBinariesDirectory = "/bin/"
            }
        };

        return graphViz.LayoutAndRenderDotGraph(graphVizString, "png");
    }
}