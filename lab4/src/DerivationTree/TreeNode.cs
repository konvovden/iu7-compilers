namespace DerivationTree;

public class TreeNode
{
    public string Text { get; }
    public string Attribute { get; set; }
    public TreeNode[] ChildNodes { get; }

    public TreeNode(string text,
        string attribute,
        TreeNode[]? childNodes = null)
    {
        Text = text;
        Attribute = attribute.Trim();
        ChildNodes = childNodes ?? Array.Empty<TreeNode>();
    }
}