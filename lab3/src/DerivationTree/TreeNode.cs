namespace DerivationTree;

public class TreeNode
{
    public string Text { get; }
    public TreeNode[] ChildNodes { get; }

    public TreeNode(string text,
        TreeNode[]? childNodes = null)
    {
        Text = text;
        ChildNodes = childNodes ?? Array.Empty<TreeNode>();
    }
}