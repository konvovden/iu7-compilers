﻿using System.Text;
using DerivationTree;
using csdot;
using csdot.Attributes.DataTypes;
using GraphVizNet;

namespace TreeVisualization;

public static class GraphvizTreeVisualizer
{
    public static void SaveTreeToFile(Tree tree, string filename)
    {
        var graphString = ConvertToGraphVizString(tree);

        var graphStringPath = filename + ".viz";
        File.WriteAllText(graphStringPath, graphString);

        Console.WriteLine($"Saved Tree to file '{graphStringPath}'");
        
        var fileBytes = ConvertGraphVizToPng(graphString);

        var pngPath = filename + ".png";
        File.WriteAllBytes(pngPath, fileBytes);

        Console.WriteLine($"Saved Tree image to file '{pngPath}'");
    }

    private static string ConvertToGraphVizString(Tree tree)
    {
        var graph = new Graph("tree");

        var nodes = GetNodesList(tree);

        for (var i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];

            var graphNode = new Node(i.ToString());
            graphNode.Attribute.label.Value = node.Text;

            var graphEdges = node.ChildNodes
                .Select(n => new Edge()
                {
                    Transition =
                    [
                        new Transition(i.ToString(), EdgeOp.undirected),
                        new Transition(nodes.IndexOf(n).ToString(), EdgeOp.unspecified)
                    ]
                })
                .ToList();
            
            graph.AddElement(graphNode);
            
            graphEdges.ForEach(e => graph.AddElement(e));
        }

        return graph.ElementToString();
    }
    
    private static byte[] ConvertGraphVizToPng(string graphVizString)
    {
        var graphViz = new GraphViz
        {
            Config =
            {
                GraphVizBinariesDirectory = "/bin"
                //GraphVizBinariesDirectory = "bin/Debug/net8.0/graphviz"
            }
        };

        
        return graphViz.LayoutAndRenderDotGraph(graphVizString, "png");
    }

    private static List<TreeNode> GetNodesList(Tree tree)
    {
        var nodesList = new List<TreeNode>();

        var nodesQueue = new Queue<TreeNode>();
        nodesQueue.Enqueue(tree.RootNode);
        
        while (nodesQueue.Count > 0)
        {
            var currentNode = nodesQueue.Dequeue();
            nodesList.Add(currentNode);

            foreach (var childNode in currentNode.ChildNodes)
                nodesQueue.Enqueue(childNode);
        }

        return nodesList;
    }
}